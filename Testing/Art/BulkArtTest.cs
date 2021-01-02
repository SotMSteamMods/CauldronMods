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
            ArtPath = Path.Combine(ProjectPath.Path, @"Art\");
        }

        private class Item
        {
            public string Name;
            public string Kind;
            public List<string> CardIdentifiers;
            public List<string> CharacterIdentifiers;
            public List<string> StartEndIdentifiers;

            public Item(object[] obj)
            {
                Name = (string)obj[0];
                Kind = (string)obj[1];
                CardIdentifiers = (List<string>)obj[2];
                CharacterIdentifiers = (List<string>)obj[3];
                StartEndIdentifiers = (List<string>)obj[4];
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
                Assert.Inconclusive();

            var files = Directory.GetFiles(expectedDirectory).Select(s => Path.GetFileName(s)).ToList();

            foreach (var name in names)
            {
                int removed = files.RemoveAll(s => s.StartsWith(name, StringComparison.OrdinalIgnoreCase));
                if (removed == 0)
                    Assert.Warn($"No Atlases found for {name}");
            }

            files.RemoveAll(s => s.StartsWith("SetupGame"));
            files.RemoveAll(s => s.StartsWith("EndGameHeroes"));
            files.RemoveAll(s => s.StartsWith("HeroLogos"));

            foreach (var file in files)
            {
                Assert.Warn($"{file} isn't used by any decks.");
            }
        }

        [Test]
        public void StartOfGameHeroes()
        {
            List<string> identifiers = new List<string>();
            foreach (var item in Items())
            {
                if (item.Kind == "Hero")
                    identifiers.AddRange(item.StartEndIdentifiers);
            }

            string expectedDirectory = Path.Combine(ArtPath, @"Cutouts\StartOfGame\Heroes");

            if (!Directory.Exists(expectedDirectory))
                Assert.Inconclusive();

            var files = Directory.GetFiles(expectedDirectory).Select(s => Path.GetFileName(s)).ToList();

            foreach (var id in identifiers)
            {
                int removed = files.RemoveAll(s => s.StartsWith(id, StringComparison.OrdinalIgnoreCase));
                if (removed == 0)
                    Assert.Warn($"No Start of Game images found for {id}");
            }

            foreach (var file in files)
            {
                Assert.Warn($"{file} isn't used by any decks.");
            }
        }

        [Test]
        public void StartOfGameVillains()
        {
            List<string> identifiers = new List<string>();
            foreach (var item in Items())
            {
                if (item.Kind == "Villain")
                    identifiers.AddRange(item.StartEndIdentifiers);
            }

            string expectedDirectory = Path.Combine(ArtPath, @"Cutouts\StartOfGame\Villains");

            if (!Directory.Exists(expectedDirectory))
                Assert.Inconclusive();

            var files = Directory.GetFiles(expectedDirectory).Select(s => Path.GetFileName(s)).ToList();

            foreach (var id in identifiers)
            {
                int removed = files.RemoveAll(s => s.StartsWith(id, StringComparison.OrdinalIgnoreCase));
                if (removed == 0)
                    Assert.Warn($"No Start of Game images found for {id}");
            }

            foreach (var file in files)
            {
                Assert.Warn($"{file} isn't used by any decks.");
            }
        }

        [Test]
        public void YesNoDialogs()
        {
            List<string> identifiers = new List<string>();
            foreach (var item in Items())
            {
                if (item.Kind == "Hero")
                    identifiers.AddRange(item.StartEndIdentifiers);
            }

            string expectedDirectory = Path.Combine(ArtPath, @"Cutouts\YesNoDialog");

            if (!Directory.Exists(expectedDirectory))
                Assert.Inconclusive();

            var files = Directory.GetFiles(expectedDirectory).Select(s => Path.GetFileName(s)).ToList();

            foreach (var id in identifiers)
            {
                int removed = files.RemoveAll(s => s.StartsWith("YesNoDialog" + id, StringComparison.OrdinalIgnoreCase));
                if (removed == 0)
                    Assert.Warn($"No Yes/No Dialog image found for {id}");
            }

            foreach (var file in files)
            {
                Assert.Warn($"{file} isn't used by any decks.");
            }
        }
    }
}
