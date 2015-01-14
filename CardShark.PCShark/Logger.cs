using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace CardShark.PCShark
{
	public class Logger
	{
		private const string LOG_ERR_FMT = "<message><![CDATA[{0}]]></message><Exception><ExceptionType>{1}</ExceptionType><message><![CDATA[{2}]]></message><stacktrace><![CDATA[{3}]]></stacktrace></Exception>{4}";
		TraceSource tSource = new TraceSource("PCShark", SourceLevels.Verbose | SourceLevels.ActivityTracing);
		public Logger() {
			//tSource.Listeners.Add(new XmlListener("C:\\Users\\PerrinL\\Documents\\Visual Studio 2012\\Projects\\CardShark\\CardShark.PCShark\\bin\\Debug\\app_tracelog.svclog", "LocalListener"));
		}

		private static Logger defaultLogger = new Logger();
		public static Logger Default { get { return defaultLogger; } set { if (value != null) defaultLogger = value; } }

		public void LogError(string logMsg, Exception err, string extraXmlData) {
			tSource.TraceEvent(TraceEventType.Error, 0, LOG_ERR_FMT, logMsg, err.GetType().Name, err.Message, err.StackTrace, extraXmlData);
		}
	}

}
