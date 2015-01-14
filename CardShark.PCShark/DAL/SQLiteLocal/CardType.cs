using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeJSON;

namespace CardShark.PCShark.DAL.SQLiteLocal
{
	public class CardType
	{
		public List<string> SuperTypes { get; private set; }
		public List<string> MainTypes { get; private set; }
		public List<string> SubTypes { get; private set; }
		public List<string> Classes { get; private set; }

		public CardType() {
			SuperTypes = new List<string>();
			MainTypes = new List<string>();
			SubTypes = new List<string>();
			Classes = new List<string>();
		}

		public static CardType FromJson(string json) {
			JSONParser parser = new JSONParser();
			JSONNode typeNode = parser.Parse(json);
			return FromJsonObject(typeNode);
		}
		public string ToJson() {
			return this.ToJsonObject().ToString();
		}

		public static CardType FromJsonObject(JSONNode typeNode) {
			CardType _this = new CardType();
			foreach (JSONNode type in typeNode.ObjectTypeMembers["SuperTypes"].ArrayTypeMembers)
				_this.SuperTypes.Add(type.StringTypeValue);
			foreach (JSONNode type in typeNode.ObjectTypeMembers["MainTypes"].ArrayTypeMembers)
				_this.MainTypes.Add(type.StringTypeValue);
			foreach (JSONNode type in typeNode.ObjectTypeMembers["SubTypes"].ArrayTypeMembers)
				_this.SubTypes.Add(type.StringTypeValue);
			foreach (JSONNode type in typeNode.ObjectTypeMembers["Classes"].ArrayTypeMembers)
				_this.Classes.Add(type.StringTypeValue);
			return _this;
		}
		public JSONNode ToJsonObject() {
			JSONNode typeNode = new JSONNode(new KeyValuePair<string, JSONNode>[] {
				new KeyValuePair<string, JSONNode>("SuperTypes", new JSONNode(new List<JSONNode>())),
				new KeyValuePair<string, JSONNode>("MainTypes", new JSONNode(new List<JSONNode>())),
				new KeyValuePair<string, JSONNode>("SubTypes", new JSONNode(new List<JSONNode>())),
				new KeyValuePair<string, JSONNode>("Classes", new JSONNode(new List<JSONNode>()))
			});
			foreach (string type in SuperTypes)
				typeNode.ObjectTypeMembers["SuperTypes"].ArrayTypeMembers.Add(new JSONNode(type));
			foreach (string type in MainTypes)
				typeNode.ObjectTypeMembers["MainTypes"].ArrayTypeMembers.Add(new JSONNode(type));
			foreach (string type in SubTypes)
				typeNode.ObjectTypeMembers["SubTypes"].ArrayTypeMembers.Add(new JSONNode(type));
			foreach (string type in Classes)
				typeNode.ObjectTypeMembers["Classes"].ArrayTypeMembers.Add(new JSONNode(type));
			return typeNode;
		}

		public override string ToString() {
			StringBuilder sb = new StringBuilder();
			bool not1st = false;
			foreach (string type in SuperTypes) {
				if (not1st) sb.Append(' ');
				else not1st = true;
				sb.Append(type);
			}
			foreach (string type in MainTypes) {
				if (not1st) sb.Append(' ');
				else not1st = true;
				sb.Append(type);
			}
			if (not1st && SubTypes.Count > 0 && Classes.Count > 0)
				sb.Append(" — ");
			foreach (string type in SubTypes) {
				if (not1st) sb.Append(' ');
				else not1st = true;
				sb.Append(type);
			}
			foreach (string type in Classes) {
				if (not1st) sb.Append(' ');
				else not1st = true;
				sb.Append(type);
			}
			return sb.ToString();
		}
		public static CardType FromString(string typeText) {
			CardType _this = new CardType();
			string[] pts1 = typeText.Split(new char[] { '—' }, StringSplitOptions.RemoveEmptyEntries);
			if (pts1.Length > 0) {
				string[] pts1_0 = pts1[0].Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
				if (pts1_0.Length > 1) {
					for (int i = 0; i < pts1_0.Length-1; i++)
						_this.SuperTypes.Add(pts1_0[i].Trim());
				}
				if (pts1_0.Length > 0)
					_this.MainTypes.Add(pts1_0[pts1_0.Length - 1].Trim());
			}
			if (pts1.Length > 1) {
				string[] pts1_1 = pts1[1].Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
				if (pts1_1.Length > 0) {
					_this.SubTypes.Add(pts1_1[0].Trim());
					for (int i = 1; i < pts1_1.Length; i++)
						_this.Classes.Add(pts1_1[i].Trim());
				}
			}
			return _this;
		}
	}
}
