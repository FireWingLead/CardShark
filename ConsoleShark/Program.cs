using CardShark.PCShark.DAL.SQLiteLocal;
using CardShark.PCShark.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeJSON;

namespace ConsoleShark
{
	class Program
	{
		static void OldMain(string[] args) {
			Console.WriteLine();
			Console.Write("Press any key to exit...");
			Console.ReadKey(true);
		}
		static void Main(string[] args) {
			Set set1 = new Set() {
				Code = "FFL",
				Description = null,
				Name = "FireFall Core",
			};
			Card card1 = new Card() {
				Author = "Perrin Larson",
				CardType = "{\"SuperTypes\":[],\"MainTypes\":[\"Instant\"],\"SubTypes\":[\"Ability\"],\"Classes\":[]}",
				Id = 22,
				LoyaltyVal = null,
				LoyaltyVar = null,
				ManaCost = "{1}{R}{R}",
				Name = "Afterburner",
				PowerVal = null,
				PowerVar = null,
				RuleText = "Remove target creature you control from combat, or make it unblockable until end of turn.",
				ToughnessVal = null,
				ToughnessVar = null
			};
			Card card2 = new Card() {
				Author = "Perrin Larson",
				CardType = "{\"SuperTypes\":[],\"MainTypes\":[\"Artifact\",\"Creature\"],\"SubTypes\":[\"Drone\"],\"Classes\":[\"Pet\"]}",
				Id = 23,
				LoyaltyVal = null,
				LoyaltyVar = null,
				ManaCost = "{1}",
				Name = "T.E.X.",
				PowerVal = 0,
				PowerVar = null,
				RuleText = "When this enters the battlefield, it becomes linked to target creature you control.\n{T}: Deal 1 damage to target creature in combat opposing the creature linked to this.",
				ToughnessVal = 1,
				ToughnessVar = null
			};
			set1.CardVariations.Add(new CardVariation() {
				Artist = null,
				Author = "Perrin Larson",
				Card = card1,
				CardId = card1.Id,
				FlavorText = "soooLOOONG-SUCKAAaaaaahs!",
				Id = 35,
				Set = set1,
				SetCode = set1.Code
			});
			card1.CardVariations.Add(set1.CardVariations.ElementAt(0));
			set1.CardVariations.Add(new CardVariation() {
				Artist = null,
				Author = "Perrin Larson",
				Card = card1,
				CardId = card1.Id,
				FlavorText = "yeeeee-HAAAAAAW!",
				Id = 36,
				Set = set1,
				SetCode = set1.Code
			});
			card1.CardVariations.Add(set1.CardVariations.ElementAt(1));
			set1.CardVariations.Add(new CardVariation() {
				Artist = null,
				Author = "Perrin Larson",
				Card = card2,
				CardId = card2.Id,
				FlavorText = "Your very own Tactical Excursion Drone!",
				Id = 37,
				Set = set1,
				SetCode = set1.Code
			});
			card2.CardVariations.Add(set1.CardVariations.ElementAt(2));

			JSONNode setsNode = new JSONNode(new List<JSONNode>());
			JSONNode cardsNode = new JSONNode(new List<JSONNode>());
			JSONNode root = new JSONNode(new KeyValuePair<string, JSONNode>[] {
				new KeyValuePair<string, JSONNode>("sets", setsNode),
				new KeyValuePair<string, JSONNode>("cards", cardsNode)
			});

			setsNode.ArrayTypeMembers.Add(new JSONNode(new KeyValuePair<string, JSONNode>[] {
				new KeyValuePair<string, JSONNode>("code", new JSONNode(set1.Code)),
				new KeyValuePair<string, JSONNode>("description", new JSONNode(set1.Description)),
				new KeyValuePair<string, JSONNode>("name", new JSONNode(set1.Name)),
				new KeyValuePair<string, JSONNode>("cardVariations", new JSONNode(set1.CardVariations.ConvertToList(cardVar => {
					return new JSONNode(new KeyValuePair<string, JSONNode>[] {
						new KeyValuePair<string, JSONNode>("artist", new JSONNode(cardVar.Artist)),
						new KeyValuePair<string, JSONNode>("author", new JSONNode(cardVar.Author)),
						new KeyValuePair<string, JSONNode>("cardId", new JSONNode(cardVar.CardId)),
						new KeyValuePair<string, JSONNode>("flavorText", new JSONNode(cardVar.FlavorText)),
						new KeyValuePair<string, JSONNode>("id", new JSONNode(cardVar.Id)),
						new KeyValuePair<string, JSONNode>("setCode", new JSONNode(cardVar.SetCode)),
						new KeyValuePair<string, JSONNode>("setName", cardVar.Set == null ? new JSONNode() : new JSONNode(cardVar.Set.Name))
					});
				})))
			}));
			cardsNode.ArrayTypeMembers.Add(new JSONNode(new KeyValuePair<string, JSONNode>[] {
				new KeyValuePair<string, JSONNode>("author", new JSONNode(card1.Author)),
				new KeyValuePair<string, JSONNode>("cardType", new JSONNode(card1.CardType)),
				new KeyValuePair<string, JSONNode>("id", new JSONNode(card1.Id)),
				new KeyValuePair<string, JSONNode>("loyaltyVal", new JSONNode(card1.LoyaltyVal)),
				new KeyValuePair<string, JSONNode>("loyaltyVar", new JSONNode(card1.LoyaltyVar)),
				new KeyValuePair<string, JSONNode>("manaCost", new JSONNode(card1.ManaCost)),
				new KeyValuePair<string, JSONNode>("name", new JSONNode(card1.Name)),
				new KeyValuePair<string, JSONNode>("powerVal", new JSONNode(card1.PowerVal)),
				new KeyValuePair<string, JSONNode>("powerVar", new JSONNode(card1.PowerVar)),
				new KeyValuePair<string, JSONNode>("ruleText", new JSONNode(card1.RuleText)),
				new KeyValuePair<string, JSONNode>("toughnessVal", new JSONNode(card1.ToughnessVal)),
				new KeyValuePair<string, JSONNode>("toughnessVar", new JSONNode(card1.ToughnessVar)),
				new KeyValuePair<string, JSONNode>("variationIds", new JSONNode(card1.CardVariations.ConvertToList(cardVar => {
					return new JSONNode(cardVar.Id);
				})))
			}));
			cardsNode.ArrayTypeMembers.Add(new JSONNode(new KeyValuePair<string, JSONNode>[] {
				new KeyValuePair<string, JSONNode>("author", new JSONNode(card2.Author)),
				new KeyValuePair<string, JSONNode>("cardType", new JSONNode(card2.CardType)),
				new KeyValuePair<string, JSONNode>("id", new JSONNode(card2.Id)),
				new KeyValuePair<string, JSONNode>("loyaltyVal", new JSONNode(card2.LoyaltyVal)),
				new KeyValuePair<string, JSONNode>("loyaltyVar", new JSONNode(card2.LoyaltyVar)),
				new KeyValuePair<string, JSONNode>("manaCost", new JSONNode(card2.ManaCost)),
				new KeyValuePair<string, JSONNode>("name", new JSONNode(card2.Name)),
				new KeyValuePair<string, JSONNode>("powerVal", new JSONNode(card2.PowerVal)),
				new KeyValuePair<string, JSONNode>("powerVar", new JSONNode(card2.PowerVar)),
				new KeyValuePair<string, JSONNode>("ruleText", new JSONNode(card2.RuleText)),
				new KeyValuePair<string, JSONNode>("toughnessVal", new JSONNode(card2.ToughnessVal)),
				new KeyValuePair<string, JSONNode>("toughnessVar", new JSONNode(card2.ToughnessVar)),
				new KeyValuePair<string, JSONNode>("variationIds", new JSONNode(card2.CardVariations.ConvertToList(cardVar => {
					return new JSONNode(cardVar.Id);
				})))
			}));

			string json = root.ToString();
			Console.WriteLine(json);
			Console.WriteLine();
			JSONNode parsed = new JSONParser().Parse(json);
			Console.WriteLine(parsed.ToStringPretty(0, true));
			Console.WriteLine();
			Console.Write("Press any key to exit...");
			Console.ReadKey(true);
		}
	}
}
