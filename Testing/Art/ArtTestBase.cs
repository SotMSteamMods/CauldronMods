using Boomlagoon.JSON;
using CauldronTests.Utilities;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CauldronTests.Art
{
    abstract class ArtSourceBase : IEnumerable
    {
        //get the list of decks, and promo's for each, cards for each
        private readonly string _kind;
        protected IEnumerable<string> ParsingErrors { get; private set; }
        protected ArtSourceBase(string kind)
        {
            _kind = kind;
        }

        public IEnumerator GetEnumerator()
        {
            Assembly assembly = typeof(Cauldron.Necro.NecroCharacterCardController).Assembly;

            // When Boomlagoon.JSON parses the JSONObject,
            // it will write any errors out to stdout.
            // This has the potential to significantly spam stdout
            // Instead, we redirect all of those writes into a PrefixStringWriter.
            // After parsing, we save all of those into this.ParsingErrors
            // and reset the output back to stdout
            PrefixStringWriter standardOutputString = new PrefixStringWriter(Console.OutputEncoding);
            Console.SetOut(standardOutputString);

            List<object[]> results = new List<object[]>();

            var ress = assembly.GetManifestResourceNames();
            for (int index = 0; index < ress.Length; index++)
            {
                string res = ress[index];
                var stream = assembly.GetManifestResourceStream(res);

                JSONObject jsonObject;
                using (var sr = new System.IO.StreamReader(stream))
                {
                    string text = sr.ReadToEnd();
                    jsonObject = JSONObject.Parse(text);
                }

                string name = Path.GetFileNameWithoutExtension(res.Replace("Cauldron.DeckLists.", "").Replace("DeckList", ""));
                standardOutputString.Prefix = $"[{name}] ";
                var kind = jsonObject.GetString("kind");
                if (_kind != null && kind != _kind)
                    continue;

                string initialCard = "<>";
                if (kind != "Environment")
                {
                    initialCard = jsonObject.GetArray("initialCardIdentifiers").First().Str;
                }

                var cards = jsonObject.GetArray("cards");

                List<string> cardIdentifiers = new List<string>();
                List<string> characterIdentifiers = new List<string>();
                List<string> heroLeadCharacterIdentifiers = new List<string>();
                List<string> startEndIdentifiers = new List<string>();

                foreach (JSONValue card in cards)
                {
                    var cardIdentifier = card.Obj.GetString("identifier");
                    standardOutputString.Prefix = $"[{name}.{cardIdentifier}] ";
                    var sharedIdentifier = card.Obj.GetString("sharedIdentifier");
                    bool isCharacter = card.Obj.GetBoolean("character");

                    cardIdentifiers.Add(cardIdentifier);

                    if (isCharacter)
                        characterIdentifiers.Add(cardIdentifier);

                    if (cardIdentifier == initialCard)
                    {
                        if (string.IsNullOrEmpty(sharedIdentifier))
                        {
                            startEndIdentifiers.Add(cardIdentifier);
                        }
                        else
                        {
                            startEndIdentifiers.Add(sharedIdentifier);
                        }
                        heroLeadCharacterIdentifiers.Add(cardIdentifier);
                    }
                }


                List<JSONValue> promos = new List<JSONValue>();
                if (jsonObject.ContainsKey("promoCards"))
                {
                    promos.AddRange(jsonObject.GetArray("promoCards"));
                }
                if (jsonObject.ContainsKey("notPromoCards"))
                {
                    promos.AddRange(jsonObject.GetArray("notPromoCards"));
                }

                foreach (JSONValue card in promos)
                {
                    var cardIdentifier = card.Obj.GetString("identifier");
                    standardOutputString.Prefix = $"[{name}.{cardIdentifier}] ";
                    var promoIdentifier = card.Obj.GetString("promoIdentifier");
                    bool isCharacter = card.Obj.GetBoolean("character");

                    cardIdentifiers.Add(promoIdentifier);

                    if (isCharacter)
                        characterIdentifiers.Add(promoIdentifier);

                    if (cardIdentifier == initialCard)
                    {
                        startEndIdentifiers.Add(promoIdentifier);
                        heroLeadCharacterIdentifiers.Add(promoIdentifier);
                    }
                }

                List<JSONValue> subdecks = new List<JSONValue>();
                Dictionary<string, List<string>> subdeckCardListDict = new Dictionary<string, List<string>>();
                if (jsonObject.ContainsKey("subdecks"))
                {
                    subdecks.AddRange(jsonObject.GetArray("subdecks"));
                }
                foreach (JSONValue subdeck in subdecks)
                {
                    var subdeckIdentifier = subdeck.Obj.GetString("identifier");
                    standardOutputString.Prefix = $"[{name}.{subdeckIdentifier}] ";
                    var subdeckCards = subdeck.Obj.GetArray("cards");
                    List<string> subdeckCardIdentifiers = new List<string>();
                    foreach (JSONValue card in subdeckCards)
                    {
                        var cardIdentifier = card.Obj.GetString("identifier");
                        subdeckCardIdentifiers.Add(cardIdentifier);
                    }
                    subdeckCardListDict.Add(subdeckIdentifier, subdeckCardIdentifiers);
                }

                results.Add(ModifyForSpecificDecks(name, kind, cardIdentifiers, characterIdentifiers, heroLeadCharacterIdentifiers, startEndIdentifiers, subdeckCardListDict));
            }

            // Save the StringWriter with all parsing errors
            // into this.ParsingErrors
            ParsingErrors = standardOutputString.ToString().Split([standardOutputString.NewLine], StringSplitOptions.None);

            // Reset Console to use stdout again
            StreamWriter standardOutput = new StreamWriter(Console.OpenStandardOutput());
            Console.SetOut(standardOutput);

            return results.GetEnumerator();
        }

        private object[] ModifyForSpecificDecks(string name, string kind, List<string> cardIdentifiers, List<string> characterIdentifiers, List<string> heroLeadCharacterIdentifiers, List<string> startEndIdentifiers, Dictionary<string, List<string>> subdeckCardListDict)
        {
            if (name == "MagnificentMara")
            {
                cardIdentifiers.Add("MesmerPendant");
            }

            return [name, kind, cardIdentifiers, characterIdentifiers, heroLeadCharacterIdentifiers, startEndIdentifiers, subdeckCardListDict];
        }

    }

    public abstract class ArtTestBase
    {
        protected static readonly Dictionary<string, HashSet<string>> warningsToIgnore = new Dictionary<string, HashSet<string>>(StringComparer.Ordinal)
        {
            ["TheMistressOfFate"] = new HashSet<string>(StringComparer.Ordinal)
            {
                "TheMistressOfFate: file 'TheMistressOfFateDayDeckBack' was not used by any cards in the deck."
            },
            ["Tiamat"] = new HashSet<string>(StringComparer.Ordinal)
            {
                "Tiamat: Atlas entry 'ElementalHydraTiamat' was not used by any cards in the deck."
            },
            ["Mythos"] = new HashSet<string>(StringComparer.Ordinal)
            {
                "Mythos: Atlas entry 'MythosClueDeckBack' was not used by any cards in the deck.",
                "Mythos: Atlas entry 'MythosDangerDeckBack' was not used by any cards in the deck.",
                "Mythos: Atlas entry 'MythosMadnessDeckBack' was not used by any cards in the deck.",

                "Mythos: file 'MythosEyeDeckBack' was not used by any cards in the deck.",
                "Mythos: file 'MythosFearDeckBack' was not used by any cards in the deck.",
                "Mythos: file 'MythosMindDeckBack' was not used by any cards in the deck.",

                "Large Image file MythosEyeDeckBack.jpg isn't used by any cards.",
                "Large Image file MythosFearDeckBack.jpg isn't used by any cards.",
                "Large Image file MythosMindDeckBack.jpg isn't used by any cards."

            },
        };

        #region Data

        public static bool IsMicroArtRequired = false;
        public static bool IsUnusedCardError = true;

        protected readonly string ArtPath;

        protected readonly string _name;
        protected readonly string _kind;
        protected readonly List<string> _cardIdentifiers;
        protected readonly List<string> _characterIdentifiers;
        protected readonly List<string> _heroLeadCharacterIdentifiers;
        protected readonly List<string> _startEndIdentifiers;
        protected readonly Dictionary<string, List<string>> _subdeckCardListDict;
        protected readonly HashSet<string> _ignoredWarnings;
        private int _numWarnings;


        protected ArtTestBase(string name, string kind, List<string> cardIdentifiers, List<string> characterIdentifiers, List<string> heroLeadCharacterIdentifiers, List<string> startEndIdentifiers, Dictionary<string, List<string>> subdeckCardListDict)
        {
            ArtPath = GetArtPath();

            _name = name;
            _kind = kind;
            _cardIdentifiers = cardIdentifiers;
            _characterIdentifiers = characterIdentifiers;
            _heroLeadCharacterIdentifiers = heroLeadCharacterIdentifiers;
            _startEndIdentifiers = startEndIdentifiers;
            _subdeckCardListDict = subdeckCardListDict;

            if (!warningsToIgnore.TryGetValue(_name, out var local))
            {
                local = new HashSet<string>(StringComparer.Ordinal);
            }
            _ignoredWarnings = local;
        }

        #endregion Data

        public static string GetArtPath()
        {
            var di = new DirectoryInfo(ProjectPath.Path);
            DirectoryAssert.Exists(di);

            string path = Path.Combine(di.Parent.FullName, @"Art\");
            DirectoryAssert.Exists(path);

            return path;
        }


        public static HashSet<string> ReadAtlasJson(string directoryPath, string atlasName)
        {
            var files = Directory.GetFiles(directoryPath, atlasName + "*.json").Select(s => Path.GetFileName(s)).ToArray();
            if (files.Length == 0)
                return null;
            HashSet<string> results = new HashSet<string>();
            foreach (var file in files)
            {
                JSONObject jsonObject;
                using (var sr = new System.IO.StreamReader(Path.Combine(directoryPath, file)))
                {
                    string text = sr.ReadToEnd();
                    jsonObject = JSONObject.Parse(text);
                }

                var frames = jsonObject.GetValue("frames");
                foreach (var thing in frames.Obj)
                {
                    if (!results.Add(Path.GetFileNameWithoutExtension(thing.Key)))
                    {
                        Assert.Warn($"In file: {file}, {thing.Key} was already present");
                    }
                }
            }

            return results;
        }

        [NUnit.Framework.SetUp]
        public void ResetWarnings()
        {
            _numWarnings = 0;
        }


        protected void Warn(string message)
        {
            if (!_ignoredWarnings.Contains(message))
            {
                Assert.Warn(message);
                _numWarnings++;
            }
        }

        protected void WarnAboutUnused(string message)
        {
            if (!_ignoredWarnings.Contains(message))
            {
                Assert.Warn(message);
                if (IsUnusedCardError)
                    _numWarnings++;
            }
        }

        protected void AssertNoWarnings()
        {
            Assert.AreEqual(0, _numWarnings, $"There were {_numWarnings} warnings.");
        }

    }
}
