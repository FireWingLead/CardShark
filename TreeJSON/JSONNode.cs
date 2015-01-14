using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TreeJSON
{
    public class JSONNode
    {
		public JSONNode() { }
		public JSONNode(IEnumerable<JSONNode> arrayMembers) {
			if (arrayMembers == null) return;
			value = new List<JSONNode>(arrayMembers);
			NodeType = NodeTypes.Array;
		}
		public JSONNode(IEnumerable<KeyValuePair<string, JSONNode>> objectMembers) {
			if (objectMembers == null) return;
			value = new Dictionary<string, JSONNode>();
			foreach (KeyValuePair<string, JSONNode> kvp in objectMembers)
				((Dictionary<string, JSONNode>)value).Add(kvp.Key, kvp.Value);
			NodeType = NodeTypes.Object;
		}
		public JSONNode(string value) { StringTypeValue = value; }
		public JSONNode(short value) { Int16TypeValue = value; }
		public JSONNode(int value) { Int32TypeValue = value; }
		public JSONNode(long value) { Int64TypeValue = value; }
		public JSONNode(decimal value) { DecimalTypeValue = value; }
		public JSONNode(float value) { FloatTypeValue = value; }
		public JSONNode(double value) { DoubleTypeValue = value; }
		public JSONNode(bool value) { BoolTypeValue = value; }
		public JSONNode(short? value) { Int16OrNullTypeValue = value; }
		public JSONNode(int? value) { Int32OrNullTypeValue = value; }
		public JSONNode(long? value) { Int64OrNullTypeValue = value; }
		public JSONNode(decimal? value) { DecimalOrNullTypeValue = value; }
		public JSONNode(float? value) { FloatOrNullTypeValue = value; }
		public JSONNode(double? value) { DoubleOrNullTypeValue = value; }
		public JSONNode(bool? value) { BoolOrNullTypeValue = value; }

		private object value = null;
		public NodeTypes NodeType { get; private set; }

		public void SetToNull() { value = null; NodeType = NodeTypes.Null; }
		public Dictionary<string, JSONNode> ObjectTypeMembers {
			get {
				if (NodeType != NodeTypes.Object)
					throw new IncompatibleNodeTypeException(NodeTypes.Object, NodeType, value);
				return (Dictionary<string, JSONNode>)value;
			}
			set {
				if (value == null) {
					SetToNull();
					return;
				}
				this.value = value;
				NodeType = NodeTypes.Object;
			}
		}
		public List<JSONNode> ArrayTypeMembers {
			get {
				if (NodeType != NodeTypes.Array)
					throw new IncompatibleNodeTypeException(NodeTypes.Object, NodeType, value);
				return (List<JSONNode>)value;
			}
			set {
				if (value == null) {
					SetToNull();
					return;
				}
				this.value = value;
				NodeType = NodeTypes.Array;
			}
		}
		public string StringTypeValue {
			get {
				if (value == null) return null;
				if (value is string) return (string)value;
				return value.ToString();
			}
			set {
				if (value == null) SetToNull();
				this.value = value;
				NodeType = NodeTypes.String;
			}
		}
		public string StrictStringTypeValue {
			get {
				if (value == null) return null;
				if (value is string) return (string)value;
				throw new IncompatibleNodeTypeException(NodeTypes.String, NodeType, value);
			}
			set {
				if (value == null) SetToNull();
				this.value = value;
				NodeType = NodeTypes.String;
			}
		}
		public short Int16TypeValue {
			get {
				if (value is short) return (short)value;
				if (value is int || value is long) {
					long val = Convert.ToInt64(value);
					if (val > short.MaxValue)
						throw new NodeValueOutOfBoundsException(typeof(short), short.MaxValue, value, true);
					if (val < short.MinValue)
						throw new NodeValueOutOfBoundsException(typeof(short), short.MinValue, value, false);
					return (short)(int)val;
				}
				if (value is string) {
					long val = 0;
					if (!long.TryParse((string)value, out val))
						throw new IncompatibleNodeTypeException(NodeTypes.Integer, NodeType, value);
					if (val > short.MaxValue)
						throw new NodeValueOutOfBoundsException(typeof(string), short.MaxValue, value, true);
					if (val < short.MinValue)
						throw new NodeValueOutOfBoundsException(typeof(string), short.MinValue, value, false);
					return (short)(int)val;
				}
				throw new IncompatibleNodeTypeException(NodeTypes.Integer, NodeType, value);
			}
			set { this.value = value; NodeType = NodeTypes.Integer; }
		}
		public short? Int16OrNullTypeValue {
			get {
				if (value is short) return (short)value;
				if (value == null) return null;
				if (value is int || value is long) {
					long val = Convert.ToInt64(value);
					if (val > short.MaxValue)
						throw new NodeValueOutOfBoundsException(typeof(short), short.MaxValue, value, true);
					if (val < short.MinValue)
						throw new NodeValueOutOfBoundsException(typeof(short), short.MinValue, value, false);
					return (short)(int)val;
				}
				if (value is string) {
					long val = 0;
					if (!long.TryParse((string)value, out val))
						throw new IncompatibleNodeTypeException(NodeTypes.Integer, NodeType, value);
					if (val > short.MaxValue)
						throw new NodeValueOutOfBoundsException(typeof(string), short.MaxValue, value, true);
					if (val < short.MinValue)
						throw new NodeValueOutOfBoundsException(typeof(string), short.MinValue, value, false);
					return (short)(int)val;
				}
				throw new IncompatibleNodeTypeException(NodeTypes.Integer, NodeType, value);
			}
			set { if (value.HasValue) { this.value = value.Value; NodeType = NodeTypes.Integer; return; } SetToNull(); }
		}
		public int Int32TypeValue {
			get {
				if (value is int || value is short) return (int)value;
				if (value is long) {
					if ((long)value > int.MaxValue)
						throw new NodeValueOutOfBoundsException(typeof(int), int.MaxValue, value, true);
					if ((long)value < int.MinValue)
						throw new NodeValueOutOfBoundsException(typeof(int), int.MinValue, value, false);
					return (int)value;
				}
				if (value is string) {
					long val = 0;
					if (!long.TryParse((string)value, out val))
						throw new IncompatibleNodeTypeException(NodeTypes.Integer, NodeType, value);
					if (val > int.MaxValue)
						throw new NodeValueOutOfBoundsException(typeof(int), int.MaxValue, value, true);
					if (val < int.MinValue)
						throw new NodeValueOutOfBoundsException(typeof(int), int.MinValue, value, false);
					return (int)val;
				}
				throw new IncompatibleNodeTypeException(NodeTypes.Integer, NodeType, value);
			}
			set { this.value = value; NodeType = NodeTypes.Integer; }
		}
		public int? Int32OrNullTypeValue {
			get {
				if (value is int || value is short) return (int)value;
				if (value == null) return null;
				if (value is long) {
					if ((long)value > int.MaxValue)
						throw new NodeValueOutOfBoundsException(typeof(int), int.MaxValue, value, true);
					if ((long)value < int.MinValue)
						throw new NodeValueOutOfBoundsException(typeof(int), int.MinValue, value, false);
					return (int)value;
				}
				if (value is string) {
					long val = 0;
					if (!long.TryParse((string)value, out val))
						throw new IncompatibleNodeTypeException(NodeTypes.Integer, NodeType, value);
					if (val > int.MaxValue)
						throw new NodeValueOutOfBoundsException(typeof(int), int.MaxValue, value, true);
					if (val < int.MinValue)
						throw new NodeValueOutOfBoundsException(typeof(int), int.MinValue, value, false);
					return (int)val;
				}
				throw new IncompatibleNodeTypeException(NodeTypes.Integer, NodeType, value);
			}
			set { if (value.HasValue) { this.value = value.Value; NodeType = NodeTypes.Integer; return; } SetToNull(); }
		}
		public long Int64TypeValue {
			get {
				if (value is long || value is int) return (long)value;
				if (value is short) return (long)(int)value;
				if (value is string) {
					long val = 0;
					if (long.TryParse((string)value, out val))
						return val;
				}
				throw new IncompatibleNodeTypeException(NodeTypes.Integer, NodeType, value);
			}
			set { this.value = value; NodeType = NodeTypes.Integer; }
		}
		public long? Int64OrNullTypeValue {
			get {
				if (value is long || value is int) return (long)value;
				if (value == null) return null;
				if (value is short) return (long)(int)value;
				if (value is string) {
					long val = 0;
					if (long.TryParse((string)value, out val))
						return val;
				}
				throw new IncompatibleNodeTypeException(NodeTypes.Integer, NodeType, value);
			}
			set { if (value.HasValue) { this.value = value.Value; NodeType = NodeTypes.Integer; return; } SetToNull(); }
		}
		public decimal DecimalTypeValue {
			get {
				if (value is decimal || value is long || value is int || value is short) return Convert.ToDecimal(value);
				if (value is double || value is float) {
					double val = (double)value;
					if (val > (double)decimal.MaxValue)
						throw new NodeValueOutOfBoundsException(typeof(decimal), decimal.MaxValue, value, true);
					if (val < (double)decimal.MinValue)
						throw new NodeValueOutOfBoundsException(typeof(decimal), decimal.MinValue, value, false);
					return (decimal)val;
				}
				if (value is string) {
					double val = 0.0;
					if (!double.TryParse((string)value, out val))
						throw new IncompatibleNodeTypeException(NodeTypes.Decimal, NodeType, value);
					if (val > (double)decimal.MaxValue)
						throw new NodeValueOutOfBoundsException(typeof(decimal), decimal.MaxValue, value, true);
					if (val < (double)decimal.MinValue)
						throw new NodeValueOutOfBoundsException(typeof(decimal), decimal.MinValue, value, false);
					return (decimal)val;
				}
				throw new IncompatibleNodeTypeException(NodeTypes.Decimal, NodeType, value);
			}
			set { this.value = value; NodeType = NodeTypes.Decimal; }
		}
		public decimal? DecimalOrNullTypeValue {
			get {
				if (value is decimal || value is long || value is int || value is short) return Convert.ToDecimal(value);
				if (value == null) return null;
				if (value is double || value is float) {
					double val = (double)value;
					if (val > (double)decimal.MaxValue)
						throw new NodeValueOutOfBoundsException(typeof(decimal), decimal.MaxValue, value, true);
					if (val < (double)decimal.MinValue)
						throw new NodeValueOutOfBoundsException(typeof(decimal), decimal.MinValue, value, false);
					return (decimal)val;
				}
				if (value is string) {
					double val = 0.0;
					if (!double.TryParse((string)value, out val))
						throw new IncompatibleNodeTypeException(NodeTypes.Decimal, NodeType, value);
					if (val > (double)decimal.MaxValue)
						throw new NodeValueOutOfBoundsException(typeof(decimal), decimal.MaxValue, value, true);
					if (val < (double)decimal.MinValue)
						throw new NodeValueOutOfBoundsException(typeof(decimal), decimal.MinValue, value, false);
					return (decimal)val;
				}
				throw new IncompatibleNodeTypeException(NodeTypes.Decimal, NodeType, value);
			}
			set { if (value.HasValue) { this.value = value.Value; NodeType = NodeTypes.Decimal; return; } SetToNull(); }
		}
		public float FloatTypeValue {
			get {
				if (value is float || value is decimal || value is long || value is int || value is short) return Convert.ToSingle(value);
				if (value is double) {
					if ((double)value > float.MaxValue)
						throw new NodeValueOutOfBoundsException(typeof(float), float.MaxValue, value, true);
					if ((double)value < float.MinValue)
						throw new NodeValueOutOfBoundsException(typeof(float), float.MinValue, value, false);
					return (float)value;
				}
				if (value is string) {
					double val = 0.0;
					if (!double.TryParse((string)value, out val))
						throw new IncompatibleNodeTypeException(NodeTypes.Decimal, NodeType, value);
					if (val > float.MaxValue)
						throw new NodeValueOutOfBoundsException(typeof(float), float.MaxValue, value, true);
					if (val < float.MinValue)
						throw new NodeValueOutOfBoundsException(typeof(float), float.MinValue, value, false);
					return (float)val;
				}
				throw new IncompatibleNodeTypeException(NodeTypes.Decimal, NodeType, value);
			}
			set { this.value = value; NodeType = NodeTypes.Decimal; }
		}
		public float? FloatOrNullTypeValue {
			get {
				if (value is float || value is decimal || value is long || value is int || value is short) return Convert.ToSingle(value);
				if (value == null) return null;
				if (value is double) {
					if ((double)value > float.MaxValue)
						throw new NodeValueOutOfBoundsException(typeof(float), float.MaxValue, value, true);
					if ((double)value < float.MinValue)
						throw new NodeValueOutOfBoundsException(typeof(float), float.MinValue, value, false);
					return (float)value;
				}
				if (value is string) {
					double val = 0.0;
					if (!double.TryParse((string)value, out val))
						throw new IncompatibleNodeTypeException(NodeTypes.Decimal, NodeType, value);
					if (val > float.MaxValue)
						throw new NodeValueOutOfBoundsException(typeof(float), float.MaxValue, value, true);
					if (val < float.MinValue)
						throw new NodeValueOutOfBoundsException(typeof(float), float.MinValue, value, false);
					return (float)val;
				}
				throw new IncompatibleNodeTypeException(NodeTypes.Decimal, NodeType, value);
			}
			set { if (value.HasValue) { this.value = value.Value; NodeType = NodeTypes.Decimal; return; } SetToNull(); }
		}
		public double DoubleTypeValue {
			get {
				if (value is double || value is float || value is decimal || value is long || value is int || value is short) return Convert.ToDouble(value);
				if (value is string) {
					double val = 0.0;
					if (double.TryParse((string)value, out val))
						return val;
				}
				throw new IncompatibleNodeTypeException(NodeTypes.Decimal, NodeType, value);
			}
			set { this.value = value; NodeType = NodeTypes.Decimal; }
		}
		public double? DoubleOrNullTypeValue {
			get {
				if (value is double || value is float || value is decimal || value is long || value is int || value is short) return Convert.ToDouble(value);
				if (value == null) return null;
				if (value is string) {
					double val = 0.0;
					if (double.TryParse((string)value, out val))
						return val;
				}
				throw new IncompatibleNodeTypeException(NodeTypes.Decimal, NodeType, value);
			}
			set { if (value.HasValue) { this.value = value.Value; NodeType = NodeTypes.Decimal; return; } SetToNull(); }
		}
		public bool BoolTypeValue {
			get {
				if (value is bool) return (bool)value;
				if (value is short || value is int || value is long) return Convert.ToInt64(value) != 0;
				if (value == null) return false;
				throw new IncompatibleNodeTypeException(NodeTypes.Boolean, NodeType, value);
			}
			set { this.value = value; NodeType = NodeTypes.Boolean; }
		}
		public bool? BoolOrNullTypeValue {
			get {
				if (value is bool) return (bool)value;
				if (value is short || value is int || value is long) return Convert.ToInt64(value) != 0;
				if (value == null) return null;
				throw new IncompatibleNodeTypeException(NodeTypes.Boolean, NodeType, value);
			}
			set { if (value.HasValue) { this.value = value.Value; NodeType = NodeTypes.Boolean; return; } SetToNull(); }
		}

		public JSONNode GetNodeAtPath(string path) {
			path = path.Trim();
			if (path.Length == 0)//We are at the desired node
				return this;
			int cChar = 1;
			if (char.IsLetter(path[0]) || path[0] == '_' || path[0] == '"' || path[0] == '\'') {//prepend a '.' for proper obj path syntax
				path = "." + path;
				cChar--; //We are now starting at one char BEFORE the 1st char in the original path.
			}
			return GetChildAtPath(path, cChar);
		}
		private JSONNode GetChildAtPath(string path, int cChar) {
			if (path[0] == '[') {//Get Array child
				Match indexMatch = Regex.Match(path, "^\\[\\d+\\]");
				if (!indexMatch.Success)
					throw new JSONSyntaxException("array indexer", "malformed path", path, 1, cChar);
				int index = int.Parse(indexMatch.Value.Substring(1, indexMatch.Value.Length - 2));
				if (path.Length == indexMatch.Value.Length)//then the indicated child is the last child in the path
					return ArrayTypeMembers[index];
				cChar += indexMatch.Value.Length;
				return ArrayTypeMembers[index].GetChildAtPath(path.Substring(indexMatch.Value.Length), cChar);
			}
			//We got this far, so next child is specified as object child, not array child.
			if (path[0] == '.') {//Read it off in preparation for getting the child member name
				if (path.Length < 2)
					throw new JSONSyntaxException("object member identifier", "end of path", null, 1, cChar + 1);
				path = path.Substring(1);
				cChar++;
			}
			StringReader rdr = new StringReader(path);
			JSONParser psr = new JSONParser();
			string childName = null;
			if (char.IsLetter(path[0]) || path[0] == '_')
				childName = psr.ReadWord(rdr);
			else if (path[0] == '"' || path[0] == '\'')
				childName = psr.ReadString(rdr);
			else
				throw new JSONSyntaxException("array indexer or object member identifier", "invalid member name character", path[0].ToString(), 1, cChar);
			if (rdr.Peek() == -1)//then the indicated child is the last child in the path
				return ObjectTypeMembers[childName];
			cChar += childName.Length;
			return ObjectTypeMembers[childName].GetChildAtPath(path.Substring(childName.Length), cChar);
		}

		public override string ToString() {
			switch (NodeType) {
				case NodeTypes.Null: return "null";
				case NodeTypes.Boolean: return (bool)value ? "true" : "false";
				case NodeTypes.Decimal: return value.ToString();
				case NodeTypes.Integer: return value.ToString();
				case NodeTypes.String: return "\"" + EscapeString((string)value) + "\"";
				case NodeTypes.Array:
					StringBuilder str = new StringBuilder();
					str.Append('[');
					bool not1st = false;
					foreach (JSONNode node in ArrayTypeMembers) {
						if (not1st) str.Append(", ");
						else not1st = true;
						str.Append(node.ToString());
					}
					str.Append(']');
					return str.ToString();
				case NodeTypes.Object:
					StringBuilder _str = new StringBuilder();
					_str.Append('{');
					bool _not1st = false;
					foreach (KeyValuePair<string, JSONNode> member in ObjectTypeMembers) {
						if (_not1st) _str.Append(", ");
						else _not1st = true;
						_str.Append("\"" + EscapeString(member.Key) + "\":" + member.Value.ToString());
					}
					_str.Append('}');
					return _str.ToString();
			}
			return null;
		}
		public string ToStringPretty(int indent = 0, bool useTabsNotSpaces = false, bool pad1stLine = true, int spacesPerIndent = 4) {
			string padding = "";
			if (useTabsNotSpaces)
				padding = padding.PadLeft(indent, '\t');
			else
				padding = padding.PadLeft(indent * spacesPerIndent);
			switch (NodeType) {
				case NodeTypes.Null: return pad1stLine ? padding + "null" : "null";
				case NodeTypes.Boolean: return pad1stLine ? ((bool)value ? padding + "true" : padding + "false") : ((bool)value ? "true" : "false");
				case NodeTypes.Decimal: return pad1stLine ? padding + value.ToString() : value.ToString();
				case NodeTypes.Integer: return pad1stLine ? padding + value.ToString() : value.ToString();
				case NodeTypes.String: return pad1stLine ? padding + "\"" + EscapeString((string)value) + "\"" : "\"" + EscapeString((string)value) + "\"";
				case NodeTypes.Array:
					StringBuilder str = new StringBuilder();
					str.Append(pad1stLine ? padding + "[" : "[");
					bool multiline = ArrayTypeMembers.Count > 1 || (ArrayTypeMembers.Count == 1 && (ArrayTypeMembers[0].NodeType == NodeTypes.Object || ArrayTypeMembers[0].NodeType == NodeTypes.Array));
					if (multiline) str.Append('\n');
					bool not1st = false;
					foreach (JSONNode node in ArrayTypeMembers) {
						if (not1st) str.Append(",\n");
						else not1st = true;
						str.Append(node.ToStringPretty(indent + 1, useTabsNotSpaces, multiline, spacesPerIndent));
					}
					if (multiline) str.Append('\n');
					str.Append(multiline ? padding + "]" : "]");
					return str.ToString();
				case NodeTypes.Object:
					StringBuilder _str = new StringBuilder();
					_str.Append(pad1stLine ? padding + "{" : "{");
					bool _multiline = ObjectTypeMembers.Count > 1 || (ObjectTypeMembers.Count == 1 && (ObjectTypeMembers.Values.ElementAt(0).NodeType == NodeTypes.Object || ObjectTypeMembers.Values.ElementAt(0).NodeType == NodeTypes.Array));
					if (_multiline) _str.Append('\n');
					bool _not1st = false;
					foreach (KeyValuePair<string, JSONNode> member in ObjectTypeMembers) {
						if (_not1st) _str.Append(",\n");
						else _not1st = true;
						if (_multiline)
							_str.Append(padding + (useTabsNotSpaces ? "\t" : "".PadLeft(spacesPerIndent)) + "\"" + EscapeString(member.Key) + "\":" + member.Value.ToStringPretty(indent + 1, useTabsNotSpaces, false, spacesPerIndent));
						else
							_str.Append("\"" + EscapeString(member.Key) + "\":" + member.Value.ToString());
					}
					if (_multiline) _str.Append('\n');
					_str.Append(_multiline ? padding + "}" : "}");
					return _str.ToString();
			}
			return null;
		}
		private string EscapeString(string str) {
			if (str == null) return null;
			return Regex.Replace(str, "[\"\\\\/\\b\\f\\n\\r\\t]", match => {
				switch (match.Value) {
					case "\n": return "\\n";
					case "\r": return "\\r";
					case "\t": return "\\t";
					case "\b": return "\\b";
					case "\f": return "\\f";
					default: return "\\" + match.Value;
				}
			});
		}
    }
}
