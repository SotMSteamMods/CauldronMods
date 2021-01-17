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

namespace CauldronTests.Art.Villain
{
    class VillainArtSource : ArtSourceBase
    {
        public VillainArtSource() : base("Villain") { }
    }

    [TestFixtureSource(typeof(VillainArtSource))]
    public class VillainArtTests : ArtTestBase
    {
        public VillainArtTests(string name, string kind, List<string> cardIdentifiers, List<string> characterIdentifiers, List<string> startEndIdentifiers)
            : base(name, kind, cardIdentifiers, characterIdentifiers, startEndIdentifiers)
        {
        }

        [Test]
        public void LargeCardTextures()
        {
            string expectedDirectory = Path.Combine(ArtPath, @"LargeCardTextures\" + _name);

            if (!Directory.Exists(expectedDirectory))
                Assert.Fail("Directory " + expectedDirectory.Replace(ArtPath.Replace(ArtPath, "<Art>\\"), "<Art>\\") + " does not exist");

            var files = new HashSet<string>(Directory.GetFiles(expectedDirectory).Select(s => Path.GetFileNameWithoutExtension(s)), StringComparer.OrdinalIgnoreCase);

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
                Assert.Warn($"{_name}: file '{leftovers}' was not used by any cards in the deck.");
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
                //no big deal if there are extras in the atlas
                Assert.Warn($"{_name}: Atlas entry '{leftovers}' was not used by any cards in the deck.");
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

            foreach (var character in _startEndIdentifiers)
            {
                string str = character;

                if (!atlas.Remove(str))
                {
                    Warn($"{_name}:{character} - Atlas Game Setup art is missing");
                }
            }

            AssertNoWarnings();
        }

        [Test]
        public void Endings()
        {
            string expectedDirectory = Path.Combine(ArtPath, @"Endings\Villains\" + _name);

            if (!Directory.Exists(expectedDirectory))
                Assert.Fail("Directory " + expectedDirectory.Replace(ArtPath.Replace(ArtPath, "<Art>\\"), "<Art>\\") + " does not exist");
            
            var files = new HashSet<string>(Directory.GetFiles(expectedDirectory).Select(s => Path.GetFileNameWithoutExtension(s)), StringComparer.OrdinalIgnoreCase);

            var suffixes = new List<string>() { "HeroesDestroyedDefeat", "VillainDestroyedVictory" };
            
            var alternateDefeats = new[] { "Menagerie", "Vector" };
            if (alternateDefeats.Contains(_name))
            {
                suffixes.Add("AlternateDefeat");
            }

            foreach (var character in _startEndIdentifiers)
            {
                foreach (var suffix in suffixes)
                {
                    string str = character + suffix;

                    if (!files.Remove(str))
                    {
                        Warn($"{_name}:{character} - Ending art {suffix} is missing");
                    }
                    else
                    {
                        //does exist, account for optional Effects
                        files.Remove(str + "Effects");
                    }
                }
            }

            foreach (var leftovers in files)
            {
                Assert.Warn($"{_name}: file '{leftovers}' was not used by any cards in the deck.");
            }

            AssertNoWarnings();
        }

        [Test]
        public void StartOfGame()
        {
            string expectedDirectory = Path.Combine(ArtPath, @"Cutouts\StartOfGame\Villains\");

            if (!Directory.Exists(expectedDirectory))
                Assert.Fail("Directory " + expectedDirectory.Replace(ArtPath.Replace(ArtPath, "<Art>\\"), "<Art>\\") + " does not exist");

            var files = new HashSet<string>(Directory.GetFiles(expectedDirectory).Select(s => Path.GetFileNameWithoutExtension(s)), StringComparer.OrdinalIgnoreCase);

            foreach (var character in _startEndIdentifiers)
            {
                string str = character + "StartOfGame";

                if (!files.Remove(str))
                {
                    Warn($"{_name}:{character} - Start of Game art is missing");
                }
                else
                {
                    //does exist, account for optional Effects
                    files.Remove(str + "Effects");
                }
            }

            AssertNoWarnings();
        }


        [Test]
        public void Cutouts()
        {
            string expectedDirectory = Path.Combine(ArtPath, @"Cutouts\Villains\" + _name);

            if (!Directory.Exists(expectedDirectory))
                Assert.Fail("Directory " + expectedDirectory.Replace(ArtPath.Replace(ArtPath, "<Art>\\"), "<Art>\\") + " does not exist");

            var files = new HashSet<string>(Directory.GetFiles(expectedDirectory).Select(s => Path.GetFileNameWithoutExtension(s)), StringComparer.OrdinalIgnoreCase);

            var suffixes = new string[] { "HeroTurn", "HeroTurnFlipped", "VillainTurn", "VillainTurnFlipped" };
            var optional = new string[] { "HeroTurnDamaged", "HeroTurnDamagedFlipped", "VillainTurnDamaged", "VillainTurnDamagedFlipped" };

            foreach (var character in _characterIdentifiers)
            {
                foreach (var suffix in suffixes)
                {
                    string str = character + suffix;

                    if (!files.Remove(str))
                    {
                        Warn($"{_name}:{character} - Cutout art {suffix} is missing");
                    }
                    else
                    {
                        //does exist, account for optional Effects
                        files.Remove(str + "Effects");
                    }
                }

                foreach (var suffix in optional)
                {
                    string str = character + suffix;
                    //acount of optional special ending suffixes, but do not warn if missing.
                    if (files.Remove(str))
                    {
                        //does exist, account for optional Effects
                        files.Remove(str + "Effects");
                    }
                }
            }

            foreach (var leftovers in files)
            {
                Assert.Warn($"{_name}: file '{leftovers}' was not used by any cards in the deck.");
            }

            AssertNoWarnings();
        }
    }
}
