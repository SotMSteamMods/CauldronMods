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

namespace CauldronTests.Art.Environment
{
    class EnvironmentArtSource : ArtSourceBase
    {
        public EnvironmentArtSource() : base("Environment") { }
    }

    [TestFixtureSource(typeof(EnvironmentArtSource))]
    public class EnvironmentArtTests : ArtTestBase
    {
        public static new bool IsMicroArtRequired = true;

        public EnvironmentArtTests(string name, string kind, List<string> cardIdentifiers, List<string> characterIdentifiers, List<string> heroLeadCharacterIdentifiers, List<string> startEndIdentifiers, Dictionary<string, List<string>> subdeckCardListDict)
            : base(name, kind, cardIdentifiers, characterIdentifiers, heroLeadCharacterIdentifiers, startEndIdentifiers, subdeckCardListDict)
        {
        }

        [Test]
        public void LargeCardTextures()
        {
            string expectedDirectory = Path.Combine(ArtPath, @"LargeCardTextures\" + _name);

            if (!Directory.Exists(expectedDirectory))
                Assert.Fail("Directory " + expectedDirectory.Replace(ArtPath.Replace(ArtPath, "<Art>\\"), "<Art>\\") + " does not exist");

            var files = new HashSet<string>(Directory.GetFiles(expectedDirectory).Select(s => Path.GetFileNameWithoutExtension(s)), StringComparer.Ordinal);

            if (!files.Remove(_name + "DeckBack"))
            {
                Warn($"{_name} - DeckBack LargeCardArt is missing");
            }

            foreach (var card in _cardIdentifiers)
            {
                if (!files.Remove(card))
                {
                    Warn($"{_name}: {card} - Card LargeCardArt front art is missing");
                }
            }

            foreach (var character in _characterIdentifiers)
            {
                if (!files.Remove(character + "Back"))
                {
                    Warn($"{_name}: {character}Back - Character LargeCardArt flipped art is missing.");
                }
            }

            foreach (var leftovers in files)
            {
                WarnAboutUnused($"{_name}: file '{leftovers}' was not used by any cards in the deck.");
            }

            AssertNoWarnings();
        }

        [Test]
        public void Atlas()
        {
            string expectedDirectory = Path.Combine(ArtPath, @"Atlas\");

            if (!Directory.Exists(expectedDirectory))
                Assert.Fail("Directory " + expectedDirectory.Replace(ArtPath.Replace(ArtPath, "<Art>\\"), "<Art>\\") + " does not exist");

            var atlas = ReadAtlasJson(expectedDirectory, _name);
            if (atlas is null)
                Assert.Fail("Atlas file " + _name + " does not exist");

            if (!atlas.Remove(_name + "DeckBack"))
            {
                Warn($"{_name} - Card Atlas DeckBack is missing");
            }

            if (!atlas.Remove(_name + "DeckBackMicro"))
            {
                if (IsMicroArtRequired)
                    Warn($"{_name} - Card Atlas DeckBackMicro is missing");
            }

            foreach (var card in _cardIdentifiers)
            {
                if (!atlas.Remove(card))
                {
                    Warn($"{_name}: {card} - Card Atlas Front art is missing");
                }
                if (!atlas.Remove(card + "Micro"))
                {
                    if (IsMicroArtRequired)
                        Warn($"{_name}: {card + "Micro"} - Card Atlas Front art is missing");
                }
            }

            foreach (var character in _characterIdentifiers)
            {
                if (!atlas.Remove(character + "Back"))
                {
                    Warn($"{_name}: {character}Back - Card Atlas Flipped art is missing.");
                }
                if (!atlas.Remove(character + "BackMicro"))
                {
                    if (IsMicroArtRequired)
                        Warn($"{_name}: {character + "BackMicro"} - Card Atlas Flipped art is missing");
                }
            }

            foreach (var leftovers in atlas)
            {
                WarnAboutUnused($"{_name}: Atlas entry '{leftovers}' was not used by any cards in the deck.");
            }

            AssertNoWarnings();
        }

        [Test]
        public void GameSetup()
        {
            string expectedDirectory = Path.Combine(ArtPath, @"Atlas\");

            if (!Directory.Exists(expectedDirectory))
                Assert.Fail("Directory " + expectedDirectory.Replace(ArtPath.Replace(ArtPath, "<Art>\\"), "<Art>\\") + " does not exist");

            var atlas = ReadAtlasJson(expectedDirectory, "SetupGame");
            if (atlas is null)
                Assert.Fail("Atlas file " + "SetupGame" + " does not exist");

            if (!atlas.Remove(_name + "DeckBack"))
            {
                Warn($"{_name} - Atlas DeckBack Game Setup art is missing");
            }

            AssertNoWarnings();
        }

        [Test]
        public void Enviroments()
        {
            string expectedDirectory = Path.Combine(ArtPath, @"Environments\" + _name);
            if (!Directory.Exists(expectedDirectory))
                Assert.Fail("Directory " + expectedDirectory.Replace(ArtPath, "<Art>\\") + " does not exist");

            var files = new HashSet<string>(Directory.GetFiles(expectedDirectory).Select(s => Path.GetFileNameWithoutExtension(s)), StringComparer.Ordinal);

            string[] suffixes = new string[] { "-0", "-1", "-2", "-3", "-4", "-5", "-6", "-7", "-8", "-9", "-10", "-11", "-Start" };

            foreach (var suffix in suffixes)
            {
                if (!files.Remove(_name + suffix))
                {
                    Warn($"{_name}: {_name + suffix} - Environment Scene Art is missing");
                }
            }

            foreach (var leftovers in files)
            {
                WarnAboutUnused($"{_name}: file '{leftovers}' is not a reconized Environment scene name.");
            }

            AssertNoWarnings();
        }


        [Test]
        public void Endings()
        {
            string expectedDirectory = Path.Combine(ArtPath, @"Endings\Environments\");

            if (!Directory.Exists(expectedDirectory))
                Assert.Fail("Directory " + expectedDirectory.Replace(ArtPath, "<Art>\\") + " does not exist");

            var files = new HashSet<string>(Directory.GetFiles(expectedDirectory, "Ending-" + _name + "*.*").Select(s => Path.GetFileNameWithoutExtension(s)), StringComparer.Ordinal);

            if (!files.Remove("Ending-" + _name))
            {
                Warn($"{_name}: Ending Environment Scene Art is missing");
            }

            if (_name == "CatchwaterHarbor")
                files.Remove("Ending-" + _name + "EnvironmentDefeat");

            if (_name == "TheCybersphere")
                files.Remove("Ending-" + _name + "EnvironmentDefeat");

            if (_name == "TheWanderingIsle")
                files.Remove("Ending-" + _name + "EnvironmentDefeat");

            foreach (var leftovers in files)
            {
                WarnAboutUnused($"{_name}: file '{leftovers}' was not used by any cards in the deck.");
            }

            AssertNoWarnings();
        }
    }
}
