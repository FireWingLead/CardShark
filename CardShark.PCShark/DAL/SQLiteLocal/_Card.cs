using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeJSON;

namespace CardShark.PCShark.DAL.SQLiteLocal
{
	#region EF 6.x Version
	//public partial class Card
	//{
	//	private string cardTypeText = string.Empty;
	//	public string CardTypeText {
	//		get { return cardTypeText; }
	//		private set {
	//			if (cardTypeText == value) return;
	//			cardTypeText = value;
	//			cardTypeData = SQLiteLocal.CardType.FromString(value);
	//			ReportPropertyChanging("CardType");
	//			_CardType = cardTypeData.ToJson();
	//			ReportPropertyChanged("CardType");
	//		}
	//	}

	//	CardType cardTypeData = new CardType();
	//	public CardType CardTypeData {
	//		get { return cardTypeData; }
	//		set {
	//			if (value == null)
	//				cardTypeData = new CardType();
	//			else
	//				cardTypeData = value;
	//			ReportPropertyChanging("CardType");
	//			_CardType = cardTypeData.ToJson();
	//			ReportPropertyChanged("CardType");
	//			cardTypeText = CardTypeData.ToString();
	//		}
	//	}

	//	string oldCardType = null;
	//	partial void OnCardTypeChanged() {
	//		try { cardTypeData = SQLiteLocal.CardType.FromJson(CardType); }
	//		catch (Exception ex) {
	//			ReportPropertyChanging("CardType");
	//			_CardType = oldCardType;
	//			ReportPropertyChanged("CardType");
	//			Logger.Default.LogError("Error parsing card type.", ex, "<EncodedTypeString><![CDATA[" + CardType + "]]></EncodedTypeString><card><id>" + this.Id + "</id><name>" + this.Name + "</name></card>");
	//		}
	//		cardTypeText = cardTypeData.ToString();
	//	}
	//	partial void OnCardTypeChanging(string value) {
	//		oldCardType = value;
	//	}

	//	string oldManaCost = null;
	//	partial void OnManaCostChanged() {
	//		SQLiteLocal.ManaCost oldData = manaCostData;
	//		if (!SQLiteLocal.ManaCost.TryParse(ManaCost, out manaCostData)) {
	//			manaCostData = oldData;
	//			Logger.Default.LogError("Error parsing card mana cost.", null, "<EncodedManaCostString><![CDATA[" + ManaCost + "]]></EncodedManaCostString><card><id>" + this.Id + "</id><name>" + this.Name + "</name></card>");
	//		}
	//		string cleanedManaCost = manaCostData.ToString();
	//		if (_ManaCost != cleanedManaCost) {
	//			ReportPropertyChanging("ManaCost");
	//			_ManaCost = cleanedManaCost;
	//			ReportPropertyChanged("ManaCost");
	//		}
	//	}

	//	private ManaCost manaCostData = null;
	//	private ManaCost ManaCostData {
	//		get { return ManaCostData; }
	//		set {
	//			if (manaCostData == value) return;
	//			manaCostData = value;
	//			ReportPropertyChanging("ManaCost");
	//			if (value == null)
	//				_ManaCost = null;
	//			else
	//				_ManaCost = value.ToString();
	//			ReportPropertyChanged("ManaCost");
	//		}
	//	}
	//}
	#endregion EF 6.x Version

	#region EF 5.x Version
	public partial class Card
	{
		private string cardTypeText = string.Empty;
		public string CardTypeText {
			get { return cardTypeText; }
			private set {
				if (cardTypeText == value) return;
				cardTypeText = value;
				cardTypeData = SQLiteLocal.CardType.FromString(value);
				encodedCardType = cardTypeData.ToJson();
			}
		}

		CardType cardTypeData = new CardType();
		public CardType CardTypeData {
			get { return cardTypeData; }
			set {
				if (cardTypeData == value) return;
				if (value == null)
					cardTypeData = new CardType();
				else
					cardTypeData = value;
				encodedCardType = cardTypeData.ToJson();
				cardTypeText = CardTypeData.ToString();
			}
		}

		private string encodedCardType = null;
		public string CardType {
			get { return encodedCardType; }
			set {
				if (encodedCardType == value) return;
				try { cardTypeData = SQLiteLocal.CardType.FromJson(value); }
				catch (Exception ex) { Logger.Default.LogError("Error parsing card type.", ex, "<EncodedTypeString><![CDATA[" + value + "]]></EncodedTypeString><card><id>" + this.Id + "</id><name>" + this.Name + "</name></card>"); }
				cardTypeText = CardTypeData.ToString();
			}
		}

		private string encodedManaCost = null;
		public string ManaCost {
			get { return encodedManaCost; }
			set {
				SQLiteLocal.ManaCost oldData = manaCostData;
				if (!SQLiteLocal.ManaCost.TryParse(value, out manaCostData)) {
					manaCostData = oldData;
					Logger.Default.LogError("Error parsing card mana cost.", null, "<EncodedManaCostString><![CDATA[" + value + "]]></EncodedManaCostString><card><id>" + this.Id + "</id><name>" + this.Name + "</name></card>");
				}
				encodedManaCost = manaCostData.ToString();
			}
		}

		private ManaCost manaCostData = null;
		public ManaCost ManaCostData {
			get { return manaCostData; }
			set {
				if (manaCostData == value) return;
				manaCostData = value;
				if (value == null)
					encodedManaCost = null;
				else
					encodedManaCost = value.ToString();
			}
		}

		public ManaSymbolSet SetManaStyle {
			get {
				CardVariation mainVar = CardVariations.FirstOrDefault();
				if (mainVar == null) return ManaSymbolSet.Default;
				return mainVar.SetManaStyle;
			}
		}
	}
	#endregion EF 5.x Version
}
