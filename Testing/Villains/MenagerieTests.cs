using Cauldron.Menagerie;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

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
        public void TestMenagerieFrontEnclosureDestroyActionLeavesPlay()
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
        public void TestMenagerieFrontEnclosureMoveActionLeavesPlay()
        {
            SetupGameController("Cauldron.Menagerie", "Haka", "Tempest", "TheScholar", "Megalopolis");
            StartGame();

            //When an enclosure leaves play, put it under this card, discarding all cards beneath it. Put any discarded targets into play.
            Card hive = PutOnDeck("HalberdHive");
            Card sphere = PlayCard("AquaticSphere");
            Card feeding = MoveCard(menagerie, "FeedingTime", sphere.UnderLocation);
            AssertNumberOfCardsUnderCard(sphere, 2);

            PlayCard("IntoTheStratosphere");
            //...discarding all cards beneath it. Put any discarded targets into play.
            AssertInTrash(feeding);
            AssertIsInPlay(hive);
            //...put it under [Menagerie]
            AssertUnderCard(menagerie.CharacterCard, sphere);
        }

        [Test()]
        public void TestMeagerieFrontAlternateLose()
        {
            SetupGameController("Cauldron.Menagerie", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            //The heroes lose if the captured hero is incapacitated.
            Card prize = FindCardInPlay("PrizedCatch");
            Card captured = prize.Location.OwnerCard;
            DealDamage(menagerie, captured, 100, DamageType.Melee);
            AssertGameOver(EndingResult.AlternateDefeat);
        }

        [Test()]
        public void TestMeagerieFrontAlternateLose_TheSentinels()
        {
            SetupGameController("Cauldron.Menagerie", "TheSentinels", "Bunker", "TheScholar", "Megalopolis");
            DiscardAllCards(new HeroTurnTakerController[] { bunker, scholar });
            StartGame();

            //The heroes lose if the captured hero is incapacitated.
            Card prize = FindCardInPlay("PrizedCatch");
            DealDamage(menagerie, idealist, 100, DamageType.Melee);
            AssertNotGameOver();
            DealDamage(menagerie, mainstay, 100, DamageType.Melee);
            AssertNotGameOver();
            DealDamage(menagerie, medico, 100, DamageType.Melee);
            AssertNotGameOver();
            DealDamage(menagerie, writhe, 100, DamageType.Melee);
            AssertGameOver(EndingResult.AlternateDefeat);
        }

        [Test()]
        public void TestMenagerieFrontAdvanced()
        {
            SetupGameController(new string[] { "Cauldron.Menagerie", "TheSentinels", "Bunker", "TheScholar", "Megalopolis" }, true);
            StartGame();

            //When an enclosure enters play, put the top card of the villain deck beneath it.
            Card sphere = PlayCard("AquaticSphere");
            //1 for when its normally played and 1 for advanced
            AssertNumberOfCardsUnderCard(sphere, 2);
        }

        [Test()]
        public void TestMenagerieFlip()
        {
            SetupGameController(new string[] { "Cauldron.Menagerie", "Legacy", "Ra", "Haka", "Bunker", "TheScholar", "Megalopolis" });
            StartGame();

            //At the end of the villain turn...if {H} enclosures are beneath this card, flip {Menagerie}'s character cards.
            MoveCards(menagerie, new string[] { "AquaticSphere", "ArborealSphere" }, menagerie.CharacterCard.UnderLocation);
            GoToEndOfTurn(menagerie);
            //2 under
            AssertNotFlipped(menagerie);

            MoveCard(menagerie, "ExoticSphere", menagerie.CharacterCard.UnderLocation);
            GoToEndOfTurn(menagerie);
            //3 under
            AssertNotFlipped(menagerie);

            MoveCard(menagerie, "ReinforcedSphere", menagerie.CharacterCard.UnderLocation);
            GoToEndOfTurn(menagerie);
            //4 under
            AssertNotFlipped(menagerie);

            MoveCard(menagerie, "SecuritySphere", menagerie.CharacterCard.UnderLocation);
            GoToEndOfTurn(menagerie);
            //5 under
            AssertFlipped(menagerie);
            AssertMaximumHitPoints(menagerie.CharacterCard, 50);
        }

        [Test()]
        public void TestMenagerieFlip3H()
        {
            SetupGameController(new string[] { "Cauldron.Menagerie", "Legacy", "Ra", "Haka", "Megalopolis" });
            StartGame();

            //At the end of the villain turn...if {H} enclosures are beneath this card, flip {Menagerie}'s character cards.
            MoveCards(menagerie, new string[] { "AquaticSphere", "ArborealSphere" }, menagerie.CharacterCard.UnderLocation);
            GoToEndOfTurn(menagerie);
            //2 under
            AssertNotFlipped(menagerie);

            MoveCard(menagerie, "ExoticSphere", menagerie.CharacterCard.UnderLocation);
            GoToEndOfTurn(menagerie);
            //3 under
            AssertFlipped(menagerie);
            AssertMaximumHitPoints(menagerie.CharacterCard, 50);
        }

        [Test()]
        public void TestMenagerieInitialFlip()
        {
            SetupGameController(new string[] { "Cauldron.Menagerie", "Legacy", "Ra", "Haka", "Megalopolis" });
            StartGame();
            Card prize = FindCardInPlay("PrizedCatch");

            //When Menagerie flips to this side, shuffle the villain trash and all enclosurese beneath this card into the villain deck. Remove Prized Catch from the game.
            IEnumerable<Card> trash = DiscardTopCards(menagerie, 6);
            string[] spheres = new string[] { "AquaticSphere", "ArborealSphere", "ExoticSphere" };
            MoveCards(menagerie, spheres, menagerie.CharacterCard.UnderLocation);
            GoToEndOfTurn(menagerie);
            AssertFlipped(menagerie);

            AssertNotInTrash(trash.ToArray());
            foreach (string sphere in spheres)
            {
                AssertNotInTrash(menagerie, sphere);
            }
            AssertNumberOfCardsUnderCard(menagerie.CharacterCard, 0);
            AssertOutOfGame(prize);
        }

        [Test()]
        public void TestMenagerieBackEnclosureLocation()
        {
            SetupGameController(new string[] { "Cauldron.Menagerie", "Legacy", "Ra", "Haka", "Megalopolis" });
            StartGame();
            MoveCards(menagerie, new string[] { "AquaticSphere", "ArborealSphere", "ExoticSphere" }, menagerie.CharacterCard.UnderLocation);
            GoToEndOfTurn(menagerie);
            AssertFlipped(menagerie);
            DestroyNonCharacterVillainCards();

            //When an enclosure enters play, move it next to the active hero with the fewest enclosures in their play area. 
            Card aqua = PlayCard("AquaticSphere");
            AssertNextToCard(aqua, legacy.CharacterCard);
            Card arb = PlayCard("ArborealSphere");
            AssertNextToCard(arb, ra.CharacterCard);

            //Heroes with enclosures in their play area may not damage cards in other play areas.
            QuickHPStorage(menagerie.CharacterCard, arb);
            DealDamage(ra, menagerie, 2, DamageType.Melee);
            DealDamage(ra, arb, 2, DamageType.Melee);
            QuickHPCheck(0, -2);
        }

        [Test()]
        public void TestMenagerieBackEnclosureLeavesPlayDestroy()
        {
            SetupGameController(new string[] { "Cauldron.Menagerie", "Legacy", "Ra", "Haka", "Megalopolis" });
            StartGame();
            MoveCards(menagerie, new string[] { "AquaticSphere", "ArborealSphere", "ExoticSphere" }, menagerie.CharacterCard.UnderLocation);
            GoToEndOfTurn(menagerie);
            AssertFlipped(menagerie);
            DestroyNonCharacterVillainCards();

            Card letho = PutOnDeck("AngryLethovore");
            //Cards beneath enclosures are not considered in play. When an enclosure leaves play, discard all cards beneath it.
            Card aqua = PlayCard("AquaticSphere");
            AssertUnderCard(aqua, letho);

            DestroyCard(aqua);
            AssertInTrash(letho);
        }

        [Test()]
        public void TestMenagerieBackEnclosureLeavesPlayMove()
        {
            SetupGameController(new string[] { "Cauldron.Menagerie", "Legacy", "Tempest", "Haka", "Megalopolis" });
            StartGame();
            MoveCards(menagerie, new string[] { "AquaticSphere", "ArborealSphere", "ExoticSphere" }, menagerie.CharacterCard.UnderLocation);
            GoToEndOfTurn(menagerie);
            AssertFlipped(menagerie);
            DestroyNonCharacterVillainCards();

            Card letho = PutOnDeck("AngryLethovore");
            //Cards beneath enclosures are not considered in play. When an enclosure leaves play, discard all cards beneath it.
            Card aqua = PlayCard("AquaticSphere");
            AssertUnderCard(aqua, letho);

            DecisionSelectCard = aqua;

            PlayCard("IntoTheStratosphere");
            AssertInTrash(letho);
        }

        [Test()]
        public void TestMenagerieBackEndTurnEffect()
        {
            SetupGameController(new string[] { "Cauldron.Menagerie", "Legacy", "Ra", "Haka", "Megalopolis" });
            StartGame();
            MoveCards(menagerie, new string[] { "AquaticSphere", "ArborealSphere", "ExoticSphere" }, menagerie.CharacterCard.UnderLocation);
            GoToEndOfTurn(menagerie);
            AssertFlipped(menagerie);
            DestroyNonCharacterVillainCards();

            PlayCard("ExoticSphere");
            //At the end of the villain turn, play the top card of the villain deck. Then, for each enclosure in play, Menagerie deals the hero next to it X projectile damage, where X is the number of cards beneath that enclosure.
            Card aqua = PutOnDeck("AquaticSphere");
            GoToEndOfTurn(env);
            QuickHPStorage(legacy, ra);
            GoToEndOfTurn(menagerie);
            QuickHPCheck(-2, -1);
        }

        [Test()]
        public void TestMenagerieBackAlternateLoss()
        {
            SetupGameController(new string[] { "Cauldron.Menagerie", "Legacy", "Ra", "Haka", "Megalopolis" });
            StartGame();
            MoveCards(menagerie, new string[] { "AquaticSphere", "ArborealSphere", "ExoticSphere" }, menagerie.CharacterCard.UnderLocation);
            GoToEndOfTurn(menagerie);
            AssertFlipped(menagerie);
            DestroyNonCharacterVillainCards();

            PlayCard("ExoticSphere");
            PlayCard("AquaticSphere");
            PlayCard("SecuritySphere");
            GoToStartOfTurn(menagerie);
            AssertGameOver(EndingResult.AlternateDefeat);
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
