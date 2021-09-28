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
using System.IO;

namespace CauldronTests.Art
{
    [TestFixture]
    class BulkArtTest : ArtSourceBase
    {
        protected readonly string ArtPath;

        public BulkArtTest() : base(null)
        {
            ArtPath = ArtTestBase.GetArtPath();
        }

        private int _numWarnings;

        [NUnit.Framework.SetUp]
        public void ResetWarnings()
        {
            _numWarnings = 0;
        }


        protected void Warn(string message)
        {
            Assert.Warn(message);
            _numWarnings++;
        }

        protected void AssertNoWarnings()
        {
            Assert.AreEqual(0, _numWarnings, $"There were {_numWarnings} warnings.");
        }

        private class Item
        {
            public string Name;
            public DeckDefinition.DeckKind Kind;
            public List<string> CardIdentifiers;
            public List<string> CharacterIdentifiers;
            public List<string> HeroLeadCharacterIdentifiers;
            public List<string> StartEndIdentifiers;
            public Dictionary<string, List<string>> SubdeckCardListsDictionary;

            public Item(object[] obj)
            {
                Name = (string)obj[0];
                Kind = (DeckDefinition.DeckKind)Enum.Parse(typeof(DeckDefinition.DeckKind), (string)obj[1]);
                CardIdentifiers = (List<string>)obj[2];
                CharacterIdentifiers = (List<string>)obj[3];
                HeroLeadCharacterIdentifiers = (List<string>)obj[4];
                StartEndIdentifiers = (List<string>)obj[5];
                SubdeckCardListsDictionary = (Dictionary<string, List<string>>)obj[6];
            }
        }

        private IEnumerable<Item> Items()
        {
            foreach (object[] obj in this)
            {
                yield return new Item(obj);
            }
        }

        [Test]
        public void Atlas()
        {
            List<string> names = new List<string>();
            foreach (var item in Items())
            {
                names.Add(item.Name);
            }

            string expectedDirectory = Path.Combine(ArtPath, @"Atlas\");

            if (!Directory.Exists(expectedDirectory))
                Assert.Fail("Directory " + expectedDirectory.Replace(ArtPath.Replace(ArtPath, "<Art>\\"), "<Art>\\") + " does not exist");

            var files = Directory.GetFiles(expectedDirectory).Select(s => Path.GetFileName(s)).ToList();

            foreach (var name in names)
            {
                int removed = files.RemoveAll(s => s.StartsWith(name, StringComparison.Ordinal));
                if (removed == 0)
                    Warn($"No Atlases found for {name}");
            }

            files.RemoveAll(s => s.StartsWith("SetupGame"));
            files.RemoveAll(s => s.StartsWith("EndGameHeroes"));
            files.RemoveAll(s => s.StartsWith("HeroLogos"));
            files.RemoveAll(s => s.StartsWith("Icons"));


            foreach (var file in files)
            {
                Warn($"{file} isn't used by any decks.");
            }

            AssertNoWarnings();
        }

        [Test]
        public void InSetupGameAtlas()
        {
            List<string> names = new List<string>();
            foreach (var item in Items())
            {
                switch (item.Kind)
                {
                    case DeckDefinition.DeckKind.Environment:
                        names.Add(item.Name + "DeckBack");
                        break;
                    default:
                        names.AddRange(item.HeroLeadCharacterIdentifiers);
                        break;
                }
            }

            string expectedDirectory = Path.Combine(ArtPath, @"Atlas\");

            if (!Directory.Exists(expectedDirectory))
                Assert.Fail("Directory " + expectedDirectory.Replace(ArtPath.Replace(ArtPath, "<Art>\\"), "<Art>\\") + " does not exist");

            var atlas = ArtTestBase.ReadAtlasJson(expectedDirectory, "SetupGame");
            if (atlas is null)
                Assert.Fail("Atlas file " + "SetupGame" + " does not exist");

            foreach (var character in names)
            {
                if (!atlas.Remove(character))
                {
                    Warn($"{character} - Game Setup Atlas art is missing");
                }
            }

            foreach (var character in atlas)
            {
                Warn($"Sprite {character} isn't used by any decks.");
            }

            AssertNoWarnings();
        }

        [Test]
        public void InHeroLogoAtlas()
        {
            List<string> names = new List<string>();
            foreach (var item in Items())
            {
                if (item.Kind == DeckDefinition.DeckKind.Hero)
                    names.AddRange(item.HeroLeadCharacterIdentifiers);
                if (item.Name == "Titan")
                    names.AddRange(new[] { "FutureTitanFormCharacter", "MinistryOfStrategicScienceTitanFormCharacter", "OniTitanFormCharacter", "TitanFormCharacter" });
            }

            string expectedDirectory = Path.Combine(ArtPath, @"Atlas\");

            if (!Directory.Exists(expectedDirectory))
                Assert.Fail("Directory " + expectedDirectory.Replace(ArtPath.Replace(ArtPath, "<Art>\\"), "<Art>\\") + " does not exist");

            var atlas = ArtTestBase.ReadAtlasJson(expectedDirectory, "HeroLogos");
            if (atlas is null)
                Assert.Fail("Atlas file " + "SetupGame" + " does not exist");

            foreach (var character in names)
            {
                if (!atlas.Remove(character))
                {
                    Warn($"{character} - Hero Logo Atlas art is missing");
                }
            }

            foreach (var character in atlas)
            {
                Warn($"Sprite {character} isn't used by any decks.");
            }

            AssertNoWarnings();
        }


        [Test]
        public void StartOfGameHeroes()
        {
            List<string> identifiers = new List<string>();
            foreach (var item in Items())
            {
                if (item.Kind == DeckDefinition.DeckKind.Hero)
                    identifiers.AddRange(item.StartEndIdentifiers);
            }

            string expectedDirectory = Path.Combine(ArtPath, @"Cutouts\StartOfGame\Heroes");

            if (!Directory.Exists(expectedDirectory))
                Assert.Fail("Directory " + expectedDirectory.Replace(ArtPath.Replace(ArtPath, "<Art>\\"), "<Art>\\") + " does not exist");

            var files = Directory.GetFiles(expectedDirectory).Select(s => Path.GetFileName(s)).ToList();

            foreach (var id in identifiers)
            {
                int removed = files.RemoveAll(s => s.StartsWith(id + "StartOfGame", StringComparison.Ordinal));
                if (removed == 0)
                    Warn($"No Start of Game images found for {id}");
            }

            foreach (var file in files)
            {
                Warn($"{file} isn't used by any decks.");
            }

            AssertNoWarnings();
        }

        [Test]
        public void StartOfGameVillains()
        {
            List<string> identifiers = new List<string>();
            foreach (var item in Items())
            {
                if (item.Kind == DeckDefinition.DeckKind.Villain)
                    identifiers.AddRange(item.StartEndIdentifiers);
            }

            string expectedDirectory = Path.Combine(ArtPath, @"Cutouts\StartOfGame\Villains");

            if (!Directory.Exists(expectedDirectory))
                Assert.Fail("Directory " + expectedDirectory.Replace(ArtPath.Replace(ArtPath, "<Art>\\"), "<Art>\\") + " does not exist");

            var files = Directory.GetFiles(expectedDirectory).Select(s => Path.GetFileName(s)).ToList();

            foreach (var id in identifiers)
            {
                int removed = files.RemoveAll(s => s.StartsWith(id, StringComparison.Ordinal));
                if (removed == 0)
                    Warn($"No Start of Game images found for {id}");
            }

            foreach (var file in files)
            {
                Warn($"{file} isn't used by any decks.");
            }

            AssertNoWarnings();
        }

        [Test]
        public void YesNoDialogs()
        {
            List<string> identifiers = new List<string>();
            foreach (var item in Items())
            {
                if (item.Name != "Titan" && item.Kind == DeckDefinition.DeckKind.Hero)
                    identifiers.AddRange(item.StartEndIdentifiers);
                if (item.Name == "Titan")
                    identifiers.AddRange(item.CharacterIdentifiers);
            }

            string expectedDirectory = Path.Combine(ArtPath, @"Cutouts\YesNoDialog");

            if (!Directory.Exists(expectedDirectory))
                Assert.Fail("Directory " + expectedDirectory.Replace(ArtPath.Replace(ArtPath, "<Art>\\"), "<Art>\\") + " does not exist");

            var files = Directory.GetFiles(expectedDirectory).Select(s => Path.GetFileName(s)).ToList();

            foreach (var id in identifiers)
            {
                int removed = files.RemoveAll(s => s.StartsWith("YesNoDialog" + id, StringComparison.Ordinal));
                if (removed == 0)
                    Warn($"No Yes/No Dialog image found for {id}");
            }

            foreach (var file in files)
            {
                Warn($"{file} isn't used by any decks.");
            }

            AssertNoWarnings();
        }

        [Test]
        public void LargeCardArt()
        {
            List<string> names = new List<string>();
            Dictionary<string, List<string>> nameCardsDict = new Dictionary<string, List<string>>();
             foreach (var item in Items())
            {
                names.Add(item.Name);
                nameCardsDict.Add(item.Name, item.CardIdentifiers);
                nameCardsDict[item.Name].AddRange(item.CharacterIdentifiers.Select(s => s + "Back").ToList());
                foreach(string subdeckName in item.SubdeckCardListsDictionary.Keys)
                {
                    names.Add(subdeckName);
                    nameCardsDict.Add(subdeckName, item.SubdeckCardListsDictionary[subdeckName]);

                }
            }

            string expectedDirectory = Path.Combine(ArtPath, @"LargeCardTextures");

            if (!Directory.Exists(expectedDirectory))
                Assert.Fail("Directory " + expectedDirectory.Replace(ArtPath.Replace(ArtPath, "<Art>\\"), "<Art>\\") + " does not exist");

            var dirs = Directory.GetDirectories(expectedDirectory).ToList();

            foreach (var name in names)
            {
                int removed = dirs.RemoveAll(s => s.EndsWith(name, StringComparison.Ordinal));
                if (removed == 0)
                {
                    Warn($"Large card directory not found for {name}");
                    continue;
                }

                string deckSpecificDirectory = expectedDirectory + "\\" + name;
                var files = Directory.GetFiles(deckSpecificDirectory).Select(s => Path.GetFileName(s)).ToList();

                nameCardsDict[name].Add(name + "DeckBack");
                foreach(var card in nameCardsDict[name])
                {
                    removed = files.RemoveAll(s => s.Equals(card + ".jpg", StringComparison.Ordinal));
                    if (removed == 0)
                        Warn($"No large card image found for {card}");
                }

                foreach (var file in files)
                {
                    Warn($"Large Image file {file} isn't used by any cards.");
                }
            }

            foreach (var dir in dirs)
            {
                Warn($"{dir} isn't used by any decks.");
            }


            AssertNoWarnings();
        }

        [Test]
        public void HeroCutouts()
        {

            //there will be lots of warnings as the cards with effects vs no effects vary greatly

            List<string> names = new List<string>();
            Dictionary<string, List<string>> namePromosDict = new Dictionary<string, List<string>>();
            foreach (var item in Items())
            {
                if (item.Kind == DeckDefinition.DeckKind.Hero)
                {
                    names.Add(item.Name);
                    namePromosDict.Add(item.Name, item.CharacterIdentifiers.ToList());
                }
            }

            string expectedDirectory = Path.Combine(ArtPath, @"Cutouts\Heroes");

            if (!Directory.Exists(expectedDirectory))
                Assert.Fail("Directory " + expectedDirectory.Replace(ArtPath.Replace(ArtPath, "<Art>\\"), "<Art>\\") + " does not exist");

            var dirs = Directory.GetDirectories(expectedDirectory).ToList();

            foreach (var name in names)
            {
                int removed = dirs.RemoveAll(s => s.EndsWith(name, StringComparison.Ordinal));
                if (removed == 0)
                {
                    Warn($"Hero cutout directory not found for {name}");
                    continue;
                }

                string deckSpecificDirectory = expectedDirectory + "\\" + name;
                var files = Directory.GetFiles(deckSpecificDirectory).Select(s => Path.GetFileName(s)).ToList();

                List<string> cutoutList = new List<string>();
                foreach(string character in namePromosDict[name])
                {
                    string heroTurn = character + "HeroTurn";
                    string heroTurnFlipped = character + "HeroTurnFlipped";
                    string heroTurnEffects = character + "HeroTurnEffects";
                    string heroTurnDamaged = character + "HeroTurnDamaged";
                    string heroTurnDamagedEffects = character + "HeroTurnDamagedEffects";
                    string villainTurn = character + "VillainTurn";
                    string villainTurnFlipped = character + "VillainTurnFlipped";
                    string villainTurnEffects = character + "VillainTurnEffects";
                    string villainTurnDamaged = character + "VillainTurnDamaged";
                    string villainTurnDamagedEffects = character + "VillainTurnDamagedEffects";

                    cutoutList.AddRange(new List<string>() { heroTurn, heroTurnFlipped, heroTurnEffects, heroTurnDamaged, heroTurnDamagedEffects, villainTurn, villainTurnFlipped, villainTurnEffects, villainTurnDamaged, villainTurnDamagedEffects });

                }

                foreach (string cutout in cutoutList)
                {
                    removed = files.RemoveAll(s => s.Equals(cutout + ".png", StringComparison.Ordinal));
                    if (removed == 0)
                        Warn($"No cutout image found for {cutout}");
                }

                foreach (var file in files)
                {
                    Warn($"Cutout Image file {file} isn't used by any cards.");
                }
            }

            foreach (var dir in dirs)
            {
                Warn($"{dir} isn't used by any decks.");
            }


            AssertNoWarnings();
        }

        [Test]
        public void EnvironmentBackgrounds()
        {
            List<string> names = new List<string>();
            foreach (var item in Items())
            {
                if (item.Kind == DeckDefinition.DeckKind.Environment)
                {
                    names.Add(item.Name);
                }
            }

            string expectedDirectory = Path.Combine(ArtPath, @"Environments");

            if (!Directory.Exists(expectedDirectory))
                Assert.Fail("Directory " + expectedDirectory.Replace(ArtPath.Replace(ArtPath, "<Art>\\"), "<Art>\\") + " does not exist");

            var dirs = Directory.GetDirectories(expectedDirectory).ToList();

            foreach (var name in names)
            {
                int removed = dirs.RemoveAll(s => s.EndsWith(name, StringComparison.Ordinal));
                if (removed == 0)
                {
                    Warn($"Environment backgrounds directory not found for {name}");
                    continue;
                }

                string deckSpecificDirectory = expectedDirectory + "\\" + name;
                var files = Directory.GetFiles(deckSpecificDirectory).Select(s => Path.GetFileName(s)).ToList();

                List<string> imageList = new List<string>();
                for(int i = 0; i < 12; i++)
                {
                    imageList.Add(name + "-" + i);
                }
                imageList.Add(name + "-Start");

                foreach (string cutout in imageList)
                {
                    removed = files.RemoveAll(s => s.Equals(cutout + ".jpg", StringComparison.Ordinal));
                    if (removed == 0)
                        Warn($"No background image found for {cutout}");
                }

                foreach (var file in files)
                {
                    Warn($"Background Image file {file} isn't used by the game.");
                }
            }

            foreach (var dir in dirs)
            {
                Warn($"{dir} isn't used by any decks.");
            }


            AssertNoWarnings();
        }
    }
}
