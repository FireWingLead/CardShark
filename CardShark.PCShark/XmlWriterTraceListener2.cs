using System.Runtime.Versioning;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System;
using System.IO;
using System.Globalization;
using System.Collections;
using System.Security.Permissions;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners;
using System.Threading;
 
namespace System.Diagnostics {
	[HostProtection(Synchronization = true)]
	[ConfigurationElementType(typeof(TraceListenerData))]
    public class XmlWriterTraceListener2 : CustomTraceListener
	{
        private const string fixedHeaderL1 = "<E2ETraceEvent xmlns=\"http://schemas.microsoft.com/2004/06/E2ETraceEvent\">";
		private const string fixedHeaderL2 = "<System xmlns=\"http://schemas.microsoft.com/2004/06/windows/eventlog/system\">";
		private readonly string machineName = Environment.MachineName;
        private StringBuilder strBldr = null;
        private XmlTextWriter xmlBlobWriter = null;
		protected string fileName = null;
		protected TextWriter writer;
        
        // Previously we had a bug where TraceTransfer did not respect the filter set on this listener.  We're fixing this
        // bug, but only for cases where the filter was set via config.  In the next side by side release, we'll remove
        // this and always respect the filter for TraceTransfer events.
        internal bool shouldRespectFilterOnTraceTransfer;

		public XmlWriterTraceListener2(Stream stream) : this(stream, string.Empty) { }
		public XmlWriterTraceListener2(Stream stream, string name) {
			Name = name;
			if (stream == null) throw new ArgumentNullException("stream");
			this.writer = new StreamWriter(stream);   
		}

		[ResourceExposure(ResourceScope.Machine)]
		[ResourceConsumption(ResourceScope.Machine)]
		public XmlWriterTraceListener2(string filename) : this(filename, string.Empty) { }
 
        [ResourceExposure(ResourceScope.Machine)]
        [ResourceConsumption(ResourceScope.Machine)]
		public XmlWriterTraceListener2(string filename, string name) {
			this.fileName = filename;
			this.Name = name;
		}

		public bool escapeMsgs = false;
		public bool EscapeMessages { get { return escapeMsgs; } set { escapeMsgs = EscapeNextMessage = value; } }
		public bool EscapeNextMessage { get; set; }
 
        public override void Write(string message) {
            this.WriteLine(message);
        }
 
        public override void WriteLine(string message) {
            this.TraceEvent(null, "Unspecified", TraceEventType.Information, 0, message);
        }
 
        public override void Fail(string message, string detailMessage) {
            StringBuilder failMessage = new StringBuilder(message);
            if (detailMessage != null) {
                failMessage.Append(" ");
                failMessage.Append(detailMessage);
            }

			this.TraceEvent(null, "Unspecified", TraceEventType.Error, 0, failMessage.ToString());
        }
 
        public override void TraceEvent(TraceEventCache eventCache, String source, TraceEventType eventType, int id, string format, params object[] args) {
			if (Filter != null && !Filter.ShouldTrace(eventCache, source, eventType, id, format, args, null, null))
                return;
 
            WriteHeader(source, eventType, id, eventCache);
 
            string message;
            if (args != null)
                message = String.Format(CultureInfo.InvariantCulture, format, args);
            else
                message = format;

			if (EscapeNextMessage)
				WriteEscaped(message);
			else
				InternalWrite(message);
            WriteFooter(eventCache);
			EscapeNextMessage = EscapeMessages;
        }
 
        public override void TraceEvent(TraceEventCache eventCache, String source, TraceEventType eventType, int id, string message) {
			if (Filter != null && !Filter.ShouldTrace(eventCache, source, eventType, id, message, null, null, null)) 
                return;

			WriteHeader(source, eventType, id, eventCache);
			if (EscapeNextMessage)
				WriteEscaped(message);
			else
				InternalWrite(message);
			WriteFooter(eventCache);
			EscapeNextMessage = EscapeMessages;
        }
 
        public override void TraceData(TraceEventCache eventCache, String source, TraceEventType eventType, int id, object data) {
            if (Filter != null && !Filter.ShouldTrace(eventCache, source, eventType, id, null, null, data, null)) 
                return;
 
            WriteHeader(source, eventType, id, eventCache);
            
            InternalWrite("<TraceData>");
            if (data != null) {
                InternalWrite("<DataItem>");
                WriteData(data);
                InternalWrite("</DataItem>");
            }
            InternalWrite("</TraceData>");
 
            WriteFooter(eventCache);
			EscapeNextMessage = EscapeMessages;
        }
 
        public override void TraceData(TraceEventCache eventCache, String source, TraceEventType eventType, int id, params object[] data) {
            if (Filter != null && !Filter.ShouldTrace(eventCache, source, eventType, id, null, null, null, data)) 
                return;
 
            WriteHeader(source, eventType, id, eventCache);
            InternalWrite("<TraceData>");
            if (data != null) {
                for (int i=0; i<data.Length; i++) {
                    InternalWrite("<DataItem>");
                    if (data[i] != null)
                        WriteData(data[i]);
                    InternalWrite("</DataItem>");
                }
            }
            InternalWrite("</TraceData>");

			WriteFooter(eventCache);
			EscapeNextMessage = EscapeMessages;
        }
 
        // Special case XPathNavigator dataitems to write out XML blob unescaped
        private void WriteData(object data) {
            XPathNavigator xmlBlob = data as XPathNavigator;

			if (xmlBlob == null) {
				if (EscapeNextMessage)
					WriteEscaped(data.ToString());
				else
					InternalWrite(data.ToString());
			}
			else {
				if (strBldr == null) {
					strBldr = new StringBuilder();
					xmlBlobWriter = new XmlTextWriter(new StringWriter(strBldr, CultureInfo.CurrentCulture));
				}
				else
					strBldr.Length = 0;

				try {
					// Rewind the blob to point to the root, this is needed to support multiple XMLTL in one TraceData call
					xmlBlob.MoveToRoot();
					xmlBlobWriter.WriteNode(xmlBlob, false);
					InternalWrite(strBldr.ToString());
				}
				catch (Exception) { // We probably only care about XmlException for ill-formed XML though 
					InternalWrite(data.ToString());
				}
			}
			EscapeNextMessage = EscapeMessages;
        }
 
        public override void Close() {
            base.Close();
            if (xmlBlobWriter != null) 
                xmlBlobWriter.Close();
            xmlBlobWriter = null;
            strBldr = null;
        }
 
        public override void TraceTransfer(TraceEventCache eventCache, String source, int id, string message, Guid relatedActivityId) {
            if (shouldRespectFilterOnTraceTransfer && (Filter != null && !Filter.ShouldTrace(eventCache, source, TraceEventType.Transfer, id, message, null, null, null)))
                return;
 
            WriteHeader(source, TraceEventType.Transfer, id, eventCache, relatedActivityId);
			if (EscapeNextMessage)
				WriteEscaped(message);
			else
				InternalWrite(message);
			WriteFooter(eventCache);
			EscapeNextMessage = EscapeMessages;
        }

		protected void WriteHeader(String source, TraceEventType eventType, int id, TraceEventCache eventCache, Guid relatedActivityId) {
            WriteStartHeader(source, eventType, id, eventCache);
            InternalWrite("\" RelatedActivityID=\"");
            InternalWrite(relatedActivityId.ToString("B"));
            WriteEndHeader(eventCache);
        }

		protected void WriteHeader(String source, TraceEventType eventType, int id, TraceEventCache eventCache) {
            WriteStartHeader(source, eventType, id, eventCache);
            WriteEndHeader(eventCache);
        }

		protected void WriteStartHeader(String source, TraceEventType eventType, int id, TraceEventCache eventCache) {
			string padding0 = string.Empty.PadLeft(IndentLevel, '\t');
			string padding1 = padding0 + "\t";
			string padding2 = padding0 + "\t\t";
			InternalWrite(padding0 + fixedHeaderL1 + "\n");
			InternalWrite(padding1 + fixedHeaderL2 + "\n");

			InternalWrite(padding2 + "<EventID>");
			InternalWrite(((uint)id).ToString(CultureInfo.InvariantCulture));
			InternalWrite("</EventID>\n");

			InternalWrite(padding2 + "<Type>3</Type>\n");

			InternalWrite(padding2 + "<SubType Name=\"");
			InternalWrite(eventType.ToString());
			InternalWrite("\">0</SubType>");

			InternalWrite(padding2 + "<Level>");
			int sev = (int)eventType;
			if (sev > 255)
				sev = 255;
			if (sev < 0)
				sev = 0;
			InternalWrite(sev.ToString(CultureInfo.InvariantCulture));
			InternalWrite("</Level>\n");

			InternalWrite(padding2 + "<TimeCreated SystemTime=\"");
			if (eventCache != null)
				InternalWrite(eventCache.DateTime.ToString("o", CultureInfo.InvariantCulture));
			else
				InternalWrite(DateTime.Now.ToString("o", CultureInfo.InvariantCulture));
			InternalWrite("\" />\n");

			InternalWrite(padding2 + "<Source Name=\"");
			WriteEscaped(source);
			InternalWrite("\" />\n");

			InternalWrite(padding2 + "<Correlation ActivityID=\"");
			if (eventCache != null)
				InternalWrite(eventCache.GetActivityId().ToString("B"));
			else
				InternalWrite(Guid.Empty.ToString("B"));
        }
 
        [ResourceExposure(ResourceScope.None)]
        [ResourceConsumption(ResourceScope.Process, ResourceScope.Process)]
		protected void WriteEndHeader(TraceEventCache eventCache) {
			string padding0 = string.Empty.PadLeft(IndentLevel, '\t');
			string padding1 = padding0 + "\t";
			string padding2 = padding0 + "\t\t";
			InternalWrite("\" />\n");

			InternalWrite(padding2 + "<Execution ProcessName=\"");
			InternalWrite(LoggingExtensions.GetProcessName());
			InternalWrite("\" ProcessID=\"");
			InternalWrite(((uint)LoggingExtensions.GetProcessId()).ToString(CultureInfo.InvariantCulture));
			InternalWrite("\" ThreadID=\"");
			if (eventCache != null)
				WriteEscaped(eventCache.ThreadId.ToString(CultureInfo.InvariantCulture));
			else
				WriteEscaped(LoggingExtensions.GetThreadId().ToString(CultureInfo.InvariantCulture));
			InternalWrite("\" />\n");

			InternalWrite(padding2 + "<Channel/>\n");

			InternalWrite(padding2 + "<Computer>");
			InternalWrite(machineName);
			InternalWrite("</Computer>\n");

			InternalWrite(padding1 + "</System>\n");

			InternalWrite(padding1 + "<ApplicationData>\n");
			InternalWrite(padding2 + "<TraceMessage>\n");
        }

		protected void WriteFooter(TraceEventCache eventCache) {
			string padding0 = string.Empty.PadLeft(IndentLevel, '\t');
			string padding1 = padding0 + "\t";
			string padding2 = padding0 + "\t\t";
			bool writeLogicalOps = IsEnabled(TraceOptions.LogicalOperationStack);
			bool writeCallstack = IsEnabled(TraceOptions.Callstack);
			bool writeTimestamp = IsEnabled(TraceOptions.Timestamp);
			
			InternalWrite(padding2 + "\n</TraceMessage>\n");
			
			if (eventCache != null && (writeLogicalOps || writeCallstack || writeTimestamp)) {
				string padding3 = padding0 + "\t\t\t";
				string padding4 = padding0 + "\t\t\t\t";
				InternalWrite(padding2 + "<System.Diagnostics xmlns=\"http://schemas.microsoft.com/2004/08/System.Diagnostics\">\n");

				if (writeLogicalOps) {
					InternalWrite("\n" + padding3 + "<LogicalOperationStack>\n");

					Stack s = eventCache.LogicalOperationStack as Stack;

					if (s != null) {
						foreach (object correlationId in s) {
							InternalWrite(padding4 + "<LogicalOperation><![CDATA[");
							InternalWrite(correlationId.ToString());
							InternalWrite("]]></LogicalOperation>\n");
						}
					}
					InternalWrite("</LogicalOperationStack>");
				}

				InternalWrite(padding3 + "<Timestamp>");
				InternalWrite(eventCache.Timestamp.ToString(CultureInfo.InvariantCulture));
				InternalWrite("</Timestamp>\n");

				if (writeCallstack) {
					InternalWrite(padding3 + "<Callstack><![CDATA[");
					InternalWrite(eventCache.Callstack);
					InternalWrite("]]></Callstack>\n");
				}

				InternalWrite(padding2 + "</System.Diagnostics>\n");
			}

			InternalWrite(padding1 + "</ApplicationData>\n");
			InternalWrite(padding0 + "</E2ETraceEvent>\n");
        }

		protected void WriteEscaped(string str) {
			if (str == null)
				return;
			int lastIndex = 0;
			for (int i = 0; i < str.Length; i++) {
				switch (str[i]) {
					case '&':
						InternalWrite(str.Substring(lastIndex, i - lastIndex));
						InternalWrite("&amp;");
						lastIndex = i + 1;
						break;
					case '<':
						InternalWrite(str.Substring(lastIndex, i - lastIndex));
						InternalWrite("&lt;");
						lastIndex = i + 1;
						break;
					case '>':
						InternalWrite(str.Substring(lastIndex, i - lastIndex));
						InternalWrite("&gt;");
						lastIndex = i + 1;
						break;
					case '"':
						InternalWrite(str.Substring(lastIndex, i - lastIndex));
						InternalWrite("&quot;");
						lastIndex = i + 1;
						break;
					case '\'':
						InternalWrite(str.Substring(lastIndex, i - lastIndex));
						InternalWrite("&apos;");
						lastIndex = i + 1;
						break;
					case (char)0xD:
						InternalWrite(str.Substring(lastIndex, i - lastIndex));
						InternalWrite("&#xD;");
						lastIndex = i + 1;
						break;
					case (char)0xA:
						InternalWrite(str.Substring(lastIndex, i - lastIndex));
						InternalWrite("&#xA;");
						lastIndex = i + 1;
						break;
				}
			}
			InternalWrite(str.Substring(lastIndex, str.Length - lastIndex));
		}

		protected bool IsEnabled(TraceOptions opts) {
			return (opts & TraceOutputOptions) != 0;
		}
 
        protected void InternalWrite(string message) {
			if (!EnsureWriter()) return;   
            // NeedIndent is nop
            writer.Write(message);
        }

		// This uses a machine resource, scoped by the fileName variable.
		[ResourceExposure(ResourceScope.None)]
		[ResourceConsumption(ResourceScope.Machine, ResourceScope.Machine)]
		protected bool EnsureWriter() {
			bool ret = true;

			if (writer == null) {
				ret = false;

				if (fileName == null)
					return ret;

				// StreamWriter by default uses UTF8Encoding which will throw on invalid encoding errors.
				// This can cause the internal StreamWriter's state to be irrecoverable. It is bad for tracing 
				// APIs to throw on encoding errors. Instead, we should provide a "?" replacement fallback  
				// encoding to substitute illegal chars. For ex, In case of high surrogate character 
				// D800-DBFF without a following low surrogate character DC00-DFFF
				// NOTE: We also need to use an encoding that does't emit BOM whic is StreamWriter's default
				Encoding noBOMwithFallback = GetEncodingWithFallback(new UTF8Encoding(false));


				// To support multiple appdomains/instances tracing to the same file,
				// we will try to open the given file for append but if we encounter 
				// IO errors, we will prefix the file name with a unique GUID value 
				// and try one more time
				string fullPath = Path.GetFullPath(fileName);
				string dirPath = Path.GetDirectoryName(fullPath);
				string fileNameOnly = Path.GetFileName(fullPath);

				for (int i = 0; i < 2; i++) {
					try {
						writer = new StreamWriter(fullPath, true, noBOMwithFallback, 4096);
						ret = true;
						break;
					}
					catch (IOException) {

						// Should we do this only for ERROR_SHARING_VIOLATION?
						//if (InternalResources.MakeErrorCodeFromHR(Marshal.GetHRForException(ioexc)) == InternalResources.ERROR_SHARING_VIOLATION) {

						fileNameOnly = Guid.NewGuid().ToString() + fileNameOnly;
						fullPath = Path.Combine(dirPath, fileNameOnly);
						continue;
					}
					catch (UnauthorizedAccessException) {
						//ERROR_ACCESS_DENIED, mostly ACL issues
						break;
					}
					catch (Exception) {
						break;
					}
				}

				if (!ret) {
					// Disable tracing to this listener. Every Write will be nop.
					// We need to think of a central way to deal with the listener
					// init errors in the future. The default should be that we eat 
					// up any errors from listener and optionally notify the user
					fileName = null;
				}
			}
			return ret;
		}

		private static Encoding GetEncodingWithFallback(Encoding encoding) {
			// Clone it and set the "?" replacement fallback
			Encoding fallbackEncoding = (Encoding)encoding.Clone();
			fallbackEncoding.EncoderFallback = EncoderFallback.ReplacementFallback;
			fallbackEncoding.DecoderFallback = DecoderFallback.ReplacementFallback;

			return fallbackEncoding;
		}
    }

	public static class LoggingExtensions
	{
		public static Guid GetActivityId(this TraceEventCache eventCache) { return Trace.CorrelationManager.ActivityId; }

		private static volatile int processId;
		private static volatile string processName;

		[ResourceExposure(ResourceScope.None)]
		[ResourceConsumption(ResourceScope.Machine, ResourceScope.Machine)]
		private static void InitProcessInfo() {
			// Demand unmanaged code permission
			new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Demand();

			if (processName == null) {
				Process p = Process.GetCurrentProcess();
				try {
					processId = p.Id;
					processName = p.ProcessName;
				}
				finally {
					p.Dispose();
				}
			}
		}

		[ResourceExposure(ResourceScope.Process)]
		internal static int GetProcessId() {
			InitProcessInfo();
			return processId;
		}

		internal static string GetProcessName() {
			InitProcessInfo();
			return processName;
		}

		internal static int GetThreadId() {
			return Thread.CurrentThread.ManagedThreadId;
		}
	}
}