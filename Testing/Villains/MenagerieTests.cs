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
    public class MenagerieTests : CauldronBaseTest
    {
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
        public void TestMenagerieFrontCannotPlayCards()
        {
            SetupGameController("Cauldron.Menagerie", "Legacy", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            PlayCard("TakeDown");
            GoToEndOfTurn(legacy);
            AssertNumberOfCardsAtLocation(menagerie.TurnTaker.Revealed, 0);
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
        public void TestMenagerieFrontChallenge()
        {
            SetupGameController(new string[] { "Cauldron.Menagerie", "TheSentinels", "Bunker", "TheScholar", "Megalopolis" }, challenge: true);
            DiscardAllCards(sentinels, bunker);
            PutInHand("DontDismissAnything");

            StartGame();

            PutOnDeck("ReinforcedSphere");


            //At the end of the villain turn, the captured hero and each hero next to an Enclosure deals themself X irreducible psychic damage, where X is the number of Enclosures in play.
            QuickHPStorage(writhe, mainstay, medico, idealist, bunker.CharacterCard, scholar.CharacterCard);
            GoToEndOfTurn(menagerie);

            //only the captured hero takes damage
            //1 enclosure in play, so 1 damage

            QuickHPCheck(0, 0, 0, 0, 0, -1);

        }

        [Test()]
        public void TestMenagerieBackChallenge()
        {
            SetupGameController(new string[] { "Cauldron.Menagerie", "TheSentinels", "Bunker", "TheScholar", "Megalopolis" }, challenge: true);
            StartGame();

            MoveCards(menagerie, new string[] { "AquaticSphere", "ArborealSphere", "ExoticSphere" }, menagerie.CharacterCard.UnderLocation);
            GoToEndOfTurn(menagerie);
            AssertFlipped(menagerie);
            GoToEndOfTurn(env);
            DestroyNonCharacterVillainCards();

            DecisionSelectCards = new Card[] { bunker.CharacterCard, scholar.CharacterCard };
            PlayCard("ReinforcedSphere");
            PutOnDeck("AquaticSphere");
            //At the end of the villain turn, play the top card of the villain deck. Then, for each enclosure in play, Menagerie deals the hero next to it X projectile damage, where X is the number of cards beneath that enclosure.
            //At the end of the villain turn, the captured hero and each hero next to an Enclosure deals themself X irreducible psychic damage, where X is the number of Enclosures in play.
            GoToPlayCardPhase(menagerie);
            AddCannotDealDamageTrigger(menagerie, menagerie.CharacterCard);
            QuickHPStorage(writhe, mainstay, medico, idealist, bunker.CharacterCard, scholar.CharacterCard);
            GoToEndOfTurn(menagerie);
            QuickHPCheck(0,0,0,0,-2,-2);


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
            //bad seeds: -860580707
            SetupGameController(new string[] { "Cauldron.Menagerie", "Legacy", "Ra", "Haka", "Megalopolis" });
            StartGame();
            Card prize = FindCardInPlay("PrizedCatch");

            StackDeck(new string[] { "HalberdHive", "AngryLethovore", "TheMonBeskmaHydric", "LumobatFlock" });

            //When Menagerie flips to this side, shuffle the villain trash and all enclosurese beneath this card into the villain deck. Remove Prized Catch from the game.
            IEnumerable<Card> trash = DiscardTopCards(menagerie, 4);
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
        public void TestMenagerieInitialFlip_601932491()
        {
            //bad seeds: -860580707
            SetupGameController(new string[] { "Cauldron.Menagerie", "Legacy", "Ra", "Haka", "Megalopolis" }, randomSeed: 601932491);
            StartGame();
            Card prize = FindCardInPlay("PrizedCatch");

            StackDeck(new string[] { "HalberdHive", "AngryLethovore", "TheMonBeskmaHydric", "LumobatFlock" });

            //When Menagerie flips to this side, shuffle the villain trash and all enclosurese beneath this card into the villain deck. Remove Prized Catch from the game.
            IEnumerable<Card> trash = DiscardTopCards(menagerie, 4);
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
            //bad seeds: -1265292002
            SetupGameController(new string[] { "Cauldron.Menagerie", "Legacy", "Ra", "Haka", "Megalopolis" });
            StartGame();
            MoveCards(menagerie, new string[] { "AquaticSphere", "ArborealSphere", "ExoticSphere" }, menagerie.CharacterCard.UnderLocation);
            GoToEndOfTurn(menagerie);
            AssertFlipped(menagerie);
            DestroyNonCharacterVillainCards();

            //When an enclosure enters play, move it next to the active hero with the fewest enclosures in their play area. 
            Card aqua = PlayCard("AquaticSphere");
            AssertNextToCard(aqua, legacy.CharacterCard);
            Card exo = PlayCard("ExoticSphere");
            AssertNextToCard(exo, ra.CharacterCard);

            //Heroes with enclosures in their play area may not damage cards in other play areas.
            QuickHPStorage(menagerie.CharacterCard, exo);
            DealDamage(ra, menagerie, 2, DamageType.Melee);
            DealDamage(ra, exo, 2, DamageType.Melee);
            QuickHPCheck(0, -2);
        }

        [Test()]
        public void TestMenagerieBackEnclosureLocation_Seed1265292002()
        {
            //bad seeds: -1265292002
            SetupGameController(new string[] { "Cauldron.Menagerie", "Legacy", "Ra", "Haka", "Megalopolis" }, randomSeed: 1265292002);
            StartGame();
            MoveCards(menagerie, new string[] { "AquaticSphere", "ArborealSphere", "ExoticSphere" }, menagerie.CharacterCard.UnderLocation);
            GoToEndOfTurn(menagerie);
            AssertFlipped(menagerie);
            DestroyNonCharacterVillainCards();

            //When an enclosure enters play, move it next to the active hero with the fewest enclosures in their play area. 
            Card aqua = PlayCard("AquaticSphere");
            AssertNextToCard(aqua, legacy.CharacterCard);
            Card exo = PlayCard("ExoticSphere");
            AssertNextToCard(exo, ra.CharacterCard);

            //Heroes with enclosures in their play area may not damage cards in other play areas.
            QuickHPStorage(menagerie.CharacterCard, exo);
            DealDamage(ra, menagerie, 2, DamageType.Melee);
            DealDamage(ra, exo, 2, DamageType.Melee);
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
        public void TestMenagerieBackEndTurnEffect_2Under()
        {
            SetupGameController(new string[] { "Cauldron.Menagerie", "Legacy", "Ra", "Haka", "Megalopolis" });
            StartGame();
            MoveCards(menagerie, new string[] { "AquaticSphere", "ArborealSphere", "ExoticSphere" }, menagerie.CharacterCard.UnderLocation);
            GoToEndOfTurn(menagerie);
            AssertFlipped(menagerie);
            GoToEndOfTurn(env);
            DestroyNonCharacterVillainCards();

            PutOnDeck("ExoticSphere");
            //At the end of the villain turn, play the top card of the villain deck. Then, for each enclosure in play, Menagerie deals the hero next to it X projectile damage, where X is the number of cards beneath that enclosure.
            QuickHPStorage(legacy);
            GoToEndOfTurn(menagerie);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestMenagerieBackEndTurnEffect_1Under()
        {
            SetupGameController(new string[] { "Cauldron.Menagerie", "Legacy", "Ra", "Haka", "Megalopolis" });
            StartGame();
            MoveCards(menagerie, new string[] { "AquaticSphere", "ArborealSphere", "ExoticSphere" }, menagerie.CharacterCard.UnderLocation);
            GoToEndOfTurn(menagerie);
            AssertFlipped(menagerie);
            GoToEndOfTurn(env);
            DestroyNonCharacterVillainCards();

            PutOnDeck("AquaticSphere");
            //At the end of the villain turn, play the top card of the villain deck. Then, for each enclosure in play, Menagerie deals the hero next to it X projectile damage, where X is the number of cards beneath that enclosure.
            QuickHPStorage(legacy);
            GoToEndOfTurn(menagerie);
            QuickHPCheck(-1);
        }

        [Test()]
        public void TestMenagerieBackAlternateLoss()
        {
            SetupGameController(new string[] { "Cauldron.Menagerie", "Legacy", "Ra", "Haka", "Megalopolis" }, true);
            StartGame();
            MoveCards(menagerie, new string[] { "AquaticSphere", "ArborealSphere", "ExoticSphere" }, menagerie.CharacterCard.UnderLocation);
            GoToEndOfTurn(menagerie);
            AssertFlipped(menagerie);
            DestroyNonCharacterVillainCards();

            MoveCards(menagerie, new string[] { "ExoticSphere", "AquaticSphere", "SecuritySphere" }, menagerie.TurnTaker.OffToTheSide);

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

            MoveCards(menagerie, FindCardsWhere((Card c) => c.DoKeywordsContain("enclosure")), menagerie.TurnTaker.OffToTheSide);
            Card ant = PutOnDeck("MutatedAnt");

            Card letho = PlayCard("AngryLethovore");

            DealDamage(haka, letho, 3, DamageType.Melee);
            //At the end of the villain turn, play the top card of the villain deck.
            GoToEndOfTurn(menagerie);
            AssertIsInPlay(ant);
            //Whenever a target enters play, this card deals that target 2 melee damage and regains 6HP.
            AssertHitPoints(ant, (ant.MaximumHitPoints ?? default) - 1);
            //Lethovore's max HP is 6 so it will go to max no matter what
            AssertHitPoints(letho, 6);
        }

        [Test()]
        public void TestAngryLethovore_118349059()
        {
            SetupGameController(new string[] { "Cauldron.Menagerie", "Haka", "Bunker", "TheScholar", "Megalopolis" }, randomSeed: 118349059);
            StartGame();

            MoveCards(menagerie, FindCardsWhere((Card c) => c.DoKeywordsContain("enclosure")), menagerie.TurnTaker.OffToTheSide);
            Card ant = PutOnDeck("MutatedAnt");

            Card letho = PlayCard("AngryLethovore");

            DealDamage(haka, letho, 3, DamageType.Melee);
            //At the end of the villain turn, play the top card of the villain deck.
            GoToEndOfTurn(menagerie);
            AssertIsInPlay(ant);
            //Whenever a target enters play, this card deals that target 2 melee damage and regains 6HP.
            AssertHitPoints(ant, (ant.MaximumHitPoints ?? default) - 1);
            //Lethovore's max HP is 6 so it will go to max no matter what
            AssertHitPoints(letho, 6);
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

        [Test()]
        public void TestAquaticSphereKillDamage()
        {
            SetupGameController("Cauldron.Menagerie", "TheSentinels", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            //When this card enters play, place the top card of the villain deck beneath it face down.
            Card sphere = PlayCard("AquaticSphere");
            AssertNumberOfCardsAtLocation(sphere.UnderLocation, 1);
            SetHitPoints(sphere, 2);

            //Whenever a hero uses a power, they must discard a card, but sphere destroyed, so no discard.
            QuickHandStorage(sentinels, bunker, scholar);
            UsePower(idealist);
            QuickHandCheck(0, 0, 0);
        }

        [Test()]
        public void TestArborealSphere()
        {
            SetupGameController("Cauldron.Menagerie", "Haka", "Bunker", "Parse", "Megalopolis");
            StartGame();

            Card hydric = PutOnDeck("TheMonBeskmaHydric");
            Card lumo = PutOnDeck("LumobatFlock");

            //When this card enters play, place the top card of the villain deck beneath it face down.
            QuickHPStorage(parse, haka, bunker);
            Card sphere = PlayCard("ArborealSphere");
            AssertNumberOfCardsAtLocation(sphere.UnderLocation, 1);

            //Then, play the top card of the villain deck.
            AssertIsInPlay(hydric);

            //Whenever a Specimen enters play, it deals the non-villain target with the lowest HP {H - 2} melee damage.
            QuickHPCheck(-1, 0, 0);
        }

        [Test()]
        public void TestArborealSphereTargetEnvironment()
        {
            SetupGameController(new[] { "Cauldron.Menagerie", "Haka", "Bunker", "Parse", "Megalopolis" });
            StartGame();

            Card traffic = PlayCard("TrafficPileup");
            var spec1 = GetCard("LumobatFlock");
            var spec2 = GetCard("HyrianSnipe");
            PutOnDeck(menagerie, spec1);
            PutOnDeck(menagerie, spec2);

            //When this card enters play, place the top card of the villain deck beneath it face down.
            QuickHPStorage(traffic);
            Card sphere = PlayCard("ArborealSphere");
            AssertAtLocation(spec2, sphere.UnderLocation);
            AssertNumberOfCardsAtLocation(sphere.UnderLocation, 1);

            //Then, play the top card of the villain deck.
            AssertIsInPlay(spec1);

            //Whenever a Specimen enters play, it deals the non-villain target with the lowest HP {H - 2} melee damage.
            QuickHPCheck(-1);
        }

        [Test()]
        public void TestExoticSphere()
        {
            SetupGameController("Cauldron.Menagerie", "Haka", "Bunker", "Benchmark", "Megalopolis");
            StartGame();

            PutOnDeck("AquaticSphere");
            PutOnDeck("LumobatFlock");

            //When this card enters play, place the top 2 cards of the villain deck beneath it face down.
            Card sphere = PlayCard("ExoticSphere");
            AssertNumberOfCardsAtLocation(sphere.UnderLocation, 2);

            MoveCards(menagerie, FindCardsWhere((Card c) => c.DoKeywordsContain("enclosure") && c.Location.IsDeck), menagerie.TurnTaker.OffToTheSide);
            GoToEndOfTurn(menagerie);

            //At the start of each hero's turn, this card deals the non-villain target with the highest HP {H - 1} toxic damage.
            QuickHPStorage(haka);
            GoToStartOfTurn(haka);
            QuickHPCheck(-2);

            QuickHPStorage(haka);
            GoToStartOfTurn(bunker);
            QuickHPCheck(-2);

            QuickHPStorage(bench);
            GoToStartOfTurn(bench);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestExoticSphere_1893948577()
        {
            SetupGameController(new string[] { "Cauldron.Menagerie", "Haka", "Bunker", "Benchmark", "Megalopolis" }, randomSeed: 1893948577);
            StartGame();

            PutOnDeck("AquaticSphere");
            PutOnDeck("LumobatFlock");

            //When this card enters play, place the top 2 cards of the villain deck beneath it face down.
            Card sphere = PlayCard("ExoticSphere");
            AssertNumberOfCardsAtLocation(sphere.UnderLocation, 2);

            MoveCards(menagerie, FindCardsWhere((Card c) => c.DoKeywordsContain("enclosure") && c.Location.IsDeck), menagerie.TurnTaker.OffToTheSide);
            GoToEndOfTurn(menagerie);

            //At the start of each hero's turn, this card deals the non-villain target with the highest HP {H - 1} toxic damage.
            QuickHPStorage(haka);
            GoToStartOfTurn(haka);
            QuickHPCheck(-2);

            QuickHPStorage(haka);
            GoToStartOfTurn(bunker);
            QuickHPCheck(-2);

            QuickHPStorage(bench);
            GoToStartOfTurn(bench);
            QuickHPCheck(-2);
        }

        [Test()]
        public void TestFeedingTime()
        {
            SetupGameController("Cauldron.Menagerie", "Haka", "Bunker", "Benchmark", "Megalopolis");
            DiscardAllCards(bench);
            StartGame();

            Card tak = PutInTrash("TakIshmael");
            Card keeper = PutInTrash("HiredKeeper");
            Card hive = PutInTrash("HalberdHive");

            PlayCard("FeedingTime");

            //When this card enters play, put all Mercenary cards from the villain trash into play.
            AssertIsInPlay(new Card[] { tak, keeper });
            AssertInTrash(hive);

            //Reduce damage dealt to Mercenaries by X, where X is the number of Specimens in play.
            //0 in play
            QuickHPStorage(tak);
            DealDamage(bench, tak, 2, DamageType.Melee);
            QuickHPCheck(-2);

            //1 in play
            PlayCard(hive);
            QuickHPStorage(tak);
            DealDamage(bench, tak, 2, DamageType.Melee);
            QuickHPCheck(-1);

            //2 in play
            PlayCard("FeethsmarAlpha");
            QuickHPStorage(tak);
            DealDamage(bench, tak, 3, DamageType.Melee);
            QuickHPCheck(-1);
        }

        [Test()]
        public void TestFeethsmarAlpha()
        {
            SetupGameController("Cauldron.Menagerie", "Haka", "TheSentinels", "Benchmark", "Megalopolis");
            DiscardAllCards(bench);
            StartGame();

            PlayCard("FeethsmarAlpha");

            Card hive = PlayCard("HalberdHive");
            Card aqua = PlayCard("AquaticSphere");
            Card traffic = PlayCard("TrafficPileup");
            PutOnDeck("ExoticSphere");

            //Increase damage dealt to Enclosures by 1.
            //At the end of the villain turn, this card deals each non-Specimen target 2 cold damage.
            QuickHPStorage(hive, haka.CharacterCard, bench.CharacterCard, aqua, traffic);
            GoToEndOfTurn(menagerie);
            QuickHPCheck(0, -2, -2, -3, -2);
        }

        [Test()]
        public void TestFeethsmarAlpha_596196033()
        {
            SetupGameController(new string[] { "Cauldron.Menagerie", "Haka", "TheSentinels", "Benchmark", "Megalopolis" }, randomSeed: 596196033);
            DiscardAllCards(bench);
            StartGame();

            PlayCard("FeethsmarAlpha");

            Card hive = PlayCard("HalberdHive");
            Card aqua = PlayCard("AquaticSphere");
            Card traffic = PlayCard("TrafficPileup");
            PutOnDeck("ExoticSphere");

            DecisionSelectCards = new Card[] { haka.CharacterCard, medico, mainstay, idealist, writhe, bench.CharacterCard, aqua, traffic };

            //Increase damage dealt to Enclosures by 1.
            //At the end of the villain turn, this card deals each non-Specimen target 2 cold damage.
            QuickHPStorage(hive, haka.CharacterCard, bench.CharacterCard, aqua, traffic);
            GoToEndOfTurn(menagerie);
            QuickHPCheck(0, -2, -2, -3, -2);
        }

        [Test()]
        public void TestHalberdHive()
        {
            SetupGameController("Cauldron.Menagerie", "Haka", "Parse", "Benchmark", "Megalopolis");
            DiscardAllCards(bench);
            StartGame();

            PutOnDeck("AquaticSphere");
            Card ant = PutInTrash("MutatedAnt");

            PlayCard("HalberdHive");
            //At the end of the villain turn, this card deals the hero target with the lowest HP 2 toxic damage. Then, put all Insects in the villain trash into play.
            QuickHPStorage(parse);
            GoToEndOfTurn(menagerie);
            QuickHPCheck(-2);
            AssertIsInPlay(ant);

            //Increase damage dealt by insects by 1.
            QuickHPStorage(haka);
            DealDamage(ant, haka, 2, DamageType.Melee);
            QuickHPCheck(-3);
        }

        [Test()]
        public void TestHiredKeeperEquipment()
        {
            SetupGameController(new[] { "Cauldron.Menagerie", "Parse", "Haka", "Benchmark", "Megalopolis" });
            //ensure parse is captured
            DiscardAllCards(haka, bench);
            PutInHand("Recompile");
            StartGame();

            PutOnDeck("HalberdHive");
            PutOnDeck("AngryLethovore"); //put under aquaic sphere
            PutOnDeck("AquaticSphere");
            Card mere = PlayCard("Mere");

            PlayCard("HiredKeeper");
            //At the end of the villain turn, this card deals the 2 non-Captured hero targets with the highest HP 2 sonic damage each.
            QuickHPStorage(haka, parse, bench);
            GoToEndOfTurn(menagerie);
            QuickHPCheck(-2, 0, -2);

            //Whenever a Specimen is destroyed, destroy 1 hero ongoing or equipment card.
            Card hive = PlayCard("HalberdHive");
            DestroyCard(hive);
            AssertInTrash(mere);
        }

        [Test()]
        public void TestHiredKeeperOngoing()
        {
            SetupGameController(new[] { "Cauldron.Menagerie", "Haka", "Parse", "Benchmark", "Megalopolis" });
            //ensure haka is captured
            DiscardAllCards(bench, parse);
            PutInHand("ElbowSmash");
            StartGame();

            MoveCards(menagerie, FindCardsWhere((Card c) => c.DoKeywordsContain("enclosure") && c.Location.IsDeck), menagerie.TurnTaker.OffToTheSide);

            Card moko = PlayCard("TaMoko");
            //stack deck
            PutOnDeck("HalberdHive");
            PutOnDeck("AngryLethovore"); //put under aquaic sphere
            PutOnDeck("AquaticSphere");
            PlayCard("HiredKeeper");
            //At the end of the villain turn, this card deals the 2 non-Captured hero targets with the highest HP 2 sonic damage each.
            QuickHPStorage(haka, parse, bench);
            GoToEndOfTurn(menagerie);
            QuickHPCheck(0, -2, -2);

            //Whenever a Specimen is destroyed, destroy 1 hero ongoing or equipment card.
            Card hive = PlayCard("HalberdHive");
            DestroyCard(hive);
            AssertInTrash(moko);
        }

        [Test()]
        public void TestHyrianSnipe_1029071514()
        {
            SetupGameController(new string[] { "Cauldron.Menagerie", "Haka", "Parse", "Benchmark", "Megalopolis" }, randomSeed: 1029071514);
            DiscardAllCards(bench, parse);
            StartGame();

            Card snipe = MoveCard(menagerie, "HyrianSnipe", menagerie.TurnTaker.OffToTheSide);

            MoveCards(menagerie, new string[] { "AquaticSphere", "ArborealSphere", "ExoticSphere" }, menagerie.CharacterCard.UnderLocation);
            GoToEndOfTurn(menagerie);
            DestroyNonCharacterVillainCards();

            Card moko = PlayCard("TaMoko");
            Card mere = PlayCard("Mere");

            PutOnDeck(menagerie, snipe);
            //At the end of the villain turn, this card deals the 2 targets other than itself with the highest HP {H - 1} psychic damage each. Then, destroy 1 equipment card.
            QuickHPStorage(menagerie, haka, parse, bench);
            GoToEndOfTurn(menagerie);
            QuickHPCheck(-2, -1, 0, 0);
            AssertInTrash(mere);
            AssertIsInPlay(moko);
        }

        [Test()]
        public void TestHyrianSnipe()
        {
            SetupGameController("Cauldron.Menagerie", "Haka", "Parse", "Benchmark", "Megalopolis");
            DiscardAllCards(bench, parse);
            StartGame();

            Card snipe = MoveCard(menagerie, "HyrianSnipe", menagerie.TurnTaker.OffToTheSide);

            MoveCards(menagerie, new string[] { "AquaticSphere", "ArborealSphere", "ExoticSphere" }, menagerie.CharacterCard.UnderLocation);
            GoToEndOfTurn(menagerie);
            DestroyNonCharacterVillainCards();

            Card moko = PlayCard("TaMoko");
            Card mere = PlayCard("Mere");

            PutOnDeck(menagerie, snipe);
            //At the end of the villain turn, this card deals the 2 targets other than itself with the highest HP {H - 1} psychic damage each. Then, destroy 1 equipment card.
            QuickHPStorage(menagerie, haka, parse, bench);
            GoToEndOfTurn(menagerie);
            QuickHPCheck(-2, -1, 0, 0);
            AssertInTrash(mere);
            AssertIsInPlay(moko);
        }

        [Test()]
        public void TestIndiscriminatePoaching()
        {
            SetupGameController("Cauldron.Menagerie", "Haka", "Parse", "Benchmark", "Megalopolis");
            DiscardAllCards(bench, parse);
            StartGame();

            Card aqua = PutOnDeck("AquaticSphere");
            Card snipe = PutOnDeck("HyrianSnipe");
            Card moko = PlayCard("TaMoko");
            Card mere = PlayCard("Mere");
            Card envTopCard = GetTopCardOfDeck(env);

            PlayCard("IndiscriminatePoaching");
            //Reveal cards from the top of the villain deck until an Enclosure is revealed. Put it into play. Shuffle the other revealed cards back into the villain deck.
            AssertIsInPlay(aqua);
            AssertInDeck(snipe);
            //Put the top card of the environment deck beneath the Enclosure with the highest HP and destroy {H - 2} hero ongoing and/or equipment cards.
            AssertUnderCard(aqua, envTopCard);
            AssertInTrash(mere);
            AssertIsInPlay(moko);
        }

        [Test()]
        public void TestLumobatFlock()
        {
            SetupGameController(new[] { "Cauldron.Menagerie", "Haka", "Parse", "Benchmark", "Megalopolis" });
            DiscardAllCards(bench, parse);
            StartGame();

            PutOnDeck("FeedingTime");
            Card aqua = PutOnDeck("AquaticSphere");

            var flock = PlayCard("LumobatFlock");
            AssertIsInPlay(flock);

            var snipe = PlayCard("HyrianSnipe");
            //The first time a Specimen enters play each turn, play the top card of the villain deck.
            AssertIsInPlay(aqua);
            DestroyCard(snipe);

            PutOnDeck("ExoticSphere");
            //At the end of the villain turn this card deals the hero target with the highest HP 2 projectile and 2 radiant damage.
            QuickHPStorage(haka, parse, bench);
            GoToEndOfTurn(menagerie);
            QuickHPCheck(-4, 0, 0);
        }

        [Test()]
        public void TestMutatedAnt()
        {
            SetupGameController("Cauldron.Menagerie", "Parse", "Benchmark", "Haka", "Megalopolis");
            DiscardAllCards(haka, parse);
            StartGame();

            PlayCard("TaMoko");

            Card ant = PlayCard("MutatedAnt");
            //Reduce damage dealt to this card by 1.
            //The first time a non-villain target deals damage each turn, this card deals that target 1 irreducible toxic damage.
            QuickHPStorage(haka.CharacterCard, ant);
            DealDamage(haka, ant, 2, DamageType.Melee);
            QuickHPCheck(-1, -1);

            QuickHPStorage(haka.CharacterCard, ant);
            DealDamage(haka, ant, 2, DamageType.Melee);
            QuickHPCheck(0, -1);
        }

        [Test()]
        public void TestMutatedAnt_Issue884()
        {
            SetupGameController("Cauldron.Menagerie", "Parse", "Benchmark", "Haka", "TheSentinels", "Megalopolis");
            DiscardAllCards(haka, parse);
            StartGame();

            //make all damage irreducible to simplify tests
            PlayCard("RevealTheFlaws");

            Card ant = PlayCard("MutatedAnt");
            Card tactics = PlayCard("SentinelTactics");
            DecisionSelectCards = new Card[] {tactics, ant };
            DecisionSelectPower = idealist;
            //The first time a non-villain target deals damage each turn, this card deals that target 1 irreducible toxic damage.
            QuickHPStorage(parse.CharacterCard, bench.CharacterCard, haka.CharacterCard, mainstay, writhe, idealist, medico, ant);
            DealDamage(mainstay, ant, 1, DamageType.Melee);
            QuickHPCheck(0,0,0,-1,0,0,0,-3);

        }

        [Test()]
        public void TestPrizedCatch()
        {
            SetupGameController("Cauldron.Menagerie", "Haka", "Parse", "Benchmark", "Megalopolis");
            DiscardAllCards(bench, parse);
            StartGame();

            Card aqua = PlayCard("AquaticSphere");
            //Increase damage dealt by Captured targets to Enclosures by 1. Reduce damage dealt by Captured targets to non-Enclosure targets by 1.
            QuickHPStorage(parse.CharacterCard, aqua);
            DealDamage(haka, aqua, 2, DamageType.Melee);
            DealDamage(haka, parse, 2, DamageType.Melee);
            QuickHPCheck(-1, -3);
        }

        [Test()]
        public void TestReinforcedSphere()
        {
            SetupGameController("Cauldron.Menagerie", "Haka", "Parse", "Benchmark", "Megalopolis");
            DiscardAllCards(bench, parse);
            StartGame();

            var cap = GetTopCardOfDeck(env);
            //When this card enters play, place the top card of the environment deck beneath it face down.
            Card sphere = PlayCard("ReinforcedSphere");
            AssertAtLocation(cap, sphere.UnderLocation);
            AssertNumberOfCardsAtLocation(sphere.UnderLocation, 1);

            //Reduce damage dealt by hero targets by 1.
            QuickHPStorage(sphere, parse.CharacterCard, bench.CharacterCard);
            DealDamage(parse, sphere, 2, DamageType.Melee);
            DealDamage(parse, bench, 2, DamageType.Melee);
            DealDamage(bench, parse, 2, DamageType.Melee);
            DealDamage(sphere, parse, 2, DamageType.Melee);
            QuickHPCheck(-1, -3, -1);
        }

        [Test()]
        public void TestSecuritySphere()
        {
            SetupGameController("Cauldron.Menagerie", "Bunker", "Legacy/AmericasGreatestLegacyCharacter", "TheSentinels", "Megalopolis");
            DiscardAllCards(legacy, sentinels);
            StartGame();

            Card flak = PlayCard("FlakCannon");
            Card mode = PlayCard("RechargeMode");

            //When this card enters play, place the top card of the villain deck beneath it face down and destroy {H - 2} hero ongoing cards.
            Card sphere = PlayCard("SecuritySphere");
            AssertNumberOfCardsAtLocation(sphere.UnderLocation, 1);
            AssertIsInPlay(flak);
            AssertInTrash(mode);

            //The Captured hero and their cards cannot affect or be affected by cards or effects from other hero decks.
            SetHitPoints(bunker, 17);
            SetHitPoints(legacy, 17);
            QuickHandStorage(bunker);
            QuickHPStorage(bunker);
            UsePower(medico);
            UsePower(legacy);
            QuickHandCheck(0);
            QuickHPCheck(0);
        }

        [Test()]
        public void TestSpecimenCollector()
        {
            SetupGameController("Cauldron.Menagerie", "Haka", "Parse", "Benchmark", "Megalopolis");
            DiscardAllCards(bench, parse);
            StartGame();

            Card aqua = PutOnDeck("AquaticSphere");

            PlayCard("SpecimenCollector");
            //At the end of the villain turn, this card deals the non-villain target with the second lowest HP {H} projectile damage. 
            QuickHPStorage(haka, parse, bench);
            GoToEndOfTurn(menagerie);
            QuickHPCheck(0, 0, -3);
            //Then, place the top card of the villain deck face down beneath the Enclosure with the fewest cards beneath it.
            AssertNumberOfCardsAtLocation(aqua.UnderLocation, 2);
        }

        [Test()]
        public void TestTakIshmael()
        {
            SetupGameController("Cauldron.Menagerie", "Haka", "Parse", "Guise", "Megalopolis");
            DiscardAllCards(guise, parse);
            StartGame();

            PutOnDeck("LumobatFlock");
            PutOnDeck("AquaticSphere");
            StackDeckAfterShuffle(menagerie, new string[] { "HyrianSnipe" });
            Card tak = PlayCard("TakIshmael");

            //This card is immune to damage dealt by non-hero cards.
            QuickHPStorage(tak);
            DealDamage(menagerie, tak, 2, DamageType.Melee);
            QuickHPCheck(0);

            //At the end of the villain turn, play the top card of the villain deck. Then, this card deals the hero target with the highest HP X projectile damage, where X is the number of Specimens in play.
            QuickHPStorage(haka, guise, parse);
            GoToEndOfTurn(menagerie);
            QuickHPCheck(-3, -2, 0);
        }

        [Test()]
        public void TestTheMonBeskmaHydric()
        {
            SetupGameController("Cauldron.Menagerie", "Haka", "Parse", "Benchmark", "Megalopolis");
            DiscardAllCards(bench, parse);
            StartGame();

            Card aqua = PutOnDeck("AquaticSphere");
            PlayCard("TheMonBeskmaHydric");

            //At the end of the villain turn, this card deals the Enclosure with the highest HP 4 melee damage and the hero target with the highest HP 2 melee damage.
            QuickHPStorage(haka.CharacterCard, aqua);
            GoToEndOfTurn(menagerie);
            QuickHPCheck(-2, -4);
        }

        [Test()]
        public void TestViewingApertures()
        {
            SetupGameController("Cauldron.Menagerie", "Haka", "Parse", "Benchmark", "Megalopolis");
            DiscardAllCards(bench, parse);
            StartGame();

            Card aqua1 = PutOnDeck("TakIshmael");
            var aqua = PlayCard("AquaticSphere");
            AssertAtLocation(aqua1, aqua.UnderLocation);

            Card exo2 = PutOnDeck("HyrianSnipe");
            Card exo1 = PutOnDeck("LumobatFlock");
            Card exo = PlayCard("ExoticSphere");
            AssertAtLocation(exo1, exo.UnderLocation);
            AssertAtLocation(exo2, exo.UnderLocation);

            Card hydric = PlayCard("TheMonBeskmaHydric");
            Card traffic = PlayCard("TrafficPileup");

            Card feed = PutOnDeck("FeedingTime");
            QuickHPStorage(haka.CharacterCard, parse.CharacterCard, bench.CharacterCard, hydric, traffic);
            PlayCard("ViewingApertures");
            //Play the top card of the villain deck.
            AssertIsInPlay(feed);

            //Select 1 face down card beneath each Enclosure. Flip those cards face up.
            AssertNotFlipped(aqua1);
            AssertNotFlipped(exo2);
            AssertFlipped(exo1);

            //{Menagerie} deals each hero, environment, and Specimen target 1 psychic damage.
            QuickHPCheck(-1, -1, -1, -1, -1);
        }
        [Test]
        public void TestEnclosureUnderCardTriggers()
        {
            SetupGameController("Cauldron.Menagerie", "Parse", "Benchmark", "Haka", "Megalopolis");
            DiscardAllCards(haka);
            StartGame();

            FlipCard(menagerie);
            Card reinforced = PutOnDeck("ReinforcedSphere");
            Card aqua = PlayCard("AquaticSphere");
            Card tak = PlayCard("TakIshmael");
            AssertUnderCard(aqua, reinforced);

            QuickHPStorage(tak);
            DealDamage(haka, tak, 1, DamageType.Melee);
            QuickHPCheck(-1);

            Card hive = PutOnDeck("HalberdHive");
            PlayCard("ViewingApertures");

            DealDamage(haka, tak, 1, DamageType.Melee);
            QuickHPCheck(-1);

            DestroyCard(aqua);
            AssertInTrash(reinforced);
            DealDamage(haka, tak, 1, DamageType.Melee);
            QuickHPCheck(-1);
        }
    }
}
