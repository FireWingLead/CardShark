using LocalSet = CardShark.PCShark.DAL.SQLiteLocal.Set;
using LocalCard = CardShark.PCShark.DAL.SQLiteLocal.Card;
using LocalCardVariation = CardShark.PCShark.DAL.SQLiteLocal.CardVariation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeJSON;

namespace CardShark.PCShark.DAL.MTGAPI_Remote
{
	public class SetAdapter
	{
		public const string CONTROLLER_NAME = "sets";
		public static readonly Type LOCAL_MODEL_TYPE = typeof(LocalSet);

		public SetAdapter() : this(URI_Manager.DefaultManager) { }
		public SetAdapter(URI_Manager uriManager) {
			URI_Manager = uriManager == null ? URI_Manager.DefaultManager : uriManager;
		}

		public URI_Manager URI_Manager { get; set; }

		public BindingList<LocalSet> GetSets(bool getSymbolImages) {
			JSONNode docRoot = RequestManager.GetRequest(URI_Manager.URIForModel(CONTROLLER_NAME));
			List<JSONNode> setNodes = docRoot.ObjectTypeMembers["sets"].ArrayTypeMembers;
			BindingList<LocalSet> sets = new BindingList<LocalSet>();
			foreach (JSONNode setNode in setNodes) {
				LocalSet set = new LocalSet() {
					CardVariations = null,
					Code = setNode.ObjectTypeMembers["code"].StringTypeValue,
					Description = null,
					Name = setNode.ObjectTypeMembers["name"].StringTypeValue
				};
				Dictionary<string, JSONNode> imgUrlNodes = setNode.ObjectTypeMembers["symbolImages"].ObjectTypeMembers;
				set.CommonSymbolURI = imgUrlNodes["common"].StringTypeValue;
				set.UncommonSymbolURI = imgUrlNodes["uncommon"].StringTypeValue;
				set.RareSymbolURI = imgUrlNodes["rare"].StringTypeValue;
				set.MythicSymbolURI = imgUrlNodes["mythic"].StringTypeValue;
				set.SpecialSymbolURI = imgUrlNodes["special"].StringTypeValue;

				if (getSymbolImages)
					GetImageDataForSet(set);

				sets.Add(set);
			}
			return sets;
		}

		public void GetImageDataForSet(LocalSet set) {
			if (set == null) return;
			if (set.CommonSymbolImage == null && !string.IsNullOrWhiteSpace(set.CommonSymbolURI))
				set.CommonSymbolImage = RequestManager.GetImage(set.CommonSymbolURI);
			if (set.UncommonSymbolImage == null && !string.IsNullOrWhiteSpace(set.UncommonSymbolURI))
				set.UncommonSymbolImage = RequestManager.GetImage(set.UncommonSymbolURI);
			if (set.RareSymbolImage == null && !string.IsNullOrWhiteSpace(set.RareSymbolURI))
				set.RareSymbolImage = RequestManager.GetImage(set.RareSymbolURI);
			if (set.MythicSymbolImage == null && !string.IsNullOrWhiteSpace(set.MythicSymbolURI))
				set.MythicSymbolImage = RequestManager.GetImage(set.MythicSymbolURI);
			if (set.SpecialSymbolImage == null && !string.IsNullOrWhiteSpace(set.SpecialSymbolURI))
				set.SpecialSymbolImage = RequestManager.GetImage(set.SpecialSymbolURI);
		}

		public void RefreshSet(LocalSet set) {
			JSONNode docRoot = RequestManager.GetRequest(URI_Manager.QueriedURIForModel(CONTROLLER_NAME, set, SetPropertyBinding.Code));
			List<JSONNode> setNodes = docRoot.ObjectTypeMembers["sets"].ArrayTypeMembers;
			if (setNodes.Count == 0)
				throw new ItemNotFoundException("Set", new KeyValuePair<string, string>(SetPropertyBinding.Code.LocalProperty.Name, (string)SetPropertyBinding.Code.LocalProperty.GetMethod.Invoke(set, null)));
			
			JSONNode setNode = setNodes[0];

			set.Name = setNode.ObjectTypeMembers["name"].StringTypeValue;

			Dictionary<string, JSONNode> imgUrlNodes = setNode.ObjectTypeMembers["symbolImages"].ObjectTypeMembers;
			set.CommonSymbolURI = imgUrlNodes["common"].StringTypeValue;
			set.UncommonSymbolURI = imgUrlNodes["uncommon"].StringTypeValue;
			set.RareSymbolURI = imgUrlNodes["rare"].StringTypeValue;
			set.MythicSymbolURI = imgUrlNodes["mythic"].StringTypeValue;
			set.SpecialSymbolURI = imgUrlNodes["special"].StringTypeValue;

			GetImageDataForSet(set);
		}
	}
}
