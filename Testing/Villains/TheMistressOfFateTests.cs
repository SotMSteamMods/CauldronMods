using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using Handelabra.Sentinels.UnitTest;
using System.Reflection;
using Handelabra;
using System.Collections.Generic;
using Cauldron.TheMistressOfFate;

namespace CauldronTests
{
    class TheMistressOfFateTests : CauldronBaseTest
    {
        #region FateHelperFunctions
        private void ResetDays()
        {
            var days = FindCardsWhere((Card c) => c.IsInPlay && c.Definition.Keywords.Contains("day"));
            foreach (Card day in days)
            {
                if (day.IsInPlayAndHasGameText)
                {
                    var dayCC = FindCardController(day) as DayCardController;
                    FlipCard(day);
                    if (dayCC.storedCard != null)
                    {
                        MoveCard(fate, dayCC.storedCard, fate.TurnTaker.Deck);
                    }
                    dayCC.ClearStoredCard();
                }
            }
            var statusEffects = GameController.Game.StatusEffects.ToList();
            foreach (StatusEffect effect in statusEffects)
            {
                GameController.ExhaustCoroutine(GameController.ExpireStatusEffect(effect, fate.CharacterCardController.GetCardSource()));
            }
        }
       

        private DamageType DTM = DamageType.Melee;
        private string MessageTerminator = "There should have been no other messages.";
        protected void CheckFinalMessage()
        {
            GameController.ExhaustCoroutine(GameController.SendMessageAction(MessageTerminator, Priority.High, null));
        }

        protected List<Card> DayCardsInOrder => GetCards("DayOfSaints", "DayOfSinners", "DayOfSorrows", "DayOfSwords")
                                                    .OrderBy(c => c.PlayIndex).ToList();

        protected void ResetRFGCards()
        {
            var facedownCards = fate.TurnTaker.GetCardsWhere((Card c) => c.IsFaceDownNonCharacter && c.IsOutOfGame);
            foreach(Card rfg in facedownCards)
            {
                GameController.ExhaustCoroutine(GameController.FlipCard(FindCardController(rfg)));
            }
        }
        #endregion
        [Test]
        public void TestMistressOfFateLoads()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(fate);
            Assert.IsInstanceOf(typeof(TheMistressOfFateCharacterCardController), fate.CharacterCardController);

            Assert.AreEqual(88, fate.CharacterCard.HitPoints);
        }
        [Test]
        public void TestMistressOfFateDecklist()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Megalopolis");

            AssertHasKeyword("day", new string[]
            {
                "DayOfSaints",
                "DayOfSinners",
                "DayOfSorrows",
                "DayOfSwords"
            });

            AssertHasKeyword("one-shot", new string[] {
                "CantFightFate",
                "FadingRealities",
                "TangledWeft",
                "ToDust"
            });
            AssertHasKeyword("ongoing", new string[] {
                "IllusionOfFreeWill",
                "MemoryOfTomorrow",
                "NecessaryCorrection",
                "SameTimeAndPlace",
                "SeeThePattern",
                "StolenFuture"
            });
            AssertHasKeyword("anomaly", new string[] {
                "HourDevourer",
                "ResidualMalus",
                "WarpedMalus"
            });
            AssertHasKeyword("creature", new string[] {
                "ChaosButterfly"
            });
        }

        [Test]
        public void TestMistressOfFateSetsUp()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Haka", "Megalopolis");
            StartGame();
            ResetDays();

            AssertIsInPlay("TheTimeline");
            var days = new Card[] { GetCard("DayOfSaints"), GetCard("DayOfSinners"), GetCard("DayOfSorrows"), GetCard("DayOfSwords") };
            AssertIsInPlay(days);
            foreach (Card day in days)
            {
                AssertDoesNotHaveGameText(day);
            }
        }

        [Test]
        public void TestMistressOfFatePreservesIncappedHero()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Haka", "Megalopolis");
            StartGame();

            Card fortitude = PutInHand("Fortitude");
            Card surge = PutInTrash("SurgeOfStrength");
            Card ring = PutOnDeck("TheLegacyRing");
            Card presence = PutIntoPlay("InspiringPresence");

            DealDamage(fate, legacy, 50, DamageType.Melee);
            AssertIncapacitated(legacy);
            FlipCard(fate);
            AssertInHand(fortitude);
            AssertInDeck(ring);
            AssertInTrash(surge);
            AssertInTrash(presence);
        }

        [Test]
        public void TestMistressOfFateRestoresIncappedHero()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Haka", "Megalopolis");
            StartGame();

            Card fortitude = PutInHand("Fortitude");
            Card surge = PutIntoPlay("SurgeOfStrength");
            Card ring = PutOnDeck("TheLegacyRing");
            Card presence = PutInTrash("InspiringPresence");

            DealDamage(fate, legacy, 50, DamageType.Melee);

            var list = new List<UnincapacitateHeroAction>();
            var coroutine = GameController.UnincapacitateHero(legacy.CharacterCardController, legacy.CharacterCard.Definition.HitPoints ?? 10, null, list, cardSource: fate.CharacterCardController.GetCardSource());
            GameController.ExhaustCoroutine(coroutine);

            AssertInHand(fortitude);
            AssertInTrash(surge, presence);
            AssertOnTopOfDeck(ring);
        }

        [Test]
        public void TestMistressOfFateContinuesWithAllHeroesIncapped()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Haka", "Megalopolis");
            StartGame();

            DealDamage(fate, legacy, 50, DamageType.Melee);
            DealDamage(fate, ra, 50, DamageType.Melee);
            DealDamage(fate, haka, 50, DamageType.Melee);

            AssertNotGameOver();
        }
        [Test]
        public void TestMistressOfFateVillainDamageImmunity()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Haka", "Megalopolis");
            StartGame();
            ResetDays();

            Card butterfly = PlayCard("ChaosButterfly");
            Card traffic = PlayCard("TrafficPileup");
            QuickHPStorage(fate.CharacterCard, ra.CharacterCard, butterfly, traffic);

            DealDamage(fate, fate, 1, DTM);
            DealDamage(butterfly, fate, 2, DTM);
            DealDamage(ra, fate, 4, DTM);
            DealDamage(traffic, fate, 8, DTM);
            QuickHPCheck(-12, 0, 0, 0);

            DealDamage(fate, butterfly, 1, DTM);
            DealDamage(fate, ra, 1, DTM);
            DealDamage(fate, traffic, 1, DTM);
            QuickHPCheck(0, -1, -1, -1);

        }
        [Test]
        public void TestMistressOfFateSpecialLossCondition()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Haka", "Megalopolis");
            StartGame();
            ResetDays();

            MoveCards(fate, fate.TurnTaker.Deck.Cards, fate.TurnTaker.Trash);
            AssertGameOver();
        }
        [Test]
        public void TestMistressOfFateFlipRoutine()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "TheVisionary", "Megalopolis");
            StartGame();
            ResetDays();

            Card malus = PutOnDeck("WarpedMalus");
            Card sorrows = GetCard("DayOfSorrows");
            FlipCard(sorrows);

            Card decoy = PlayCard("DecoyProjection");
            SetHitPoints(decoy, 1);
            SetHitPoints(legacy, 15);
            DealDamage(fate, ra, 50, DTM);

            Card future = PutInTrash("StolenFuture");
            Card butterfly = PutOnDeck("ChaosButterfly");
            Card timePlace = PutOnDeck("SameTimeAndPlace");

            Card traffic = PlayCard("TrafficPileup");
            Card police = PlayCard("PoliceBackup");

            SetHitPoints(fate, 50);
            FlipCard(fate);

            //days reclaim their cards
            AssertUnderCard(sorrows, malus);
            //everything gets set to full HP, except Mistress
            AssertHitPoints(decoy, 5);
            AssertHitPoints(legacy, legacy.CharacterCard.MaximumHitPoints ?? 0);
            AssertHitPoints(fate, 50);
            //environment is destroyed
            AssertInTrash(traffic, police);
            //day cards are flipped face down
            AssertFlipped(sorrows);
            //trash and top H-1 cards of deck are RFG, deck cards face-down
            AssertOutOfGame(future, butterfly, timePlace);
            AssertFlipped(butterfly, timePlace);
            AssertNotFlipped(future);
            //incapped heroes are restored
            AssertNotIncapacitatedOrOutOfGame(ra);
            //flips back
            AssertNotFlipped(fate);

            AssertNumberOfCardsInTrash(fate, 0);
        }

        [Test]
        public void TestMistressOfFateFlipRoutine_DualDrift()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Cauldron.Drift/DualDriftCharacter", "Ra", "TheVisionary", "Megalopolis");
            Card track = FindCardsWhere((Card c) => c.Identifier == "DualShiftTrack1", false).FirstOrDefault();
            Card past = FindCardsWhere((Card c) => c.Identifier == "PastDriftCharacter").FirstOrDefault();
            Card future = FindCardsWhere((Card c) => c.Identifier == "FutureDriftCharacter").FirstOrDefault();


            DecisionSelectCards = new Card[] { track, past };
            StartGame();

            ResetDays();

            DealDamage(fate, future, 50, DTM);

            FlipCard(fate);

            AssertIsAtMaxHP(future);
            AssertInPlayArea(drift, track);
        }

        [Test]
        public void TestMistressOfFateFlipRoutine_Challenge()
        {
            SetupGameController(new string[] { "Cauldron.TheMistressOfFate", "Legacy", "Ra", "TheVisionary", "Megalopolis" }, challenge: true);
            StartGame();
            ResetDays();

            Card malus = PutOnDeck("WarpedMalus");
            Card sorrows = GetCard("DayOfSorrows");
            FlipCard(sorrows);

            Card decoy = PlayCard("DecoyProjection");
            SetHitPoints(decoy, 1);
            SetHitPoints(legacy, 15);
            DealDamage(fate, ra, 50, DTM);

            Card future = PutInTrash("StolenFuture");
            Card butterfly = PutOnDeck("ChaosButterfly");
            Card timePlace = PutOnDeck("SameTimeAndPlace");

            Card traffic = PlayCard("TrafficPileup");
            Card police = PlayCard("PoliceBackup");

            SetHitPoints(fate, 50);

            // there are 3 face down day cards
            // there are 3 Heroes
            // the top 5 cards of the deck should be remove from the game
            IEnumerable<Card> topCardsToRemove = fate.TurnTaker.Deck.GetTopCards(5);
            IEnumerable<Card> cardsThatShouldBeFlipped = topCardsToRemove.ToList().GetRange(3, 2);
            IEnumerable<Card> cardsThatShouldNotBeFlipped = topCardsToRemove.ToList().GetRange(0, 3);
            FlipCard(fate);

            AssertOutOfGame(topCardsToRemove);

            //days reclaim their cards
            AssertUnderCard(sorrows, malus);
            //everything gets set to full HP, except Mistress
            AssertHitPoints(decoy, 5);
            AssertHitPoints(legacy, legacy.CharacterCard.MaximumHitPoints ?? 0);
            AssertHitPoints(fate, 50);
            //environment is destroyed
            AssertInTrash(traffic, police);
            //day cards are flipped face down
            AssertFlipped(sorrows);
            //trash and top H-1 cards of deck are RFG, deck cards face-down
            AssertOutOfGame(future, butterfly, timePlace);
            AssertFlipped(cardsThatShouldBeFlipped.ToArray());
            AssertNotFlipped(cardsThatShouldNotBeFlipped.ToArray());
            //incapped heroes are restored
            AssertNotIncapacitatedOrOutOfGame(ra);
            //flips back
            AssertNotFlipped(fate);

            //trash should be empty
            AssertNumberOfCardsInTrash(fate, 0);
        }
        [Test]
        public void TestMistressOfFateAdvancedEndOfTurnDamage()
        {
            SetupGameController(new string[] { "Cauldron.TheMistressOfFate", "Legacy", "Ra", "Tempest", "Megalopolis" }, advanced: true);
            StartGame();
            ResetDays();
            FlipCard(fate);

            QuickHPStorage(legacy, ra, tempest);
            GoToEndOfTurn();
            QuickHPCheck(0, 0, -3);
        }
        [Test]
        public void TestMistressOfFateAdvancedFlipGainHP()
        {
            SetupGameController(new string[] { "Cauldron.TheMistressOfFate", "Legacy", "Ra", "Tempest", "Megalopolis" }, advanced: true);
            StartGame();

            SetHitPoints(fate, 50);
            QuickHPStorage(fate);
            FlipCard(fate);
            QuickHPCheck(10);
        }
        [Test]
        public void TestTheTimelineDayCardsNotAffectedByHeroCards()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();
            ResetDays();

            PlayCard("IntoTheStratosphere");
            AssertNumberOfCardsInPlay(fate, 6);
        }
        [Test]
        public void TestTheTimelineEndOfEnvironmentFlipAllHeroesIncapped()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Haka", "Megalopolis");
            StartGame();
            ResetDays();

            GoToStartOfTurn(FindEnvironment());

            DealDamage(fate, legacy, 50, DamageType.Melee);
            DealDamage(fate, ra, 50, DamageType.Melee);
            DealDamage(fate, haka, 50, DamageType.Melee);

            DecisionYesNo = false;
            GoToStartOfTurn(fate);
            AssertNumberOfCardsAtLocation(fate.TurnTaker.OutOfGame, 2);
        }
        [Test]
        public void TestTheTimelineEndOfEnvironmentFlipAllDaysFaceUp()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();
            ResetDays();

            GoToStartOfTurn(FindEnvironment());
            var days = FindCardsWhere((Card c) => c.IsInPlay && c.IsFaceDownNonCharacter);
            foreach (Card day in days)
            {
                FlipCard(day);
            }
            DecisionYesNo = false;
            GoToStartOfTurn(fate);

            AssertNumberOfCardsAtLocation(fate.TurnTaker.OutOfGame, 2);
        }
        [Test]
        public void TestTheTimelineEndOfEnvironmentFlipMakeDecision()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();
            ResetDays();

            DecisionYesNo = true;
            GoToStartOfTurn(fate);

            AssertNumberOfCardsAtLocation(fate.TurnTaker.OutOfGame, 2);
        }
        [Test]
        public void TestTheTimelineEndOfEnvironmentFlipMakeDecisionDecline()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();
            ResetDays();

            DecisionYesNo = false;
            GoToStartOfTurn(fate);

            AssertNumberOfCardsAtLocation(fate.TurnTaker.OutOfGame, 0);
        }
        [Test]
        public void TestDayOfSaintsFlipFaceUp()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();
            ResetDays();
            FlipCard(fate);

            QuickHPStorage(fate, legacy, ra, tempest);
            FlipCard(GetCard("DayOfSaints"));
            //3 * 3 for the raw damage, +2 for the boost, total 11 damage
            QuickHPCheck(0, 0, 0, -11);

            DealDamage(fate, legacy, 1, DamageType.Melee);
            QuickHPCheck(0, -3, 0, 0);

            DealDamage(legacy, fate, 1, DamageType.Melee);
            QuickHPCheck(-1, 0, 0, 0);
        }
        [Test]
        public void TestDayOfSinnersFlipFaceUp()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();
            ResetDays();

            Card day = GetCard("DayOfSinners");
            Card oneshot = PutOnDeck("ToDust");
            Card anomaly = PutOnDeck("WarpedMalus");
            Card ongoing = PutOnDeck("SameTimeAndPlace");
            Card creature = PutOnDeck("ChaosButterfly");

            FlipCard(day);
            AssertUnderCard(day, oneshot);
            AssertInDeck(anomaly);
            AssertInDeck(ongoing);
            AssertInDeck(creature);

        }
        [Test]
        public void TestDayOfSinnersSpecialStrings()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();
            ResetDays();

            Card day = GetCard("DayOfSinners");
            Card oneshot = PutOnDeck("ToDust");

            FlipCard(day);
            AssertCardSpecialString(day, 0, "On this day, To Dust recurs.");
            AssertCardSpecialString(oneshot, 1, "This card recurs on the Day of Sinners.");
        }
        [Test]
        public void TestDayOfSorrowsFlipFaceUp()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();
            ResetDays();

            Card day = GetCard("DayOfSorrows");
            Card anomaly = PutOnDeck("WarpedMalus");
            Card oneshot = PutOnDeck("ToDust");
            Card ongoing = PutOnDeck("SameTimeAndPlace");
            Card creature = PutOnDeck("ChaosButterfly");

            FlipCard(day);
            AssertIsInPlay(anomaly);
            AssertInDeck(oneshot);
            AssertInDeck(ongoing);
            AssertInDeck(creature);

            DestroyCard("WarpedMalus");
            AssertUnderCard(day, anomaly);
        }
        [Test]
        public void TestDayOfSorrowsSpecialStrings()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();
            ResetDays();

            Card day = GetCard("DayOfSorrows");
            Card anomaly = PutOnDeck("WarpedMalus");

            FlipCard(day);
            AssertCardSpecialString(anomaly, 0, "This card recurs on the Day of Sorrows.");
            AssertCardSpecialString(day, 0, "On this day, Warped Malus recurs.");

            DestroyCard(anomaly);
            AssertCardSpecialString(anomaly, 1, "This card recurs on the Day of Sorrows.");
        }
        [Test]
        public void TestDayOfSwordsFlipFaceUpGrabOneShot()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();
            ResetDays();

            Card day = GetCard("DayOfSwords");
            Card anomaly = PutOnDeck("WarpedMalus");
            Card oneshot = PutOnDeck("ToDust");
            Card ongoing = PutOnDeck("SameTimeAndPlace");
            Card creature = PutOnDeck("ChaosButterfly");

            FlipCard(day);
            AssertInDeck(anomaly);
            AssertInDeck(ongoing);
            AssertInDeck(creature);

            AssertUnderCard(day, oneshot);
        }

        [Test]
        public void TestDayOfSwordsFlipFaceUp_NoOneShotOrAnomaly()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();
            ResetDays();

            Card day = GetCard("DayOfSwords");
            IEnumerable<Card> toTrash = FindCardsWhere(c => fate.TurnTaker.Deck.HasCard(c) && (c.IsOneShot || c.IsAnomaly));
            PutInTrash(toTrash);
            AssertNextMessage("The Day of Swords dawns, but could not find any anomaly or one-shot cards!");
            FlipCard(day);
            
        }
        [Test]
        public void TestDayOfSwordsFlipFaceUpGrabAnomaly()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();
            ResetDays();

            Card day = GetCard("DayOfSwords");
            Card oneshot = PutOnDeck("ToDust");
            Card anomaly = PutOnDeck("WarpedMalus");
            Card ongoing = PutOnDeck("SameTimeAndPlace");
            Card creature = PutOnDeck("ChaosButterfly");

            FlipCard(day);
            AssertIsInPlay(anomaly);
            AssertInDeck(oneshot);
            AssertInDeck(ongoing);
            AssertInDeck(creature);

            DestroyCard(anomaly);
            AssertUnderCard(day, anomaly);
        }
        [Test]
        public void TestDayOfSwordsSpecialStrings()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();
            ResetDays();

            Card day = GetCard("DayOfSwords");
            Card anomaly = PutOnDeck("WarpedMalus");

            FlipCard(day);
            AssertCardSpecialString(anomaly, 0, "This card recurs on the Day of Swords.");
            AssertCardSpecialString(day, 0, "On this day, Warped Malus recurs.");

            DestroyCard(anomaly);
            AssertCardSpecialString(anomaly, 1, "This card recurs on the Day of Swords.");
        }

        [Test]
        public void TestCantFightFateNoViableDiscard()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();
            FlipCard(fate);
            ResetDays();
            ResetRFGCards();

            PutOnDeck(legacy, legacy.HeroTurnTaker.Hand.Cards);
            QuickHPStorage(legacy);
            AssertNextMessages("Legacy does not have 3 cards that share a keyword in their hand!", MessageTerminator);

            PlayCard("CantFightFate");
            QuickHPCheck(-20);
            CheckFinalMessage();
        }
        [Test]
        public void TestCantFightFateDiscardThree()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();
            FlipCard(fate);
            ResetDays();
            ResetRFGCards();

            PutOnDeck(legacy, legacy.HeroTurnTaker.Hand.Cards);
            QuickHPStorage(legacy);

            Card ring = PutInHand("TheLegacyRing");
            Card fort = PutInHand("Fortitude");
            Card surge = PutInHand("SurgeOfStrength");
            Card thokk = PutInHand("Thokk");

            PlayCard("CantFightFate");
            QuickHPCheck(0);
            AssertInTrash(ring, fort, surge);
            AssertInHand(thokk);
        }
        [Test]
        public void TestCantFightFateSkipDiscard()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();
            FlipCard(fate);
            ResetRFGCards();
            ResetDays();

            PutOnDeck(legacy, legacy.HeroTurnTaker.Hand.Cards);
            QuickHPStorage(legacy);

            Card ring = PutInHand("TheLegacyRing");
            Card fort = PutInHand("Fortitude");
            Card surge = PutInHand("SurgeOfStrength");
            Card thokk = PutInHand("Thokk");

            DecisionSelectWordSkip = true;
            PlayCard("CantFightFate");
            QuickHPCheck(-20);
            AssertInHand(ring, fort, surge, thokk);
        }
        [Test]
        public void TestFadingRealitiesNoRemovalPossible()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();
            FlipCard(fate);
            ResetRFGCards();
            ResetDays();

            PutOnDeck(ra, ra.HeroTurnTaker.Hand.Cards);
            QuickHPStorage(ra);

            AssertNextMessages("Ra has no cards in their hand or in play to select!", MessageTerminator);
            AssertNoDecision();

            PlayCard("FadingRealities");
            QuickHPCheck(-20);
            CheckFinalMessage();
        }
        [Test]
        public void TestFadingRealitiesRemoveCards()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();
            FlipCard(fate);
            ResetRFGCards();
            ResetDays();

            PutOnDeck(ra, ra.HeroTurnTaker.Hand.Cards);
            QuickHPStorage(ra);

            Card staff = PutInHand("TheStaffOfRa");
            Card flesh = PlayCard("FleshOfTheSunGod");

            PlayCard("FadingRealities");
            QuickHPCheck(0);
            AssertOutOfGame(flesh, staff);
        }
        [Test]
        public void TestFadingRealitiesRemoveNotEnough()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();
            FlipCard(fate);
            ResetRFGCards();
            ResetDays();

            QuickHPStorage(ra);
            PlayCard("TheStaffOfRa");
            PutOnDeck(ra, ra.HeroTurnTaker.Hand.Cards);
            DecisionYesNo = true;

            PlayCard("FadingRealities");
            QuickHPCheck(-20);
            AssertNumberOfCardsAtLocation(ra.TurnTaker.OutOfGame, 1);
        }
        [Test]
        public void TestHourDevourerDamageOneDay()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();
            ResetDays();
            FlipCard(fate);
            ResetRFGCards();

            Card traffic = PlayCard("TrafficPileup");
            Card hour = PutOnDeck("HourDevourer");
            FlipCard(GetCard("DayOfSorrows"));

            QuickHPStorage(legacy.CharacterCard, ra.CharacterCard, tempest.CharacterCard, traffic, hour);
            GoToEndOfTurn(fate);
            QuickHPCheck(-3, -3, -3, -3, 0);
        }
        [Test]
        public void TestHourDevourerDamageTwoDays()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();
            ResetDays();
            FlipCard(fate);
            ResetRFGCards();

            Card traffic = PlayCard("TrafficPileup");
            Card hour = PutOnDeck("HourDevourer");
            FlipCard(GetCard("DayOfSorrows"));
            FlipCard(GetCard("DayOfSaints"));

            QuickHPStorage(legacy.CharacterCard, ra.CharacterCard, tempest.CharacterCard, traffic, hour);
            GoToEndOfTurn(fate);
            //6 for 2 days, +2 for Day of Saints buff
            QuickHPCheck(-8, -8, -8, -8, 0);
        }
        [Test]
        public void TestHourDevourerImmunityReward()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();
            ResetDays();
            FlipCard(fate);
            ResetRFGCards();

            Card hour = PlayCard("HourDevourer");
            DealDamage(legacy, hour, 10, DTM);
            QuickHPStorage(legacy, ra, tempest, fate);

            DealDamage(fate, legacy, 10, DTM);
            DealDamage(fate, ra, 10, DTM);
            DealDamage(legacy, fate, 10, DTM);
            DealDamage(tempest, legacy, 10, DTM);

            QuickHPCheck(0, -10, 0, -10);

            GoToStartOfTurn(legacy);
            DealDamage(fate, legacy, 10, DTM);
            QuickHPCheck(-10, 0, 0, 0);
        }
        [Test]
        public void TestHourDevourerImmunityNotIfDirectlyDestroyed()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();
            ResetDays();
            FlipCard(fate);
            ResetRFGCards();

            Card hour = PlayCard("HourDevourer");
            SetHitPoints(hour, 2);
            Card gaze = PlayCard("WrathfulGaze");
            UsePower(gaze);

            QuickHPStorage(ra);
            DealDamage(fate, ra, 10, DTM);
            QuickHPCheck(-10);
        }
        [Test]
        public void TestIllusionOfFreeWill()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();
            ResetDays();
            FlipCard(fate);
            ResetRFGCards();

            SetHitPoints(legacy, 20);
            Card illusion = PlayCard("IllusionOfFreeWill");
            AssertNextToCard(illusion, legacy.CharacterCard);

            QuickHPStorage(legacy, ra, tempest, fate);
            PlayCard("Thokk");
            QuickHPCheck(0, -3, 0, 0);

            Card ring = PutOnDeck("TheLegacyRing");
            Card fort = PutOnDeck("Fortitude");

            AssertNoDecision();
            GoToEndOfTurn(legacy);
            AssertIsInPlay(ring, fort);
        }

        [Test]
        public void TestIllusionOfFreeWill_HeroIncapped()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Tempest","AbsoluteZero/FreedomFiveAbsoluteZeroCharacter", "Megalopolis");
            StartGame();
            ResetDays();
            FlipCard(fate);
            ResetRFGCards();

            SetHitPoints(az, 20);
            Card illusion = PlayCard("IllusionOfFreeWill");
            AssertNextToCard(illusion, az.CharacterCard);
            DealDamage(fate, az, 100, DTM);
            AssertIncapacitated(az);

            //make legacy the highest by a long shot
            SetHitPoints(ra, 10);
            SetHitPoints(tempest, 10);

            GoToUseIncapacitatedAbilityPhase(az);
            QuickHPStorage(legacy, ra, tempest);
            UseIncapacitatedAbility(az, 1);
            //they should all be redirected to Legacy
            QuickHPCheck(-3, 0, 0);
        }
        [Test]
        public void TestIllusionOfFreeWillDecksizeDestruction()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();
            ResetDays();
            FlipCard(fate);
            ResetRFGCards();

            Card illusion = PlayCard("IllusionOfFreeWill");
            MoveCards(fate, fate.TurnTaker.Deck.Cards, fate.TurnTaker.Trash, leaveSomeCards: 4);
            AssertInTrash(illusion);
        }
        [Test]
        public void TestIllusionOfFreeWillFateFlipDestruction()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();
            ResetDays();
            FlipCard(fate);
            ResetRFGCards();

            Card illusion = PlayCard("IllusionOfFreeWill");
            FlipCard(fate);
            AssertNotInPlay(illusion);
        }
        [Test]
        public void TestMemoryOfTomorrowEntersPlayDamage()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();
            ResetDays();
            FlipCard(fate);
            ResetRFGCards();

            QuickHPStorage(legacy, ra, tempest, fate);
            PlayCard("Fortitude");
            PlayCard("MemoryOfTomorrow");
            //5 and 5, so Fortitude is good for a total of 2 damage reduction
            QuickHPCheck(-8, -10, -10, 0);
        }
        [Test]
        public void TestMemoryOfTomorrowFlipFaceUp()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();
            ResetDays();
            FlipCard(fate);
            ResetRFGCards();

            Card malus = PutOnDeck("WarpedMalus");
            Card day = GetCard("DayOfSorrows");
            FlipCard(day);
            DestroyCard(malus);

            Card memory = PlayCard("MemoryOfTomorrow");
            AssertNextToCard(memory, day);

            Card ring = PutOnDeck("TheLegacyRing");
            Card fort = PutInHand("Fortitude");
            DecisionSelectCard = fort;

            FlipCard(day);
            FlipCard(day);
            AssertIsInPlay(malus);
            AssertIsInPlay(fort);
            AssertInHand(ring);
        }
        [Test]
        public void TestMemoryOfTomorrowLastFaceUpDay()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();
            ResetDays();
            FlipCard(fate);
            ResetRFGCards();

            var days = DayCardsInOrder;
            Card firstDay = days[0];
            Card secondDay = days[1];

            FlipCard(firstDay);
            FlipCard(secondDay);
            Card memory = PlayCard("MemoryOfTomorrow");
            AssertNextToCard(memory, secondDay);
        }
        [Test]
        public void TestNecessaryCorrectionDamage()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();
            ResetDays();
            FlipCard(fate);
            ResetRFGCards();

            QuickHPStorage(legacy, ra, tempest);
            PlayCard("NecessaryCorrection");
            QuickHPCheck(-10, -10, 0);
        }
        [Test]
        public void TestNecessaryCorrectionCauseMistressFlip()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();
            ResetDays();
            FlipCard(fate);
            ResetRFGCards();

            GoToStartOfTurn(FindEnvironment());
            Card illusion = PutOnDeck("IllusionOfFreeWill");
            Card butterfly = PutOnDeck("ChaosButterfly");
            Card correction = PlayCard("NecessaryCorrection");
            GoToEndOfTurn();
            AssertOutOfGame(illusion, butterfly);
            AssertNotInPlay(correction);
        }
        [Test]
        public void TestNecessaryCorrectionNotTriggerWithSmallDeck()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();
            ResetDays();
            FlipCard(fate);
            ResetRFGCards();

            GoToStartOfTurn(FindEnvironment());
            Card correction = PlayCard("NecessaryCorrection");
            MoveAllCards(fate, fate.TurnTaker.Deck, fate.TurnTaker.Trash, leaveSomeCards: 4);

            GoToEndOfTurn();
            AssertIsInPlay(correction);
            AssertNumberOfCardsInDeck(fate, 4);
        }
        [Test]
        public void TestResidualMalusDamageResponse()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();
            ResetDays();
            FlipCard(fate);
            ResetRFGCards();

            PlayCard("ResidualMalus");

            QuickHPStorage(legacy, ra, tempest);
            PlayCard("SurgeOfStrength");
            QuickHPCheck(-1, 0, 0);

            //only once per turn
            PlayCard("AquaticCorrespondence");
            QuickHPCheckZero();

            PutInTrash("TrafficPileup");
            GoToStartOfTurn(legacy);
            PlayCard("FleshOfTheSunGod");
            QuickHPCheck(0, -2, 0);
        }

        [Test]
        public void TestResidualMalusDamageResponsePutIntoPlay()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();
            ResetDays();
            FlipCard(fate);
            ResetRFGCards();

            PlayCard("ResidualMalus");

            QuickHPStorage(legacy, ra, tempest);
            PlayCard("SurgeOfStrength", isPutIntoPlay: true);
            QuickHPCheck(0, 0, 0);

            //only once per turn
            PlayCard("AquaticCorrespondence");
            QuickHPCheck(0, 0, -1);

            PutInTrash("TrafficPileup");
            GoToStartOfTurn(legacy);
            PlayCard("FleshOfTheSunGod");
            QuickHPCheck(0, -2, 0);
        }
        [Test]
        public void TestResidualMalusOnDestructionEffect()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();
            ResetDays();
            FlipCard(fate);
            ResetRFGCards();

            Card residual = PlayCard("ResidualMalus");
            Card warped = PlayCard("WarpedMalus");

            DestroyCard(residual);
            AssertInTrash(warped);
        }

        [Test]
        public void TestResidualMalusOnDestructionEffect_NoWarpedMalus()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();
            ResetDays();
            FlipCard(fate);
            ResetRFGCards();

            Card residual = PlayCard("ResidualMalus");
            AssertNextMessage("There are no Warped Malus cards in play for Residual Malus to destroy.");
            DestroyCard(residual);
        }
        [Test]
        public void TestSameTimeAndPlaceDamage()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();
            ResetDays();
            FlipCard(fate);
            ResetRFGCards();

            QuickHPStorage(legacy, ra, tempest);
            PlayCard("Fortitude");
            PlayCard("SameTimeAndPlace");
            QuickHPCheck(-9, -10, -10);
        }
        [Test]
        public void TestSameTimeAndPlaceStoreCard()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();
            ResetDays();
            FlipCard(fate);
            ResetRFGCards();

            Card stap = PlayCard("SameTimeAndPlace");
            Card traffic = PlayCard("TrafficPileup");
            PutOnDeck(FindEnvironment(), traffic);

            AssertUnderCard(stap, traffic);
            Card police = PlayCard("PoliceBackup");
            DestroyCard(police);
            AssertInTrash(police);
        }
        [Test]
        public void TestSameTimeAndPlacePutCardOnDeck()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();
            ResetDays();
            FlipCard(fate);
            ResetRFGCards();

            Card stap = PlayCard("SameTimeAndPlace");
            Card traffic = PlayCard("TrafficPileup");
            DestroyCard(traffic);
            AssertUnderCard(stap, traffic);

            //not face-down to face-up
            Card saints = GetCard("DayOfSaints");
            FlipCard(saints);
            AssertUnderCard(stap, traffic);

            FlipCard(saints);
            AssertNumberOfCardsAtLocation(stap.UnderLocation, 0);
            AssertOnTopOfDeck(traffic);
        }
        [Test]
        public void TestSeeThePatternDamage()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();
            ResetDays();
            FlipCard(fate);
            ResetRFGCards();

            QuickHPStorage(legacy, ra, tempest);
            PlayCard("Fortitude");
            PlayCard("SeeThePattern");
            //H and H - Fortitude applies twice
            QuickHPCheck(-4, -6, -6);
        }
        [Test]
        public void TestSeeThePatternReturn()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();
            ResetDays();
            FlipCard(fate);
            ResetRFGCards();

            Card ring = PutInTrash("TheLegacyRing");
            Card staff = PutInTrash("TheStaffOfRa");
            Card shackles = PutInTrash("GeneBoundShackles");

            Card pattern = PlayCard("SeeThePattern");
            FlipCard(fate);
            AssertInHand(ring, staff, shackles);
            AssertInTrash(pattern);
        }
        [Test]
        public void TestSeeThePatternOptional()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();
            ResetDays();
            FlipCard(fate);
            ResetRFGCards();

            Card ring = PutInTrash("TheLegacyRing");
            Card staff = PutInTrash("TheStaffOfRa");
            Card shackles = PutInTrash("GeneBoundShackles");
            Card pattern = PlayCard("SeeThePattern");

            DecisionSelectTurnTakers = new TurnTaker[] {null, legacy.TurnTaker, ra.TurnTaker, tempest.TurnTaker };
            FlipCard(fate);
            AssertIsInPlay(pattern);
            AssertInTrash(ring, staff, shackles);
        }
        [Test]
        public void TestStolenFutureBasic()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();
            ResetDays();
            FlipCard(fate);
            ResetRFGCards();

            QuickHPStorage(fate);
            Card stolen = PlayCard("StolenFuture");
            AssertInTrash(stolen);
            AssertIncapacitated(legacy);
            QuickHPCheck(-5);
        }
        [Test] 
        public void TestStolenFutureTiedMaxHP()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();
            ResetDays();
            FlipCard(fate);
            ResetRFGCards();

            SetHitPoints(legacy, 30);
            DecisionSelectCards = new Card[] { ra.CharacterCard, fate.CharacterCard };
            QuickHPStorage(fate);
            Card stolen = PlayCard("StolenFuture");
            AssertInTrash(stolen);
            AssertIncapacitated(ra);
            QuickHPCheck(-5);
        }
        [Test]
        public void TestStolenFutureMultiCharacterHero()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "TheSentinels", "Megalopolis");
            StartGame();
            ResetDays();
            FlipCard(fate);
            ResetRFGCards();

            SetHitPoints(legacy, 10);
            SetHitPoints(ra, 10);

            PlayCard("StolenFuture");
            AssertIncapacitated(sentinels);
        }

        [Test]
        public void TestStolenFuture_CompletionistGuise()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Haka", "Guise/CompletionistGuiseCharacter", "Megalopolis");
            StartGame();
            ResetDays();
            FlipCard(fate);
            ResetRFGCards();

            SetHitPoints(legacy, 10);
            SetHitPoints(ra, 10);
            SetHitPoints(haka, 10);

            DecisionSelectCards =  new Card[] { haka.CharacterCard, fate.CharacterCard, ra.CharacterCard, fate.CharacterCard, legacy.CharacterCard, guise.CharacterCard, fate.CharacterCard};
            UsePower(guise.CharacterCard);
            UsePower(guise.CharacterCard);
            UsePower(guise.CharacterCard);
            UsePower(guise.CharacterCard);


            PlayCard("StolenFuture");
            AssertIncapacitated(guise);
            AssertNumberOfCardsUnderCard(guise.CharacterCard, 4);
        }
        [Test]
        public void TestTangledWeftAllowedDiscards()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();
            ResetDays();
            FlipCard(fate);
            ResetRFGCards();

            //give a face-up day card to count
            PutOnDeck("WarpedMalus");
            FlipCard(GetCard("DayOfSorrows"));

            PlayCard("SurgeOfStrength"); //ongoing, limited
            PlayCard("FleshOfTheSunGod"); //ongoing

            PutOnDeck(legacy, legacy.HeroTurnTaker.Hand.Cards);
            PutOnDeck(ra, ra.HeroTurnTaker.Hand.Cards);
            PutOnDeck(tempest, tempest.HeroTurnTaker.Hand.Cards);

            Card ring = PutInHand("TheLegacyRing"); //equip, limited
            Card evo = PutInHand("NextEvolution"); //ongoing
            Card thokk = PutInHand("Thokk");
            Card staff = PutInHand("TheStaffOfRa");

            QuickHPStorage(legacy, ra, tempest);
            DecisionAutoDecideIfAble = true;
            AssertNextDecisionChoices(new Card[] { ring, evo }, new Card[] { thokk });
            PlayCard("TangledWeft");

            QuickHPCheck(0, -5, -5);
        }
        [Test]
        public void TestTangledWeftScalesWithDays()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();
            ResetDays();
            FlipCard(fate);
            ResetRFGCards();

            PlayCard("Fortitude");
            PutOnDeck(legacy, legacy.HeroTurnTaker.Hand.Cards);

            //give a face-up day card to count
            PutOnDeck("WarpedMalus");
            FlipCard(GetCard("DayOfSorrows"));

            PutOnDeck("ResidualMalus");
            FlipCard(GetCard("DayOfSwords"));

            QuickHPStorage(legacy, ra, tempest);
            PlayCard("TangledWeft");
            QuickHPCheck(-8, -10, -10);
        }
        [Test]
        public void TestToDustNotEnoughCards()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();
            ResetDays();
            FlipCard(fate);
            ResetRFGCards();

            QuickHPStorage(legacy, ra, tempest);
            AssertNextMessages("Ra's trash does not have 10 cards to shuffle in!", MessageTerminator);
            PlayCard("ToDust");
            QuickHPCheck(0, -15, 0);
            CheckFinalMessage();
        }
        [Test]
        public void TestToDustRedirect()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();
            ResetDays();
            FlipCard(fate);
            ResetRFGCards();

            Card traffic = PlayCard("TrafficPileup");
            QuickHPStorage(legacy, ra, tempest);
            DecisionAutoDecideIfAble = true;
            DecisionYesNo = true;

            MoveCards(ra, ra.TurnTaker.Deck.Cards, ra.TurnTaker.Trash, leaveSomeCards: 10);
            DecisionRedirectTarget = traffic;
            PlayCard("ToDust");
            AssertInTrash(traffic);
            AssertNumberOfCardsInDeck(ra, 20);
            QuickHPCheckZero();

        }
        //TODO - Warped Malus tests
        [Test]
        public void TestWarpedMalusDR()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();
            ResetDays();
            FlipCard(fate);
            ResetRFGCards();

            Card malus = PlayCard("WarpedMalus");
            QuickHPStorage(malus);
            DealDamage(legacy, malus, 4, DTM);
            QuickHPCheck(-3);
        }
        [Test]
        public void TestWarpedMalusEndOfTurn()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();
            ResetDays();
            FlipCard(fate);
            ResetRFGCards();

            Card malus = PlayCard("WarpedMalus");
            SetHitPoints(malus, 1);
            QuickHPStorage(malus, legacy.CharacterCard, ra.CharacterCard, tempest.CharacterCard);
            GoToEndOfTurn();
            QuickHPCheck(4, -4, -4, -4);
        }

        [Test]
        public void TestChaosButterflyDamage()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();
            ResetDays();
            FlipCard(fate);
            ResetRFGCards();

            PlayCard("Fortitude");
            PlayCard("ChaosButterfly");
            QuickHPStorage(legacy, ra, tempest);
            GoToEndOfTurn();
            QuickHPCheck(-4, -6, -6);
        }
        [Test]
        public void TestChaosButterflySwapDays()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();
            ResetDays();
            FlipCard(fate);
            ResetRFGCards();

            Card firstDay = DayCardsInOrder[0];
            int firstDayStartingIndex = firstDay.PlayIndex ?? -1;
            Card secondDay = DayCardsInOrder[1];
            int secondDayStartingIndex = secondDay.PlayIndex ?? -1;

            FlipCard(firstDay);
            FlipCard(secondDay);

            DestroyCards((Card c) => c.IsVillain && !c.IsCharacter && !c.Definition.Keywords.Contains("day"));
            Card underFirst = firstDay.UnderLocation.Cards.FirstOrDefault();
            Card underSecond = secondDay.UnderLocation.Cards.FirstOrDefault();
            DecisionYesNo = true;
            PlayCard("ChaosButterfly");
            DestroyCard("ChaosButterfly");

            Assert.AreEqual(firstDayStartingIndex, secondDay.PlayIndex);
            Assert.AreEqual(secondDayStartingIndex, firstDay.PlayIndex);

            if (underFirst != null)
            {
                AssertUnderCard(firstDay, underFirst);
            }
            if(underSecond != null)
            {
                AssertUnderCard(secondDay, underSecond);
            }

            FlipCard(fate);
            GoToStartOfTurn(fate);
            AssertNotFlipped(secondDay);
            AssertFlipped(firstDay);

        }
        [Test]
        public void TestChaosButterflyUnableToSwapDays()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Tempest", "Megalopolis");
            StartGame();
            ResetDays();
            FlipCard(fate);
            ResetRFGCards();

            AssertNextMessage("There are not enough face-up day cards to swap their positions.");
            PlayCard("ChaosButterfly");
            DestroyCard("ChaosButterfly");
        }
        //TODO - Test the 'preserve cards' effect with Sentinels, Temple of Zhu Long, Requital Cosmic

        [Test]
        public void TestHeroPreservationMulticharHero()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "TheSentinels", "Megalopolis");
            StartGame();
            ResetDays();
            FlipCard(fate);
            ResetRFGCards();

            Card chains = PlayCard("DurasteelChains");
            DestroyCard(idealist);
            AssertIsInPlay(chains);
            PlayCard("SecondChance");
            AssertIsInPlay(chains);

            Card oath = PutInHand("HippocraticOath");
            DestroyCard(idealist);
            DestroyCard(mainstay);
            DestroyCard(medico);
            DestroyCard(writhe);

            AssertIncapacitated(sentinels);

            FlipCard(fate);

            AssertInHand(oath);
            AssertInTrash(chains);
            AssertNotFlipped(idealist);
        }

        [Test]
        public void TestHeroPreservation_RoninKnight()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Cauldron.TheKnight/WastelandRoninTheKnightCharacter", "Ra", "Megalopolis");
            StartGame();
            ResetDays();
            FlipCard(fate);
            ResetRFGCards();

            DestroyCard(youngKnight);
            DestroyCard(oldKnight);

            AssertIncapacitated(knight);

            FlipCard(fate);

            AssertNotFlipped(youngKnight);
            AssertNotFlipped(oldKnight);

            AssertNumberOfCardsInHand(knight, 4);
            AssertNumberOfCardsInDeck(knight, 36);
            AssertNumberOfCardsInTrash(knight, 0);


        }

        [Test]
        public void TestHeroPreservation_NightloreStarlight()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Cauldron.Starlight/NightloreCouncilStarlightCharacter", "Ra", "Megalopolis");
            StartGame();
            ResetDays();
            FlipCard(fate);
            ResetRFGCards();

            DestroyCard(cryos);
            DestroyCard(asheron);
            DestroyCard(terra);


            AssertIncapacitated(starlight);

            FlipCard(fate);

            AssertNotFlipped(cryos);
            AssertNotFlipped(asheron);
            AssertNotFlipped(terra);


            AssertNumberOfCardsInHand(starlight, 4);
            AssertNumberOfCardsInDeck(starlight, 36);
            AssertNumberOfCardsInTrash(starlight, 0);


        }
        [Test]
        public void TestHeroPreservationCaptainCosmicRequital()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "CaptainCosmic/CaptainCosmicRequitalCharacter", "Megalopolis");
            StartGame();
            ResetDays();
            FlipCard(fate);
            ResetRFGCards();

            Card crest = PlayCard("CosmicCrest");
            DestroyCard(cosmic.CharacterCard);
            Log.Debug($"Crest location: {crest.Location.GetFriendlyName()}");

            FlipCard(fate);
            Log.Debug($"Crest location after revival: {crest.Location.GetFriendlyName()}");
            AssertInTrash(crest);
        }
        [Test]
        public void TestHeroPreservationRevivedByTempleOfZhuLong()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Ra", "Tempest", "TheTempleOfZhuLong");
            StartGame();
            ResetDays();
            FlipCard(fate);
            ResetRFGCards();

            MoveAllCardsFromHandToDeck(tempest);
            DestroyCard(tempest.CharacterCard);

            PlayCard("RitesOfRevival");
            GoToEndOfTurn(FindEnvironment());
            AssertNumberOfCardsInHand(tempest, 2);
            AssertHitPoints(tempest, 8);
        }
    }
}
