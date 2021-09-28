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
    class AssembySource : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            yield return new object[] { "Cauldron", typeof(Cauldron.Necro.NecroCharacterCardController) };
        }
    }

    [TestFixtureSource(typeof(AssembySource))]
    public class AssemblyTests
    {
        #region Data

        private readonly Assembly _assembly;
        private readonly string _namespace;

        public AssemblyTests(string ns, Type sourceType)
        {
            _assembly = sourceType.Assembly;
            _namespace = ns;
        }

        private static readonly HashSet<string> smallIcons;

        static AssemblyTests()
        {
            smallIcons = new HashSet<string>(StringComparer.Ordinal)
            {
                "CannotDealDamage",
                "CannotDrawCards",
                "CannotPlayCards",
                "CannotUsePowers",
                "CounterDamage",
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
                "IncreaseGainHP",
                "ReduceGainHP",
                "MakeDamageIrreducible",
                "MakeDecision",
                "Manipulate",
                "PlayCardExtra",
                "PlayCardNow",
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
                "AllForms",
                "CrocodileForm",
                "GazelleForm",
                "RhinocerosForm",
                "CannotGainHP",
                "RemoveFromGame",
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
                "Cauldron.ShiftLR",
                "Cauldron.Bass",
                "Cauldron.Drums",
                "Cauldron.Guitar",
                "Cauldron.Vocal",
                "Cauldron.AllInstruments",
                "Cauldron.Irradiate",
                "Cauldron.Clue",
                "Cauldron.Danger",
                "Cauldron.Madness",
                "Cauldron.ClueDangerMadness"
            };

            var damageTypes = Enum.GetNames(typeof(DamageType));
            var damageStrings = new[]
            {
                "DealDamage",
                "ImmuneToDamage",
                "IncreaseDamageDealt",
                "IncreaseDamageTaken",
                "RedirectDamage",
                "ReduceDamageDealt",
                "ReduceDamageTaken"
            };
            foreach (var str in damageStrings)
            {
                smallIcons.Add(str);
            }
            //build all permutations of the damage type strings, not all are in the game currently but should be eventually
            foreach (var dt in damageTypes)
            {
                foreach (var str in damageStrings)
                {
                    smallIcons.Add(str + dt);
                }
            }
        }
        #endregion Data

        [Test()]
        [Order(1)]
        public void DeckListResourceNaming()
        {
            Console.WriteLine("Checking Embedded Resource Names...");
            foreach (var res in _assembly.GetManifestResourceNames())
            {
                var split = res.Split('.');

                Assert.AreEqual(_namespace, split[0], "{0} has {1} and should have {2}", res, split[0], _namespace);
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
            Console.WriteLine("Checking Json Deserialization...");
            foreach (var res in _assembly.GetManifestResourceNames())
            {
                Console.Write(res);
                Console.WriteLine("...");

                var stream = _assembly.GetManifestResourceStream(res);
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
            Console.WriteLine("Checking Decklist Icons...");
            foreach (var res in _assembly.GetManifestResourceNames())
            {
                Console.Write(res);
                Console.WriteLine("...");

                var stream = _assembly.GetManifestResourceStream(res);

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
                List<string> warnings = new List<string>();
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
                        warnings.Add(string.Format("Deck: \"{0}\", Card \"{1}\" has more than 4 icons.  Only 4 icons will be displayed. icons: {2}", name, id, string.Join(", ", icons.ToArray())));
                    }

                    foreach (string icon in icons)
                    {
                        if (!smallIcons.Contains(icon))
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
                        warnings.Add(string.Format("Deck: \"{0}\", Card \"{1}\" has more than flipped 4 icons.  Only 4 icons will be displayed. icons: {2}", name, id, string.Join(", ", icons.ToArray())));
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
                if (warnings.Count() > 0)
                {
                    Assert.Warn(string.Join(Environment.NewLine + "  ", warnings.ToArray()));
                }
            }

            Console.WriteLine("Done");
        }

        [Test]
        public void OpeningLinesCrossReference()
        {
            Dictionary<string, HashSet<string>> map = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
            var ress = _assembly.GetManifestResourceNames();
            for (int index = 0; index < ress.Length; index++)
            {
                string res = ress[index];
                var stream = _assembly.GetManifestResourceStream(res);

                JSONObject jsonObject;
                using (var sr = new System.IO.StreamReader(stream))
                {
                    string text = sr.ReadToEnd();
                    jsonObject = JSONObject.Parse(text);
                }

                var name = jsonObject.GetString("name");
                var kind = jsonObject.GetString("kind");
                if (kind != "Environment")
                {
                    var initialId = jsonObject.GetArray("initialCardIdentifiers").First().Str;

                    //find the initial card as that's where the opening lines should be
                    var initialCard = jsonObject.GetArray("cards").First(jc => jc.Obj.GetString("identifier") == initialId);

                    string baseIdentifier = initialCard.Obj.GetString("identifier");
                    var lineArray = initialCard.Obj.GetValue("openingLines");
                    mapHelper(map, baseIdentifier, lineArray);

                    if (jsonObject.ContainsKey("promoCards"))
                    {
                        var promos = jsonObject.GetArray("promoCards");
                        foreach (var promo in promos)
                        {
                            var identifier = promo.Obj.GetString("identifier");
                            if (identifier == baseIdentifier)
                            {
                                var promoIdentifier = promo.Obj.GetString("promoIdentifier");
                                lineArray = promo.Obj.GetValue("openingLines");
                                mapHelper(map, promoIdentifier, lineArray);
                            }
                        }
                    }

                    if (jsonObject.ContainsKey("notPromoCards"))
                    {
                        var promos = jsonObject.GetArray("notPromoCards");
                        foreach (var promo in promos)
                        {
                            var identifier = promo.Obj.GetString("identifier");
                            if (identifier == baseIdentifier)
                            {
                                var promoIdentifier = promo.Obj.GetString("promoIdentifier");
                                lineArray = promo.Obj.GetValue("openingLines");
                                mapHelper(map, promoIdentifier, lineArray);
                            }
                        }
                    }
                }
            }

            //with our map, we check each opening line is default or a key of the map
            //var keys = new HashSet<string>(map.Keys, StringComparer.OrdinalIgnoreCase);
            foreach(var kvp in map)
            {
                foreach(var target in kvp.Value)
                {
                    if (target == "default")
                        continue;

                    if (!map.TryGetValue(target, out var response))
                    {
                        Assert.Warn($"'{kvp.Key}' has an opening line for '{target}' which is not a reconized identifier");
                    }
                    else if (!response.Contains(kvp.Key))
                    {
                        Assert.Warn($"'{kvp.Key}' has an opening line for '{target}', but '{target}' doesn't have a response!");
                    }
                }
            }
            Console.WriteLine();
            Console.WriteLine("All Identifiers");
            map.Keys.OrderBy((string s) => s).ForEach(s => Console.WriteLine(s));
        }

        private void mapHelper(Dictionary<string, HashSet<string>> map, string identifier, JSONValue lineArray)
        {
            HashSet<string> lines;
            if (lineArray != null)
            {
                lines = new HashSet<string>(lineArray.Obj.Select(js => js.Key), StringComparer.OrdinalIgnoreCase);

                lineArray.Obj.ForEach(js =>
                {
                    string text = js.Value.Str;
                    if (string.IsNullOrEmpty(text))
                        Assert.Warn($"{identifier} is has a blank opening line for {js.Key}");
                    else if (text.Contains("placeholder"))
                        Assert.Warn($"{identifier} is using a placeholder opening line for {js.Key}: {text}");
                });
            }
            else
            {
                lines = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            }

            if (lines.Count == 0)
            {
                Assert.Warn($"{identifier} has no opening lines.");
            }
            else if (!lines.Contains("default"))
            {
                Assert.Warn($"{identifier} is missing a default opening line.");
            }

            if (map.ContainsKey(identifier))
            {
                Assert.Fail($"{identifier} has already been seen, it cannot be added to the map.");
            }
            else
            {
                map.Add(identifier, lines);
            }
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
