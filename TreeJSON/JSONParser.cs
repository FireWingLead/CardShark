using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TreeJSON
{
	public class JSONParser
	{
		private long cLine = 1, cChar = 1;
		public JSONParser() { }

		internal void Reset() {
			cLine = 1;
			cChar = 1;
		}

		public JSONNode Parse(string jsonStr) {
			if (jsonStr == null) return new JSONNode();
			StringReader reader = new StringReader(jsonStr);
			return Parse(reader);
		}
		public JSONNode Parse(TextReader reader) {
			Reset();
			ReadOffWhiteSpace(reader);
			int peekVal = reader.Peek();
			if (peekVal == -1)
				return new JSONNode();
			if ((char)peekVal == '[')
				return ReadArray(reader);
			if ((char)peekVal == '{')
				return ReadObject(reader);
			throw new JSONSyntaxException("root array or object, or null (empty) document", "character", ((char)peekVal).ToString(), cLine, cChar);
		}

		private char ReadChar(TextReader reader) {
			char chr = (char)reader.Read();
			if (chr == '\n') {
				cChar = 1;
				cLine++;
			}
			else
				cChar++;
			return chr;
		}
		private void ReadOffWhiteSpace(TextReader reader) {
			while (reader.Peek() != -1 && char.IsWhiteSpace((char)reader.Peek()))
				ReadChar(reader);
		}
		internal string ReadWord(TextReader reader) {
			StringBuilder wrd = new StringBuilder();
			if (reader.Peek() == -1)
				throw new JSONSyntaxException("identifier", "end of file", null, cLine, cChar);
			if (!(char.IsLetter((char)reader.Peek()) || (char)reader.Peek() == '_'))
				throw new JSONSyntaxException("identifier", "character", ((char)reader.Peek()).ToString(), cLine, cChar);
			for (char chr = (char)reader.Peek(); char.IsLetterOrDigit(chr) || chr == '_'; chr = (char)reader.Peek()) {
				wrd.Append(ReadChar(reader));
				if (reader.Peek() == -1)
					break;
			}
			return wrd.ToString();
		}
		private string ReadOperator(TextReader reader) {
			StringBuilder opr = new StringBuilder();
			if (reader.Peek() == -1)
				throw new JSONSyntaxException("operator", "end of file", null, cLine, cChar);
			if (!char.IsSymbol((char)reader.Peek()))
				throw new JSONSyntaxException("operator", "character", ((char)reader.Peek()).ToString(), cLine, cChar);
			for (char chr = (char)reader.Peek(); char.IsSymbol(chr); chr = (char)reader.Peek()) {
				opr.Append(ReadChar(reader));
				if (reader.Peek() == -1)
					break;
			}
			return opr.ToString();
		}
		internal string ReadString(TextReader reader) {
			StringBuilder str = new StringBuilder();
			if (reader.Peek() == -1)
				throw new JSONSyntaxException("start of string literal", "end of file", null, cLine, cChar);
			char startChar = (char)reader.Peek();
			if (!(startChar == '"' || startChar == '\''))
				throw new JSONSyntaxException("start of string literal", "character", startChar.ToString(), cLine, cChar);
			ReadChar(reader);
			if (reader.Peek() == -1)
				throw new JSONSyntaxException("character or end of string literal", "end of file", null, cLine, cChar);
			long slashCnt=0;
			for (char chr = ReadChar(reader); !(chr == startChar && slashCnt % 2 == 0); chr = ReadChar(reader)) {
				str.Append(chr);
				if (chr == '\\')
					slashCnt++;
				else
					slashCnt = 0;
				if (reader.Peek() == -1)
					throw new JSONSyntaxException("character or end of string literal", "end of file", null, cLine, cChar);
			}
			return UnescapeString(str.ToString());
		}
		private string ReadDigits(TextReader reader) {
			StringBuilder strVal = new StringBuilder();
			int peekVal = reader.Peek();
			if (reader.Peek() == -1)
				throw new JSONSyntaxException("digit", "end of file", null, cLine, cChar);
			if (!char.IsDigit((char)peekVal))
				throw new JSONSyntaxException("digit", "character", ((char)peekVal).ToString(), cLine, cChar);
			for (char chr = (char)peekVal; char.IsDigit(chr); chr = (char)reader.Peek()) {
				strVal.Append(ReadChar(reader));
				if (reader.Peek() == -1)
					break;
			}
			return strVal.ToString();
		}
		private JSONNode ReadNumber(TextReader reader) {
			StringBuilder strVal = new StringBuilder();
			string stringVal = null;
			int peekVal = reader.Peek();
			if (peekVal == -1)
				throw new JSONSyntaxException("digit or '-'", "end of file", null, cLine, cChar);
			bool isNeg = (char)peekVal == '-';
			if (isNeg) {
				strVal.Append(ReadChar(reader));
				peekVal = reader.Peek();
			}
			strVal.Append(ReadDigits(reader));

			if ((char)reader.Peek() == '.') {//Read, parse, and return as floating point number
				strVal.Append(ReadChar(reader));
				strVal.Append(ReadDigits(reader));
				peekVal = reader.Peek();
				if ((char)peekVal == 'e' || (char)peekVal == 'E') {
					strVal.Append(ReadChar(reader));
					peekVal = reader.Peek();
					if ((char)peekVal == '+' || (char)peekVal == '-')
						strVal.Append(ReadChar(reader));
					strVal.Append(ReadDigits(reader));
				}
				stringVal = strVal.ToString();
				try { return new JSONNode(decimal.Parse(stringVal)); }
				catch (System.OverflowException) { /*Ignore and move on to next attempt with the bigger float.*/}
				try { return new JSONNode(float.Parse(stringVal)); }
				catch (System.OverflowException) { /*Ignore and move on to next attempt with the bigger double.*/}
				return new JSONNode(double.Parse(stringVal));
			}
			else if (((char)reader.Peek() == 'x' || (char)reader.Peek() == 'X') && ((strVal.Length == 1 && strVal[0] == '0') || (strVal.Length == 2 && strVal[0] == '-' && strVal[1] == '0'))) {
				//Read, parse, and return as Hex Integer
				strVal.Append(ReadChar(reader));
				peekVal = reader.Peek();
				if (reader.Peek() == -1)
					throw new JSONSyntaxException("hex digit", "end of file", null, cLine, cChar);
				char peekChr = char.ToLower((char)peekVal);
				if (!(char.IsDigit(peekChr) || (peekChr >= 'a' && peekChr < 'g')))
					throw new JSONSyntaxException("hex digit", "character", ((char)peekVal).ToString(), cLine, cChar);
				for (char chr = peekChr; char.IsDigit(chr) || (chr >= 'a' && chr < 'g'); chr = char.ToLower((char)reader.Peek())) {
					strVal.Append(ReadChar(reader));
					if (reader.Peek() == -1)
						break;
				}
				stringVal = strVal.ToString();
				try { return new JSONNode(short.Parse(stringVal, System.Globalization.NumberStyles.AllowLeadingSign | System.Globalization.NumberStyles.HexNumber)); }
				catch (System.OverflowException) { /*Ignore and move on to next attempt with the bigger int.*/}
				try { return new JSONNode(int.Parse(stringVal, System.Globalization.NumberStyles.AllowLeadingSign | System.Globalization.NumberStyles.HexNumber)); }
				catch (System.OverflowException) { /*Ignore and move on to next attempt with the bigger long.*/}
				return new JSONNode(long.Parse(stringVal, System.Globalization.NumberStyles.HexNumber | System.Globalization.NumberStyles.AllowLeadingSign));
			}
			//Default, parse as base 10 integer.
			stringVal = strVal.ToString();
			try { return new JSONNode(short.Parse(stringVal)); }
			catch (System.OverflowException) { /*Ignore and move on to next attempt with the bigger int.*/}
			try { return new JSONNode(int.Parse(stringVal)); }
			catch (System.OverflowException) { /*Ignore and move on to next attempt with the bigger long.*/}
			return new JSONNode(long.Parse(stringVal));
		}
		private JSONNode ReadObject(TextReader reader) {
			JSONNode objNode = new JSONNode() { ObjectTypeMembers = new Dictionary<string, JSONNode>() };
			int peekVal = reader.Peek();
			if (peekVal == -1)
				throw new JSONSyntaxException("'{'", "end of file", null, cLine, cChar);
			char peekChr = (char)peekVal;
			if (peekChr != '{')
				throw new JSONSyntaxException("'{'", "character", peekChr.ToString(), cLine, cChar);
			ReadChar(reader);
			ReadOffWhiteSpace(reader);
			peekVal = reader.Peek();
			if (peekVal == -1)
				throw new JSONSyntaxException("identifier or '}'", "end of file", null, cLine, cChar);
			peekChr = (char)peekVal;
			if (peekChr != '}') {
				while (true) {
					string memberName = null;
					if (peekChr == '"' || peekChr == '\'')
						memberName = ReadString(reader);
					else if (char.IsLetter(peekChr) || peekChr == '_')
						memberName = ReadWord(reader);
					else
						throw new JSONSyntaxException("identifier or '}'", "character", peekChr.ToString(), cLine, cChar);
					ReadOffWhiteSpace(reader);

					peekVal = reader.Peek();
					if (peekVal == -1)
						throw new JSONSyntaxException("':'", "end of file", null, cLine, cChar);
					peekChr = (char)peekVal;
					if (peekChr != ':')
						throw new JSONSyntaxException("':'", "character", peekChr.ToString(), cLine, cChar);
					ReadChar(reader);
					ReadOffWhiteSpace(reader);

					objNode.ObjectTypeMembers.Add(memberName, ReadNode(reader));
					ReadOffWhiteSpace(reader);

					peekVal = reader.Peek();
					if (peekVal == -1)
						throw new JSONSyntaxException("',' or '}'", "end of file", null, cLine, cChar);
					peekChr = (char)peekVal;
					if (peekChr == ',') {
						ReadChar(reader);
						ReadOffWhiteSpace(reader);
						peekVal = reader.Peek();
						if (peekVal == -1)
							throw new JSONSyntaxException("identifier or '}'", "end of file", null, cLine, cChar);
						peekChr = (char)peekVal;
					}
					else if (peekChr == '}')
						break;
					else
						throw new JSONSyntaxException("',' or '}'", "character", peekChr.ToString(), cLine, cChar);
				}
			}
			ReadChar(reader);
			return objNode;
		}
		private JSONNode ReadArray(TextReader reader) {
			JSONNode arrNode = new JSONNode() { ArrayTypeMembers = new List<JSONNode>() };
			int peekVal = reader.Peek();
			if (peekVal == -1)
				throw new JSONSyntaxException("'['", "end of file", null, cLine, cChar);
			char peekChr = (char)peekVal;
			if (peekChr != '[')
				throw new JSONSyntaxException("'['", "character", peekChr.ToString(), cLine, cChar);
			ReadChar(reader);
			ReadOffWhiteSpace(reader);
			peekVal = reader.Peek();
			if (peekVal == -1)
				throw new JSONSyntaxException("array member value or ']'", "end of file", null, cLine, cChar);
			peekChr = (char)peekVal;
			if (peekChr != ']') {
				while (true) {
					arrNode.ArrayTypeMembers.Add(ReadNode(reader));
					ReadOffWhiteSpace(reader);

					peekVal = reader.Peek();
					if (peekVal == -1)
						throw new JSONSyntaxException("',' or ']'", "end of file", null, cLine, cChar);
					peekChr = (char)peekVal;
					if (peekChr == ',') {
						ReadChar(reader);
						ReadOffWhiteSpace(reader);
						peekVal = reader.Peek();
						if (peekVal == -1)
							throw new JSONSyntaxException("array member value or ']'", "end of file", null, cLine, cChar);
						peekChr = (char)peekVal;
					}
					else if (peekChr == ']')
						break;
					else
						throw new JSONSyntaxException("',' or ']'", "character", peekChr.ToString(), cLine, cChar);
				}
			}
			ReadChar(reader);
			return arrNode;
		}
		private JSONNode ReadNode(TextReader reader) {
			int peekVal = reader.Peek();
			if (peekVal == -1)
				throw new JSONSyntaxException("string, number, boolean, null, array, or object", "end of file", null, cLine, cChar);
			char peekChr = (char)peekVal;
			switch (peekChr) {
				case '[': return ReadArray(reader);
				case '{': return ReadObject(reader);
				case '"': return new JSONNode(ReadString(reader));
				case '\'': return new JSONNode(ReadString(reader));
				case '-': return ReadNumber(reader);
				default:
					if (char.IsDigit(peekChr)) return ReadNumber(reader);
					if (char.IsLetter(peekChr)) {
						string keywrd = ReadWord(reader).ToLower();
						switch (keywrd) {
							case "null": return new JSONNode();
							case "false": return new JSONNode(false);
							case "true": return new JSONNode(true);
						}
					}
					break;
			}
			throw new JSONSyntaxException("string, number, boolean, null, array, or object", "character", peekChr.ToString(), cLine, cChar);
		}

		public static string UnescapeString(string str) {
			if (str == null) return null;
			return Regex.Replace(str, "(\\\\[^u])|(\\\\u[0-9a-fA-F]{4})", match => {
				switch (match.Value[1]) {
					case 'n': return "\n";
					case 'r': return "\r";
					case 't': return "\t";
					case 'b': return "\b";
					case 'f': return "\f";
					case 'u': return ((char)int.Parse(match.Value.Substring(2), System.Globalization.NumberStyles.HexNumber)).ToString();
					default: return match.Value[1].ToString();
				}
			}, RegexOptions.Singleline);
		}
	}
}
