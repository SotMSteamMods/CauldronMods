using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using Handelabra.Sentinels.UnitTest;
using System.Reflection;
using Cauldron.Anathema;
using Handelabra;
using System.Collections.Generic;
using Boomlagoon.JSON;

namespace CauldronTests
{
    [TestFixture()]
    public class AssemblyTests
    {
        #region Data

        private static readonly HashSet<string> smallIcons = new HashSet<string>(StringComparer.Ordinal)
        {
            "CannotDealDamage",
            "CannotDrawCards",
            "CannotPlayCards",
            "CannotUsePowers",
            "CounterDamage",
            "DealDamageCold",
            "ImmuneToDamageCold",
            "IncreaseDamageDealtCold",
            "IncreaseDamageTakenCold",
            "RedirectDamageCold",
            "ReduceDamageTakenCold",
            "DealDamageEnergy",
            "ImmuneToDamageEnergy",
            "ReduceDamageTakenEnergy",
            "DealDamageFire",
            "ImmuneToDamageFire",
            "IncreaseDamageDealtFire",
            "IncreaseDamageTakenFire",
            "RedirectDamageFire",
            "ReduceDamageDealtFire",
            "ReduceDamageTakenFire",
            "DealDamageInfernal",
            "ReduceDamageTakenInfernal",
            "DealDamageLightning",
            "ImmuneToDamageLightning",
            "IncreaseDamageDealtLightning",
            "IncreaseDamageTakenLightning",
            "RedirectDamageLightning",
            "ReduceDamageTakenLightning",
            "DealDamageMelee",
            "ImmuneToDamageMelee",
            "ReduceDamageDealtMelee",
            "ReduceDamageTakenMelee",
            "DealDamageProjectile",
            "ImmuneToDamageProjectile",
            "IncreaseDamageDealtProjectile",
            "ReduceDamageDealtProjectile",
            "ReduceDamageTakenProjectile",
            "DealDamagePsychic",
            "ImmuneToDamagePsychic",
            "ReduceDamageTakenPsychic",
            "DealDamageRadiant",
            "ReduceDamageTakenRadiant",
            "DealDamageSonic",
            "ImmuneToDamageSonic",
            "IncreaseDamageDealtSonic",
            "ReduceDamageTakenSonic",
            "DealDamageToxic",
            "ImmuneToDamageToxic",
            "ReduceDamageTakenToxic",
            "DealDamage",
            "Destroy",
            "DestroyEnvironment",
            "DestroyEquipment",
            "DestroyOngoing",
            "DestroySelf",
            "DestroyTarget",
            "DestroyHero",
            "Discard",
            "DrawCardExtra",
            "DrawCardNow",
            "EndOfTurnAction",
            "GainHP",
            "HasPower",
            "ImmuneToDamage",
            "IncreaseDamageDealt",
            "IncreaseDamageTaken",
            "IncreaseGainHP",
            "ReduceGainHP",
            "MakeDamageIrreducible",
            "MakeDecision",
            "Manipulate",
            "PlayCardExtra",
            "PlayCardNow",
            "RedirectDamage",
            "ReduceDamageDealt",
            "ReduceDamageTaken",
            "Search",
            "SkipTurn",
            "StartAndEndOfTurnAction",
            "StartOfTurnAction",
            "UsePowerExtra",
            "UsePowerNow",
            "Indestructible",
            "Perform",
            "PerformMelody",
            "PerformHarmony",
            "PerformRhythm",
            "Accompany",
            "AccompanyHarmony",
            "AccompanyRhythm",
            "CancelledDamageGreen",
            "CancelledDamageRed",
            "DealUnknownDamage",
            "MakeDamageUnredirectable",
            "FlipFaceUp",
            "FlipFaceDown",
            "FlipFaceUpAndDown",
            "LoseTheGame",
            "WinTheGame",
            "CannotWinTheGame",
            "AddTokens",
            "RemoveTokens",
            "AddOrRemoveTokens",
            "IncreaseDamageDealtEnergy",
            "IncreaseDamageDealtMelee",
            "AllForms",
            "CrocodileForm",
            "GazelleForm",
            "RhinocerosForm",
            "IncreaseDamageTakenRadiant",
            "CannotGainHP",
            "IncreaseDamageDealtInfernal",
            "CannotDealDamageRadiant",
            "RemoveFromGame",
            "IncreaseDamageDealtPsychic",
            "MakeDamageUnincreasable",
            "Filter",
            "TakeExtraTurn",
            "FlipArcanaControlToken",
            "FlipAvianControlToken",
            "FlipControlToken",
            "SwitchBattleZone",
            "MoveCountdownTokenDown",
            "MoveCountdownTokenUp",
            "RemoveDevastation",
            "AddDevastation",

            //not supported yet, but planned to be added
            "ImmuneToDamageRadiant",
            "IncreaseDamageDealtRadiant",
            "IncreaseDamageTakenRadiant",
            "RedirectDamageRadiant",
            "ReduceDamageTakenRadiant",

            "ImmuneToDamageSonic",
            "IncreaseDamageDealtSonic",
            "IncreaseDamageTakenSonic",
            "RedirectDamageSonic",
            "ReduceDamageTakenSonic",
        };

        #endregion Data

        [Test()]
        [Order(1)]
        public void DeckListResourceNaming()
        {
            var asb = typeof(Cauldron.Necro.NecroCharacterCardController).Assembly;

            Console.WriteLine("Checking Embedded Resource Names...");
            foreach (var res in asb.GetManifestResourceNames())
            {
                var split = res.Split('.');

                Assert.AreEqual("Cauldron", split[0], "{0} has {1} and should have {2}", res, split[0], "Cauldron");
                Assert.AreEqual("DeckLists", split[1], "{0} has {1} and should have {2}", res, split[1], "DeckLists");
                StringAssert.EndsWith("DeckList", split[2], "{0} ends with {1} and end with {2}", res, split[2], "DeckList");
                Assert.AreEqual("json", split[3], "{0} has {1} and should have {2}", res, split[3], "json");
            }
            Console.WriteLine("Done");
        }


        [Test()]
        [Order(2)]
        public void DeckListJsonValidation()
        {
            var asb = typeof(Cauldron.Necro.NecroCharacterCardController).Assembly;

            Console.WriteLine("Checking Embedded Resource Names...");
            foreach (var res in asb.GetManifestResourceNames())
            {
                Console.Write(res);
                Console.WriteLine("...");

                var stream = asb.GetManifestResourceStream(res);
                Assert.IsNotNull(stream, "'{0}' resource stream is null!", res);
                Assert.IsTrue(stream.Length != 0, "'{0}' resource stream is empty!", res);

                JSONObject jsonObject;
                using (var sr = new System.IO.StreamReader(stream))
                {
                    string text = sr.ReadToEnd();
                    Assert.IsFalse(string.IsNullOrEmpty(text), "'{0}' resource text is empty!", res);
                    jsonObject = JSONObject.Parse(text);
                }

                Assert.IsNotNull(jsonObject, "'{0}' jsonObject failed to parse.", res);

                var name = jsonObject.GetString("name");

                Assert.IsFalse(string.IsNullOrEmpty(name), "'{0}' failed to read name from jsonObject.", res);
            }
            Console.WriteLine("Done");
        }

        [Test()]
        [Order(3)]
        public void DeckListCardIcons()
        {
            var asb = typeof(Cauldron.Necro.NecroCharacterCardController).Assembly;

            Console.WriteLine("Checking Embedded Resource Names...");
            foreach (var res in asb.GetManifestResourceNames())
            {
                Console.Write(res);
                Console.WriteLine("...");

                var stream = asb.GetManifestResourceStream(res);

                JSONObject jsonObject;
                using (var sr = new System.IO.StreamReader(stream))
                {
                    string text = sr.ReadToEnd();
                    jsonObject = JSONObject.Parse(text);
                }

                var name = jsonObject.GetString("name");
                List<JSONValue> cards = new List<JSONValue>();
                cards.AddRange(jsonObject.GetArray("cards"));
                cards.AddRange(jsonObject.GetArray("notPromoCards") ?? Enumerable.Empty<JSONValue>());
                cards.AddRange(jsonObject.GetArray("promoCards") ?? Enumerable.Empty<JSONValue>());

                List<string> errors = new List<string>();

                foreach (var jsonvalue in cards)
                {
                    var cardObject = jsonvalue.Obj;
                    string id = cardObject.GetString("title") ?? cardObject.GetString("Title") ??
                        cardObject.GetString("promoIdentifer") ?? cardObject.GetString("identifer");
                    List<string> icons = new List<string>();
                    icons.AddRange(ReadStrings(cardObject, "icons"));
                    icons.AddRange(ReadStrings(cardObject, "advancedIcons"));
                    icons.AddRange(ReadStrings(cardObject, "challengeIcons"));

                    if (icons.Count > 4)
                    {
                        errors.Add(string.Format("Deck: \"{0}\", Card \"{1}\" has more than 4 icons.  Only 4 icons will be displayed. icons: {2}", name, id, string.Join(", ", icons.ToArray())));
                    }

                    foreach (string icon in icons)
                    {
                        if(!smallIcons.Contains(icon))
                        {
                            errors.Add(string.Format("Deck: \"{0}\", Card \"{1}\" has icon '{2}' that is not in the master list.", name, id, icon));
                        }
                    }

                    icons.Clear();
                    icons.AddRange(ReadStrings(cardObject, "flippedIcons"));
                    icons.AddRange(ReadStrings(cardObject, "flippedAdvancedIcons"));
                    icons.AddRange(ReadStrings(cardObject, "flippedChallengeIcons"));

                    if (icons.Count > 4)
                    {
                        errors.Add(string.Format("Deck: \"{0}\", Card \"{1}\" has more than flipped 4 icons.  Only 4 icons will be displayed. icons: {2}", name, id, string.Join(", ", icons.ToArray())));
                    }


                    foreach (string icon in icons)
                    {
                        if (!smallIcons.Contains(icon))
                        {
                            errors.Add(string.Format("Deck: \"{0}\", Card \"{1}\" has flipped icon '{2}' that is not in the master list.", name, id, icon));
                        }
                    }
                }

                Assert.IsTrue(errors.Count == 0, string.Join(Environment.NewLine + "  ", errors.ToArray()));
            }
            Console.WriteLine("Done");
        }

        private List<string> ReadStrings(JSONObject jsonObject, string key)
        {
            List<string> list = new List<string>(); ;

            if (jsonObject.ContainsKey(key))
            {
                JSONArray array = jsonObject.GetArray(key);
                if (array != null)
                {
                    foreach (JSONValue jsonvalue in array)
                    {
                        list.Add(jsonvalue.Str);
                    }
                }
                else
                {
                    string str = jsonObject.GetString(key);
                    if (str != null)
                    {
                        list.Add(str);
                    }
                }
            }
            return list;
        }
    }
}
