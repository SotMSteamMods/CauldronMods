using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

using Cauldron.Menagerie;

namespace CauldronTests
{
    [TestFixture()]
    public class MenagerieTests : BaseTest
    {
        protected TurnTakerController menagerie { get { return FindVillain("Menagerie"); } }


        private void AssertHasKeyword(string keyword, IEnumerable<string> identifiers)
        {
            foreach (var id in identifiers)
            {
                var card = GetCard(id);
                AssertCardHasKeyword(card, keyword, false);
            }
        }

        [Test()]
        [Order(0)]
        public void TestMenagerieLoad()
        {
            SetupGameController("Cauldron.Menagerie", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            Assert.AreEqual(5, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(menagerie);
            Assert.IsInstanceOf(typeof(MenagerieCharacterCardController), menagerie.CharacterCardController);

            foreach (var card in menagerie.TurnTaker.GetAllCards())
            {
                var cc = GetCardController(card);
                Assert.IsTrue(cc.GetType() != typeof(CardController), $"{card.Identifier} is does not have a CardController");
            }

            Assert.AreEqual(null, menagerie.CharacterCard.HitPoints);
            AssertIsInPlay("PrizedCatch");
        }

        [Test()]
        public void TestMenagerieDecklist()
        {
            SetupGameController("Cauldron.Menagerie", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            AssertHasKeyword("device", new[]
            {
                "SpecimenCollector"
            });

            AssertHasKeyword("drone", new[]
            {
                "SpecimenCollector"
            });

            AssertHasKeyword("enclosure", new[]
            {
                "AquaticSphere",
                "ArborealSphere",
                "ExoticSphere",
                "ReinforcedSphere",
                "SecuritySphere"
            });

            AssertHasKeyword("insect", new[]
            {
                "MutatedAnt"
            });

            AssertHasKeyword("mercenary", new[]
            {
                "HiredKeeper",
                "TakIshmael"
            });

            AssertHasKeyword("one-shot", new[]
            {
                "IndiscriminatePoaching",
                "ViewingApertures"
            });

            AssertHasKeyword("ongoing", new[]
            {
                "FeedingTime",
                "PrizedCatch"
            });

            AssertHasKeyword("specimen", new[]
            {
                "AngryLethovore",
                "FeethsmarAlpha",
                "HalberdHive",
                "HyrianSnipe",
                "LumobatFlock",
                "TheMonBeskmaHydric"
            });

            AssertHasKeyword("test subject", new[]
            {
                "HalberdHive"
            });

            AssertHitPoints(GetCard("AngryLethovore"), 6);
            AssertHitPoints(GetCard("AquaticSphere"), 10);
            AssertHitPoints(GetCard("ArborealSphere"), 15);
            AssertHitPoints(GetCard("ExoticSphere"), 8);
            AssertHitPoints(GetCard("FeethsmarAlpha"), 4);
            AssertHitPoints(GetCard("HalberdHive"), 13);
            AssertHitPoints(GetCard("HiredKeeper"), 4);
            AssertHitPoints(GetCard("HyrianSnipe"), 4);
            AssertHitPoints(GetCard("LumobatFlock"), 3);
            AssertHitPoints(GetCard("MutatedAnt"), 5);
            AssertHitPoints(GetCard("ReinforcedSphere"), 8);
            AssertHitPoints(GetCard("SecuritySphere"), 10);
            AssertHitPoints(GetCard("SpecimenCollector"), 6);
            AssertHitPoints(GetCard("TakIshmael"), 12);
            AssertHitPoints(GetCard("TheMonBeskmaHydric"), 8);
        }

        [Test()]
        public void TestMenagerieFront()
        {
            SetupGameController("Cauldron.Menagerie", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            //At the end of the villain turn, reveal cards from the top of the villain deck until an enclosure is revealed, play it, and shuffle the other revealed cards back into the deck. Then if {H} enclosures are beneath this card, flip {Menagerie}'s character cards.
            Card sphere = PutOnDeck("AquaticSphere");
            Card hive = PutOnDeck("HalberdHive");
            GoToEndOfTurn(menagerie);
            AssertIsInPlay(sphere);
            AssertInDeck(hive);
            //Prized Catch is Indestructible.
            Card prize = FindCardInPlay("PrizedCatch");
            DestroyCard(prize);
            AssertIsInPlay(prize);
        }

        [Test()]
        public void TestMenagerieFrontEnclosureLeavesPlay()
        {
            SetupGameController("Cauldron.Menagerie", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            //When an enclosure leaves play, put it under this card, discarding all cards beneath it. Put any discarded targets into play.
            Card hive = PutOnDeck("HalberdHive");
            Card sphere = PlayCard("AquaticSphere");
            Card feeding = MoveCard(menagerie, "FeedingTime", sphere.UnderLocation);
            AssertNumberOfCardsUnderCard(sphere, 2);

            DestroyCard(sphere);
            //...discarding all cards beneath it. Put any discarded targets into play.
            AssertInTrash(feeding);
            AssertIsInPlay(hive);
            //...put it under [Menagerie]
            AssertUnderCard(menagerie.CharacterCard, sphere);
        }

        [Test()]
        public void TestAngryLethovore()
        {
            SetupGameController("Cauldron.Menagerie", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            //At the end of the villain turn, play the top card of the villain deck.",
            //Whenever a target enters play, this card deals that target 2 melee damage and regains 6HP.
        }

        [Test()]
        public void TestAquaticSphere()
        {
            SetupGameController("Cauldron.Menagerie", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            //When this card enters play, place the top card of the villain deck beneath it face down.
            Card sphere = PlayCard("AquaticSphere");
            AssertNumberOfCardsAtLocation(sphere.UnderLocation, 1);

            //Whenever a hero uses a power, they must discard a card.
            QuickHandStorage(bunker);
            UsePower(bunker);
            //+1 from power, -1 from sphere
            QuickHandCheck(0);

            QuickHandStorage(haka);
            UsePower(haka);
            QuickHandCheck(-1);

            QuickHandStorage(scholar);
            UsePower(scholar);
            QuickHandCheck(-1);
        }
    }
}
