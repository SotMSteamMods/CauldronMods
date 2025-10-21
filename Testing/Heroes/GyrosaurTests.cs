using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using Cauldron.Gyrosaur;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CauldronTests
{
    [TestFixture()]
    class GyrosaurTests : CauldronBaseTest
    {
        #region GyrosaurHelperFunctions
        protected DamageType DTM = DamageType.Melee;
        #endregion

        [Test]
        public void TestGyrosaurLoads()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(gyrosaur);
            Assert.IsInstanceOf(typeof(GyrosaurCharacterCardController), gyrosaur.CharacterCardController);

            Assert.AreEqual(30, gyrosaur.CharacterCard.HitPoints);
        }

        [Test]
        public void TestGyrosaurDecklist()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Megalopolis");

            AssertHasKeyword("crash", new string[]
            {
                "IndiscriminatePass",
                "Ricochet",
                "SphereOfDevastation",
                "TerrifyingMomentum",
                "Wipeout",
                "WreckingBall"
            });
            AssertHasKeyword("equipment", new string[]
            {
                "GyroStabilizer"
            });
            AssertHasKeyword("limited", new string[]
            {
                "GyroStabilizer",
                "RapturianShell",
                "RecklessAlienRacingTortoise"
            });
            AssertHasKeyword("one-shot", new string[]
            {
                "IndiscriminatePass",
                "Omnivore",
                "ProtectiveEscort",
                "Ricochet",
                "SphereOfDevastation",
                "TerrifyingMomentum",
                "Wipeout"
            });
            AssertHasKeyword("ongoing", new string[]
            {
                "AMerryChase",
                "HiddenDetour",
                "Hyperspin",
                "OnARoll",
                "RapturianShell",
                "ReadTheTerrain",
                "RecklessAlienRacingTortoise",
                "WreckingBall"
            });
        }

        #region Test Gyrosaur Innate Power
        /*
         * If you have at least 2 crash cards in your hand, {Gyrosaur} deals up to 3 targets 1 melee damage each. 
         * If not, draw a card.
         */
        [Test]
        public void TestGyrosaurInnate2CrashInHand()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            MoveAllCardsFromHandToDeck(gyrosaur);
            PutInHand("SphereOfDevastation"); // Crash
            PutInHand("Ricochet"); // Crash

            PrintSpecialStringsForCard(gyrosaur.CharacterCard);
            string expectedString = "Gyrosaur's hand has 2 crash cards.";
            AssertCardSpecialStringsContain(gyrosaur.CharacterCard, expectedString);

            DecisionSelectTargets = new Card[] { baron.CharacterCard, gyrosaur.CharacterCard, legacy.CharacterCard, ra.CharacterCard };
            QuickHPStorage(baron, gyrosaur, legacy, ra);
            QuickHandStorage(gyrosaur, legacy, ra);
            UsePower(gyrosaur);
            QuickHPCheck(-1, -1, -1, 0);
            QuickHandCheckZero();

            //check that it's "up to"
            DecisionSelectTargets = new Card[] { baron.CharacterCard, null, gyrosaur.CharacterCard };
            DecisionSelectTargetsIndex = 0;
            UsePower(gyrosaur);
            QuickHPCheck(-1, 0, 0, 0);
        }
        [Test]
        public void TestGyrosaurInnate0CrashInHand()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            MoveAllCardsFromHandToDeck(gyrosaur);
            Card stabilizer = PutOnDeck("GyroStabilizer");

            PrintSpecialStringsForCard(gyrosaur.CharacterCard);
            string expectedString = "Gyrosaur's hand has no crash cards.";
            AssertCardSpecialStringsContain(gyrosaur.CharacterCard, expectedString);

            DecisionSelectTargets = new Card[] { baron.CharacterCard, gyrosaur.CharacterCard, legacy.CharacterCard, ra.CharacterCard };
            QuickHandStorage(gyrosaur, legacy, ra);
            QuickHPStorage(baron, gyrosaur, legacy, ra);
            UsePower(gyrosaur);
            QuickHandCheck(1, 0, 0);
            QuickHPCheckZero();
            AssertInHand(stabilizer);
        }
        [Test]
        public void TestGyrosaurInnateStabilized0Crash()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            MoveAllCardsFromHandToDeck(gyrosaur);
            PlayCard("GyroStabilizer");
            Card omni = PutOnDeck("Omnivore");
            DecisionSelectTargets = new Card[] { baron.CharacterCard, gyrosaur.CharacterCard, legacy.CharacterCard, ra.CharacterCard };
            QuickHandStorage(gyrosaur, legacy, ra);
            QuickHPStorage(baron, gyrosaur, legacy, ra);
            AssertNoDecision();

            //with 0 cards in hand Gyro Stabilizer cannot bump it up to the threshold.
            //Therefore it should not present a decision.
            UsePower(gyrosaur);
            QuickHandCheck(1, 0, 0);
            QuickHPCheckZero();
            AssertInHand(omni);
        }
        [Test]
        public void TestGyrosaurInnateStabilized1CrashUnchanged()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            MoveAllCardsFromHandToDeck(gyrosaur);
            PlayCard("GyroStabilizer");
            PutInHand("SphereOfDevastation"); //Crash
            Card omni = PutOnDeck("Omnivore");
            DecisionSelectTargets = new Card[] { baron.CharacterCard, gyrosaur.CharacterCard, legacy.CharacterCard, ra.CharacterCard };
            QuickHandStorage(gyrosaur, legacy, ra);
            QuickHPStorage(baron, gyrosaur, legacy, ra);

            DecisionSelectWord = "1 crash card";

            //with 1 crash in hand Gyro Stabilizer can bump it up to 2.
            //We should get a decision, and be able to stand pat for a draw.
            UsePower(gyrosaur);
            QuickHandCheck(1, 0, 0);
            QuickHPCheckZero();
            AssertInHand(omni);
        }
        [Test]
        public void TestGyrosaurInnateStabilized1CrashIncreased()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            MoveAllCardsFromHandToDeck(gyrosaur);
            PlayCard("GyroStabilizer");
            PutInHand("SphereOfDevastation");
            
            Card omni = PutOnDeck("Omnivore");
            DecisionSelectTargets = new Card[] { baron.CharacterCard, gyrosaur.CharacterCard, legacy.CharacterCard, ra.CharacterCard };
            QuickHandStorage(gyrosaur, legacy, ra);
            QuickHPStorage(baron, gyrosaur, legacy, ra);

            DecisionSelectWord = "2 crash cards";

            //with 1 crash in hand Gyro Stabilizer can bump it up to 2.
            //We should get a decision, and be able to increase it for damage.
            UsePower(gyrosaur);
            QuickHandCheck(0, 0, 0);
            QuickHPCheck(-1, -1, -1, 0);
            AssertOnTopOfDeck(omni);
        }
        [Test]
        public void TestGyrosaurInnateStabilized2CrashUnchanged()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            MoveAllCardsFromHandToDeck(gyrosaur);
            PlayCard("GyroStabilizer");
            PutInHand("SphereOfDevastation"); // Crash
            PutInHand("Wipeout"); // Crash

            Card omni = PutOnDeck("Omnivore");
            DecisionSelectTargets = new Card[] { baron.CharacterCard, gyrosaur.CharacterCard, legacy.CharacterCard, ra.CharacterCard };
            QuickHandStorage(gyrosaur, legacy, ra);
            QuickHPStorage(baron, gyrosaur, legacy, ra);

            DecisionSelectWord = "2 crash cards";

            //with 2 crash in hand, Gyro Stabilizer can bump it down to 1.
            //We should get a decision, and be able to leave it alone for damage.
            UsePower(gyrosaur);
            QuickHandCheck(0, 0, 0);
            QuickHPCheck(-1, -1, -1, 0);
            AssertOnTopOfDeck(omni);
        }
        [Test]
        public void TestGyrosaurInnateStabilized2CrashDecreased()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            MoveAllCardsFromHandToDeck(gyrosaur);
            PlayCard("GyroStabilizer");
            PutInHand("SphereOfDevastation"); // Crash
            PutInHand("Wipeout"); // Crash

            Card omni = PutOnDeck("Omnivore");
            DecisionSelectTargets = new Card[] { baron.CharacterCard, gyrosaur.CharacterCard, legacy.CharacterCard, ra.CharacterCard };
            QuickHandStorage(gyrosaur, legacy, ra);
            QuickHPStorage(baron, gyrosaur, legacy, ra);

            DecisionSelectWord = "1 crash card";

            //with 2 crash in hand, Gyro Stabilizer can bump it down to 1.
            //We should get a decision, and be able to decrease it for a draw.
            UsePower(gyrosaur);
            QuickHandCheck(1, 0, 0);
            QuickHPCheckZero();
            AssertInHand(omni);
        }
        [Test]
        public void TestGyrosaurInnateStabilized3Crash()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            MoveAllCardsFromHandToDeck(gyrosaur);
            PlayCard("GyroStabilizer");
            PutInHand("SphereOfDevastation"); // Crash
            PutInHand("Wipeout"); // Crash
            PutInHand("Ricochet"); // Crash

            Card omni = PutOnDeck("Omnivore");
            DecisionSelectTargets = new Card[] { baron.CharacterCard, gyrosaur.CharacterCard, legacy.CharacterCard, ra.CharacterCard };
            QuickHandStorage(gyrosaur, legacy, ra);
            QuickHPStorage(baron, gyrosaur, legacy, ra);

            AssertMaxNumberOfDecisions(3);

            //with 3 crash in hand, Gyro Stabilizer cannot push them past the threshold.
            //We should not get a decision for it.
            UsePower(gyrosaur);
            QuickHandCheck(0, 0, 0);
            QuickHPCheck(-1, -1, -1, 0);
            AssertOnTopOfDeck(omni);
        }
        #endregion Test Gyrosaur Innate Power

        #region Test Gyrosaur Incap Powers
        /* 
         * One player may draw a card now. 
         * One target with more than 10 HP deals itself 3 melee damage. 
         * Select a non-character target. Increase damage dealt to that target by 1 until the start of your next turn.
        */
        [Test]
        public void TestGyrosaurIncap1()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();
            DealDamage(baron, gyrosaur, 50, DTM);

            AssertIncapLetsHeroDrawCard(gyrosaur, 0, legacy, 1);
        }
        [Test]
        public void TestGyrosaurIncap2()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();
            DealDamage(baron, gyrosaur, 50, DTM);

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            AssertNextDecisionChoices(new Card[] { baron.CharacterCard, legacy.CharacterCard, ra.CharacterCard }, new Card[] { mdp });
            QuickHPStorage(baron, legacy, ra);
            //check it's self-damage
            PlayCard("TheStaffOfRa");
            DecisionSelectTarget = ra.CharacterCard;
            UseIncapacitatedAbility(gyrosaur, 1);
            QuickHPCheck(0, 0, -4);
        }
        [Test]
        public void TestGyrosaurIncap3()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();
            DealDamage(baron, gyrosaur, 50, DTM);

            Card traffic = PlayCard("TrafficPileup");
            Card mdp = GetMobileDefensePlatform().Card;
            AssertNextDecisionChoices(new Card[] { mdp, traffic }, new Card[] { baron.CharacterCard, legacy.CharacterCard, ra.CharacterCard });
            UseIncapacitatedAbility(gyrosaur, 2);
            AssertNumberOfStatusEffectsInPlay(1);
            QuickHPStorage(mdp, traffic);
            DealDamage(ra, mdp, 1, DTM);
            DealDamage(ra, traffic, 1, DTM);
            QuickHPCheck(-2, -1);

            GoToStartOfTurn(gyrosaur);
            AssertNumberOfStatusEffectsInPlay(0);
        }
        #endregion Test Gyrosaur Incap Powers

        #region Test A Merry Chase
        [Test]
        public void TestAMerryChase()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();

            Card chase = PlayCard("AMerryChase");
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            QuickHPStorage(baron, gyrosaur, legacy, ra);
            DealDamage(baron, gyrosaur, 1, DTM);
            AssertNumberOfStatusEffectsInPlay(1);
            DealDamage(baron, legacy, 1, DTM);
            QuickHPCheck(0, -1, 0, 0);

            //doesn't prevent damage to Gyrosaur or stack extra effects
            DealDamage(baron, gyrosaur, 1, DTM);
            QuickHPCheck(0, -1, 0, 0);
            AssertNumberOfStatusEffectsInPlay(1);

            //doesn't stop damage from other targets
            DealDamage(mdp, legacy, 1, DTM);
            QuickHPCheck(0, 0, -1, 0);

            //prevent damage from multiple targets at once
            Card traffic = PlayCard("TrafficPileup");
            DealDamage(traffic, gyrosaur, 1, DTM);
            DealDamage(traffic, legacy, 1, DTM);
            QuickHPCheck(0, -1, 0, 0);
            AssertNumberOfStatusEffectsInPlay(2);

            PlayCard("HeroicInterception");
            QuickHPStorage(baron, gyrosaur, legacy, ra);

            //requires damage to actually be dealt
            DealDamage(mdp, gyrosaur, 1, DTM);
            AssertNumberOfStatusEffectsInPlay(2);
            DealDamage(mdp, legacy, 1, DTM);
            QuickHPCheck(0, 0, -1, 0);

            //start-of-turn destruction and status effect end
            GoToStartOfTurn(gyrosaur);
            AssertInTrash(chase);
            DealDamage(baron, legacy, 1, DTM);
            QuickHPCheck(0, 0, -2, 0);
        }
        #endregion Test A Merry Chase

        #region Test Gyro Stabilizer
        /*
         * When this card enters play, discard up to 3 cards. Draw as many cards as you discarded this way.
         * Whenever you evaluate the number of Crash cards in your hand, you may treat it as being 1 higher or 1 lower than it is.
         */
        //Gyro Stabilizer's "adjust crash-in-hand count" is done as an ActivatesEffect and tested on individual cards
        [Test]
        public void TestGyroStabilizerDiscardToDraw([Values(0, 1, 2, 3)] int numToDiscard)
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();

            //Move all Gyro Stabilizers to the deck to make sure Gyrosaur doesn't play it from hand and mess up the card count
            MoveCards(gyrosaur, FindCardsWhere((Card c) => c.Identifier == "GyroStabilizer"), gyrosaur.TurnTaker.Deck);

            //Make sure Gyrosaur has enough cards in hand to discard 3
            MoveCards(gyrosaur, FindCardsWhere((Card c) => c.Identifier == "Omnivore"), gyrosaur.HeroTurnTaker.Hand);

            var discards = new List<Card>();
            for(int i = 0; i < 3; i++)
            {
                if(i < numToDiscard)
                {
                    discards.Add(gyrosaur.HeroTurnTaker.Hand.Cards.ToList()[i]);
                }
                else
                {
                    discards.Add(null);
                }
            }
            DecisionSelectCards = discards.ToArray();

            QuickHandStorage(gyrosaur, legacy, ra);
            PlayCard("GyroStabilizer");
            QuickHandCheckZero();
            AssertNumberOfCardsInTrash(gyrosaur, numToDiscard);
        }
        #endregion Test Gyro Stabilizer

        #region Test Hiddent Detour
        /* 
         * When this card enters play, {Gyrosaur} regains 2 HP. Then, reveal the top card of the environment 
         * deck and place it beneath this card. 
         * Cards beneath this one are not considered in play. When an environment card would enter play, you 
         * may first switch it with the card beneath this one.
         */
        [Test]
        public void TestHiddenDetourPlayCard()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();

            SetHitPoints(gyrosaur, 20);

            Card traffic = PutOnDeck("TrafficPileup");
            Card detour = PlayCard("HiddenDetour");
            AssertHitPoints(gyrosaur, 22);
            AssertUnderCard(detour, traffic);

            DecisionYesNo = true;
            Card hostage = PlayCard("HostageSituation");
            AssertIsInPlay(traffic);
            AssertUnderCard(detour, hostage);
        }
        [Test]
        public void TestHiddenDetourPutCardInPlay()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();

            SetHitPoints(gyrosaur, 20);

            Card traffic = PutOnDeck("TrafficPileup");
            Card detour = PlayCard("HiddenDetour");
            AssertHitPoints(gyrosaur, 22);
            AssertUnderCard(detour, traffic);

            DecisionYesNo = true;
            Card hostage = PutIntoPlay("HostageSituation");
            AssertIsInPlay(traffic);
            AssertUnderCard(detour, hostage);
        }
        [Test]
        public void TestHiddenDetourMoveCard()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();

            SetHitPoints(gyrosaur, 20);

            Card traffic = PutOnDeck("TrafficPileup");
            Card detour = PlayCard("HiddenDetour");
            AssertHitPoints(gyrosaur, 22);
            AssertUnderCard(detour, traffic);

            DecisionYesNo = true;
            Card hostage = GetCard("HostageSituation");
            GameController.ExhaustCoroutine(GameController.MoveCard(env, hostage, env.TurnTaker.PlayArea));
            AssertIsInPlay(traffic);
            AssertUnderCard(detour, hostage);
        }
        [Test]
        public void TestHiddenDetourEmpty()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "TheTempleOfZhuLong");
            StartGame();

            Card ninja = PutOnDeck("ShinobiAssassin");
            Card detour = PlayCard("HiddenDetour");

            AssertNumberOfCardsAtLocation(detour.UnderLocation, 0);
            AssertNoDecision();
            PlayCard("RitesOfRevival");
        }
        [Test]
        public void TestHiddenDetourOptional()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();

            Card traffic = PutOnDeck("TrafficPileup");
            Card detour = PlayCard("HiddenDetour");
            AssertUnderCard(detour, traffic);

            DecisionYesNo = false;
            Card hostage = PlayCard("HostageSituation");
            AssertIsInPlay(hostage);
            AssertUnderCard(detour, traffic);
        }
        [Test]
        public void TestHiddenDetourWithPrisonRiot()
        {
            Card prisonRiot;
            Card charr;
            Card imprisonedRogue;
            Card timeCrazedPrisoner;
            Card hiddenDetour;

            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "TheBlock");
            StartGame();

            imprisonedRogue = PutOnDeck("ImprisonedRogue");
            charr = PutOnDeck("Char");
            prisonRiot = PutOnDeck("PrisonRiot");
            hiddenDetour = PlayCard("HiddenDetour");
            AssertUnderCard(hiddenDetour, prisonRiot);

            DecisionsYesNo = new bool[] { true, true, false };
            timeCrazedPrisoner = PlayCard("TimeCrazedPrisoner");

            AssertIsInPlay(timeCrazedPrisoner);
            AssertIsInPlay(imprisonedRogue);
            AssertUnderCard(hiddenDetour, charr);
        }
        [Test]
        public void TestMultipleHiddenDetoursWithPrisonRiot()
        {
            Card prisonRiot;
            Card defensiveDisplacement;
            Card charr;
            Card imprisonedRogue;
            Card timeCrazedPrisoner;
            Card hiddenDetour1;
            Card hiddenDetour2;

            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "TheBlock");
            StartGame();

            imprisonedRogue = PutOnDeck("ImprisonedRogue");
            charr = PutOnDeck("Char");
            defensiveDisplacement = PutOnDeck("DefensiveDisplacement");
            prisonRiot = PutOnDeck("PrisonRiot");
            hiddenDetour1 = PlayCard("HiddenDetour", 0);
            hiddenDetour2 = PlayCard("HiddenDetour", 1);
            AssertUnderCard(hiddenDetour1, prisonRiot);
            AssertUnderCard(hiddenDetour2, defensiveDisplacement);

            DecisionsYesNo = new bool[] { true, false, true, true };
            // (true) Replace prison riot with Time Crazed Prisoner - Time Crazed Prisoner under Hidden Detour 1
            // Prison riot comes into play and plays first Char and then Imprisoned Rogue
            // (false) do not replace Time Crazed Prisoner under Hidden Detour 1 with Char - Time Crazed Prisoner under Hidden Detour 1
            // (true) replace Defensive Placement under Hidden Detour 2 with Char - Char under Hidden Detour 2
            // (true) replace Time Crazed Prisoner under Hidden Detour 1 with Imprisioned Rogue - Imprisioned Rogue under Hidden Detour 1
            timeCrazedPrisoner = PutIntoPlay("TimeCrazedPrisoner");

            AssertIsInPlay(timeCrazedPrisoner);
            AssertIsInPlay(defensiveDisplacement);
            AssertIsInPlay(prisonRiot);
            AssertUnderCard(hiddenDetour1, imprisonedRogue);
            AssertUnderCard(hiddenDetour2, charr);
        }

        [Test]
        public void TestHiddenDetoursDestruction()
        {
            Card prisonRiot;
            Card defensiveDisplacement;
            Card hiddenDetour1;
            Card hiddenDetour2;

            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "TheBlock");
            StartGame();

            defensiveDisplacement = PutOnDeck("DefensiveDisplacement");
            prisonRiot = PutOnDeck("PrisonRiot");
            hiddenDetour1 = PlayCard("HiddenDetour", 0);
            hiddenDetour2 = PlayCard("HiddenDetour", 1);
            AssertUnderCard(hiddenDetour1, prisonRiot);
            AssertUnderCard(hiddenDetour2, defensiveDisplacement);

            DestroyCard(hiddenDetour1);
            DestroyCard(hiddenDetour2);

            AssertNotInPlay(hiddenDetour1);
            AssertNotInPlay(hiddenDetour2);
            AssertNotInPlay(defensiveDisplacement);
            AssertNotInPlay(prisonRiot);
        }
        #endregion Test Hidden Detour

        #region Test Hyperspin
        /*
         * When this card enters play, you may play a card.
         * Increase damage dealt by {Gyrosaur} to non-hero targets by 1. 
         * If you would draw a Crash card, play it instead. Then, destroy all other copies of Hyperspin and this card.
         */
        [Test]
        public void TestHyperspinExtraPlayAndDamageBoost()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();
            Card mdp = GetCardInPlay("MobileDefensePlatform");

            Card chase = PutInHand("AMerryChase");
            DecisionSelectCard = chase;
            PlayCard("Hyperspin");

            AssertIsInPlay(chase);

            Card traffic = PlayCard("TrafficPileup");
            QuickHPStorage(mdp, legacy.CharacterCard, traffic);
            DealDamage(gyrosaur, mdp, 1, DTM);
            DealDamage(gyrosaur, legacy, 1, DTM);
            DealDamage(gyrosaur, traffic, 1, DTM);
            QuickHPCheck(-2, -1, -2);

            DealDamage(legacy, mdp, 1, DTM);
            DealDamage(legacy, legacy, 1, DTM);
            DealDamage(legacy, traffic, 1, DTM);
            QuickHPCheck(-1, -1, -1);
        }
        [Test]
        public void TestHyperspinExtraPlayOptional()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();

            Card spin = PutOnDeck("Hyperspin");
            DecisionSelectCards = new Card[] { null };
            QuickHandStorage(gyrosaur, legacy, ra);
            PlayCard(spin);
            QuickHandCheckZero();
        }
        [Test]
        public void TestHyperspinPlayDrawnCrashResponse()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();

            DecisionSelectCards = new Card[] { null };
            Card spin1 = PlayCard("Hyperspin");
            Card spin2 = PlayCard("Hyperspin");
            DecisionSelectCards = null;

            Card stabilizer = PutOnDeck("GyroStabilizer");
            DrawCard(gyrosaur);
            AssertInHand(stabilizer);
            AssertIsInPlay(spin1, spin2);

            QuickHandStorage(gyrosaur, legacy, ra);
            Card ball = PutOnDeck("WreckingBall");
            DrawCard(gyrosaur);
            AssertIsInPlay(ball);
            AssertInTrash(spin1, spin2);
            QuickHandCheckZero();
        }
        [Test]
        public void TestHyperspinNotLeakCrashStatus()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            MoveAllCardsFromHandToDeck(gyrosaur);
            DecisionYesNo = false;

            PlayCard("Hyperspin");

            Card pass = PutOnDeck("IndiscriminatePass"); // Crash
            DrawCard(gyrosaur, optional: true);
            AssertOnTopOfDeck(pass);

            Card chase = PutOnDeck("AMerryChase");
            DrawCard(gyrosaur, optional: true);
            AssertOnTopOfDeck(chase);
        }
        #endregion Test Hyperspin

        #region Test Indiscriminate Pass
        /*
         * If you have at least 1 Crash card in your hand, {Gyrosaur} deals another hero target 2 melee damage.
         * {Gyrosaur} deals 1 non-hero target 4 melee damage.
         */
        [Test]
        public void TestIndiscriminatePass0Crash()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            MoveAllCardsFromHandToDeck(gyrosaur);

            Card traffic = PlayCard("TrafficPileup");
            Card batt = PlayCard("BladeBattalion");
            AssertNextDecisionChoices(new Card[] { baron.CharacterCard, batt, traffic }, new Card[] { gyrosaur.CharacterCard, legacy.CharacterCard, ra.CharacterCard });
            DecisionSelectTarget = baron.CharacterCard;
            QuickHPStorage(baron.CharacterCard, gyrosaur.CharacterCard, legacy.CharacterCard, ra.CharacterCard, traffic, batt);
            PlayCard("IndiscriminatePass");
            QuickHPCheck(-4, 0, 0, 0, 0, 0);
        }
        [Test]
        public void TestIndiscriminatePass1Crash()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            MoveAllCardsFromHandToDeck(gyrosaur);
            PutInHand("WreckingBall"); // Crash

            Card traffic = PlayCard("TrafficPileup");
            Card batt = PlayCard("BladeBattalion");
            AssertNextDecisionChoices(new Card[] { legacy.CharacterCard, ra.CharacterCard }, new Card[] { gyrosaur.CharacterCard, baron.CharacterCard, batt, traffic });
            DecisionSelectTargets = new Card[] { legacy.CharacterCard, baron.CharacterCard };
            QuickHPStorage(baron.CharacterCard, gyrosaur.CharacterCard, legacy.CharacterCard, ra.CharacterCard, traffic, batt);
            PlayCard("IndiscriminatePass");
            QuickHPCheck(-4, 0, -2, 0, 0, 0);
        }
        #endregion Test Indiscriminate Pass

        #region Test Omnivore
        /*
         * Destroy a target with 3 or fewer HP.
         * {Gyrosaur} regains X HP, where X is the HP of that target before it was destroyed. 
         * You may shuffle your trash into your deck.
         */
        [Test]
        public void TestOmnivoreDestroyAndGainHP()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            Card traffic = PlayCard("TrafficPileup");
            Card redist = PlayCard("ElementalRedistributor");

            SetHitPoints(traffic, 2);
            SetHitPoints(mdp, 3);
            SetHitPoints(redist, 4);

            AssertNextDecisionChoices(new Card[] { mdp, traffic }, new Card[] { redist });
            DecisionSelectCards = new Card[] { mdp, traffic };

            SetHitPoints(gyrosaur, 20);
            SetHitPoints(legacy, 20);
            SetHitPoints(ra, 20);
            QuickHPStorage(gyrosaur, legacy, ra);
            PlayCard("Omnivore");
            QuickHPCheck(3, 0, 0);
            AssertInTrash(mdp);
            AssertIsInPlay(traffic);
            PlayCard("Omnivore");
            QuickHPCheck(2, 0, 0);
            AssertInTrash(traffic);
            PlayCard("Omnivore");
            QuickHPCheckZero();
        }
        [Test]
        public void TestOmnivoreDestroyReplaced()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();

            SetHitPoints(baron, 3);
            SetHitPoints(gyrosaur, 10);
            SetHitPoints(legacy, 10);
            SetHitPoints(ra, 10);
            QuickHPStorage(gyrosaur, legacy, ra);
            PlayCard("Omnivore");
            QuickHPCheck(3, 0, 0);
        }
        [Test]
        public void TestOmnivoreShuffleTrash()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();

            Card chase = PutInTrash("AMerryChase");
            QuickShuffleStorage(gyrosaur.TurnTaker.Deck);
            DecisionsYesNo = new bool[] { false, true };

            PlayCard("Omnivore");
            AssertInTrash(chase);
            QuickShuffleCheck(0);

            PlayCard("Omnivore");
            AssertInDeck(chase);
            QuickShuffleCheck(1);
        }
        #endregion Test Omnivore

        #region Test On A Roll
        /*
         * At the end of your turn, draw a card. Then if you have at least 2 Crash cards in your hand, {Gyrosaur} deals 
         * each non-hero target 1 melee damage and this card is destroyed.
         */
        [Test]
        public void TestOnARollDraw()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();

            MoveAllCardsFromHandToDeck(gyrosaur);
            PutInHand("GyroStabilizer");

            Card chase = PutOnDeck("AMerryChase");
            GoToPlayCardPhaseAndPlayCard(gyrosaur, "OnARoll");
            QuickHandStorage(gyrosaur, legacy, ra);
            GoToEndOfTurn();
            QuickHandCheck(1, 0, 0);
            AssertInHand(chase);
            AssertIsInPlay("OnARoll");
        }
        [Test]
        public void TestOnARollSelfDestructResponse()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            MoveAllCardsFromHandToDeck(gyrosaur);

            Card traffic = PlayCard("TrafficPileup"); // Prevents card draw
            Card roll = PlayCard("OnARoll");

            PutInHand("Wipeout"); // Crash
            PutInHand("WreckingBall"); // Crash
            Card top = PutOnDeck("AMerryChase");
            QuickHPStorage(baron.CharacterCard, gyrosaur.CharacterCard, legacy.CharacterCard, ra.CharacterCard, traffic);

            GoToPlayCardPhase(gyrosaur);
            AssertIsInPlay(roll);
            AssertOnTopOfDeck(top);
            QuickHPCheckZero();

            GoToEndOfTurn();
            AssertOnTopOfDeck(top);
            AssertInTrash(roll);
            QuickHPCheck(-1, 0, 0, 0, -1);
        }
        [Test]
        public void TestOnARollSelfDestructResponseFromDraw()
        {
            Card plummetingMonorail;
            Card onARoll;
            Card top;

            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();
            MoveAllCardsFromHandToDeck(gyrosaur);

            plummetingMonorail = PlayCard("PlummetingMonorail");
            onARoll = PlayCard("OnARoll");

            PutInHand("Wipeout"); // Crash
            top = PutOnDeck("WreckingBall"); // Crash

            QuickHPStorage(baron.CharacterCard, gyrosaur.CharacterCard, legacy.CharacterCard, ra.CharacterCard, plummetingMonorail);

            GoToPlayCardPhase(gyrosaur);
            AssertIsInPlay(onARoll);
            AssertOnTopOfDeck(top);
            QuickHPCheckZero();

            GoToEndOfTurn();
            AssertNotOnTopOfDeck(gyrosaur, top);
            AssertInHand(top);
            AssertInTrash(onARoll);
            QuickHPCheck(-1, 0, 0, 0, -1);
        }
        #endregion Test On A Roll

        #region Test Protective Escort
        /*
         * Draw 2 cards. 
         * Select a target and a damage type. That target is immune to that damage type until the start of your next turn.
         */
        [Test]
        public void TestProtectiveEscort()
        {
            SetupGameController(new List<string>() { "BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis" });
            StartGame();
            DestroyNonCharacterVillainCards();

            Card escort = MoveCard(gyrosaur, "ProtectiveEscort", gyrosaur.TurnTaker.Trash);

            DecisionSelectCard = legacy.CharacterCard;
            DecisionSelectDamageType = DamageType.Projectile;
            QuickHandStorage(gyrosaur, legacy, ra);
            QuickHPStorage(baron, gyrosaur, legacy, ra);

            PlayCard(escort);
            QuickHandCheck(2, 0, 0);

            //check immunity
            AssertNumberOfStatusEffectsInPlay(1);
            DealDamage(gyrosaur, legacy, 1, DamageType.Projectile);
            DealDamage(baron, legacy, 1, DamageType.Projectile);
            QuickHPCheckZero();

            //check only projectile
            DealDamage(gyrosaur, legacy, 1, DamageType.Melee);
            QuickHPCheck(0, 0, -1, 0);

            //check only the one target
            DealDamage(baron, gyrosaur, 1, DamageType.Projectile);
            QuickHPCheck(0, -1, 0, 0);

            //check expiration time
            GoToStartOfTurn(gyrosaur);
            AssertNumberOfStatusEffectsInPlay(0);
            DealDamage(baron, legacy, 1, DamageType.Projectile);
            QuickHPCheck(0, 0, -2, 0);
        }
        #endregion

        #region Test Rapturian Shell
        /*
         * At the end of your turn, if you have 0 Crash cards in your hand, {Gyrosaur} deals 1 other hero 2 psychic damage.
         * Powers 
         * Play a Crash card, or discard cards from the top of your deck until you discard a Crash card and put it into your hand.
         */
        [Test]
        public void TestRapturianShellEndOfTurn([Values(true, false)] bool hasCrashInHand)
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();

            DecisionSelectTarget = legacy.CharacterCard;
            MoveAllCardsFromHandToDeck(gyrosaur);
            PlayCard("RapturianShell");
            if(hasCrashInHand)
            {
                PutInHand("WreckingBall"); // Crash
            }

            QuickHPStorage(baron, gyrosaur, legacy, ra);
            if(hasCrashInHand)
            {
                AssertNoDecision();
            }
            else
            {
                AssertNextDecisionChoices(new Card[] { legacy.CharacterCard, ra.CharacterCard }, new Card[] { gyrosaur.CharacterCard, baron.CharacterCard });
            }
            GoToEndOfTurn(gyrosaur);
            int expectedDamage = hasCrashInHand ? 0 : -2;
            QuickHPCheck(0, 0, expectedDamage, 0);
        }
        [Test]
        public void TestRapturianShellPowerPlayCard()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();

            Card shell = PlayCard("RapturianShell");
            MoveAllCardsFromHandToDeck(gyrosaur);

            Card ball = PutInHand("WreckingBall"); // Crash
            Card pass = PutInHand("IndiscriminatePass"); // Crash
            Card chase = PutInHand("AMerryChase");
            DecisionSelectFunction = 0;
            DecisionSelectCard = ball;

            AssertNextDecisionChoices(new Card[] { ball, pass }, new Card[] { chase });
            UsePower(shell);
            AssertIsInPlay(ball);
            AssertInHand(pass);
        }
        [Test]
        public void TestRapturianShellPowerFetchCard()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();

            Card shell = PlayCard("RapturianShell");
            MoveAllCardsFromHandToDeck(gyrosaur);

            Card ball = PutInHand("WreckingBall"); // Crash
            Card pass = PutOnDeck("IndiscriminatePass"); // Crash
            Card chase = PutOnDeck("AMerryChase");
            DecisionSelectFunction = 1;            

            AssertMaxNumberOfDecisions(1);
            UsePower(shell);
            AssertInHand(pass, ball);
            AssertInTrash(chase);
        }
        [Test]
        public void TestRapturianShellCannotPlayCard()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();

            Card shell = PlayCard("RapturianShell");
            MoveAllCardsFromHandToDeck(gyrosaur);

            Card ball = PutInHand("WreckingBall"); // Crash
            Card pass = PutOnDeck("IndiscriminatePass"); // Crash
            Card chase = PutOnDeck("AMerryChase");
            DecisionSelectFunction = 1;

            PlayCard("HostageSituation"); // Prevents hero card play
            AssertNoDecision();
            UsePower(shell);
            AssertInHand(pass, ball);
            AssertInTrash(chase);
        }
        #endregion Test Rapturian Shell

        #region Test Read The Terrain
        /*
         * At the start of your turn, reveal the top card of your deck and replace or discard it.
         * Powers
         * If {Gyrosaur} deals no damage this turn, increase damage dealt by {Gyrosaur} during your next turn to non-hero targets by 1.
         */
        [Test]
        public void TestReadTheTerrainStartOfTurnReplace()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();

            PlayCard("ReadTheTerrain");
            Card spin = PutOnDeck("Hyperspin");
            DecisionYesNo = false;
            GoToStartOfTurn(gyrosaur);
            AssertOnTopOfDeck(spin);
        }
        [Test]
        public void TestReadTheTerrainStartOfTurnDiscard()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();

            PlayCard("ReadTheTerrain");
            Card spin = PutOnDeck("Hyperspin");
            DecisionYesNo = true;
            GoToStartOfTurn(gyrosaur);
            AssertInTrash(spin);
        }
        [Test]
        public void TestReadTheTerrainPowerStandard()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            QuickHPStorage(baron, gyrosaur, legacy, ra);
            Card read = GoToPlayCardPhaseAndPlayCard(gyrosaur, "ReadTheTerrain");
            UsePower(read);
            AssertNumberOfStatusEffectsInPlay(1);

            //damage boost has not kicked in yet
            GoToStartOfTurn(legacy);
            AssertNumberOfStatusEffectsInPlay(1);
            DealDamage(gyrosaur, legacy, 1, DTM);
            DealDamage(gyrosaur, baron, 1, DTM);
            QuickHPCheck(-1, 0, -1, 0);

            //now we get a damage boost
            GoToStartOfTurn(gyrosaur);
            AssertNumberOfStatusEffectsInPlay(1);
            DealDamage(gyrosaur, legacy, 1, DTM);
            DealDamage(gyrosaur, baron, 1, DTM);
            QuickHPCheck(-2, 0, -1, 0);

            //wears off
            GoToStartOfTurn(legacy);
            AssertNumberOfStatusEffectsInPlay(0);
            DealDamage(gyrosaur, legacy, 1, DTM);
            DealDamage(gyrosaur, baron, 1, DTM);
            QuickHPCheck(-1, 0, -1, 0);
        }
        [Test]
        public void TestReadTheTerrainPowerTurnBefore()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            QuickHPStorage(baron, gyrosaur, legacy, ra);
            Card read = PlayCard(gyrosaur, "ReadTheTerrain");
            UsePower(read);
            AssertNumberOfStatusEffectsInPlay(1);

            //now we get a damage boost
            GoToStartOfTurn(gyrosaur);
            AssertNumberOfStatusEffectsInPlay(1);
            DealDamage(gyrosaur, legacy, 1, DTM);
            DealDamage(gyrosaur, baron, 1, DTM);
            QuickHPCheck(-2, 0, -1, 0);

            //wears off
            GoToStartOfTurn(legacy);
            AssertNumberOfStatusEffectsInPlay(0);
            DealDamage(gyrosaur, legacy, 1, DTM);
            DealDamage(gyrosaur, baron, 1, DTM);
            QuickHPCheck(-1, 0, -1, 0);
        }
        [Test]
        public void TestReadTheTerrainPowerDamageStopsEffect()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            QuickHPStorage(baron, gyrosaur, legacy, ra);
            Card read = PlayCard(gyrosaur, "ReadTheTerrain");
            UsePower(read);
            AssertNumberOfStatusEffectsInPlay(1);
            DealDamage(gyrosaur, ra, 1, DTM);

            //no damage boost
            GoToStartOfTurn(gyrosaur);
            AssertNumberOfStatusEffectsInPlay(0);
            DealDamage(gyrosaur, legacy, 1, DTM);
            DealDamage(gyrosaur, baron, 1, DTM);
            QuickHPCheck(-1, 0, -1, -1);
        }
        [Test]
        public void TestReadTheTerrainPowerStacks()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            QuickHPStorage(baron, gyrosaur, legacy, ra);
            Card read = PlayCard(gyrosaur, "ReadTheTerrain");
            UsePower(read);
            UsePower(read);
            AssertNumberOfStatusEffectsInPlay(2);

            //now we get a damage boost
            GoToStartOfTurn(gyrosaur);
            AssertNumberOfStatusEffectsInPlay(1);
            DealDamage(gyrosaur, legacy, 1, DTM);
            DealDamage(gyrosaur, baron, 1, DTM);
            QuickHPCheck(-3, 0, -1, 0);

            //wears off
            GoToStartOfTurn(legacy);
            AssertNumberOfStatusEffectsInPlay(0);
            DealDamage(gyrosaur, legacy, 1, DTM);
            DealDamage(gyrosaur, baron, 1, DTM);
            QuickHPCheck(-1, 0, -1, 0);
        }
        #endregion Test Read The Terrain

        #region Test Reckless Alien Racing Tortoise
        /*
         * During your turn, the first time you have more than 3 Crash cards in your hand, immediately use this card's power and then destroy it.
         * Powers
         * {Gyrosaur} deals 1 target X+1 melee damage, where X is the number of Crash cards in your hand.
         */
        [Test]
        public void TestRecklessAlienRacingTortoisePower()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            MoveAllCardsFromHandToDeck(gyrosaur);

            Card rart = PlayCard("RecklessAlienRacingTortoise");
            QuickHPStorage(baron, gyrosaur, legacy, ra);

            UsePower(rart);
            QuickHPCheck(-1, 0, 0, 0);

            PutInHand("Wipeout"); //Crash
            UsePower(rart);
            QuickHPCheck(-2, 0, 0, 0);
        }
        [Test]
        public void TestRecklessAlienRacingTortoiseAutoUseStartOfTurn()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            MoveAllCardsFromHandToDeck(gyrosaur);

            Card rart = PlayCard("RecklessAlienRacingTortoise");
            QuickHPStorage(baron, gyrosaur, legacy, ra);

            PutInHand("Wipeout"); // Crash
            PutInHand("SphereOfDevastation"); // Crash
            PutInHand("WreckingBall"); // Crash
            PutInHand("IndiscriminatePass"); // Crash

            AssertIsInPlay(rart);
            GoToStartOfTurn(gyrosaur);
            QuickHPCheck(-5, 0, 0, 0);
            AssertInTrash(rart);
        }
        [Test]
        public void TestRecklessAlienRacingTortoiseAutoUseFromDrawCrash()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            MoveAllCardsFromHandToDeck(gyrosaur);

            Card rart = PlayCard("RecklessAlienRacingTortoise");
            QuickHPStorage(baron, gyrosaur, legacy, ra);

            PutInHand("Wipeout"); // Crash
            PutInHand("SphereOfDevastation"); // Crash
            PutInHand("WreckingBall"); // Crash

            PutOnDeck("IndiscriminatePass"); // Crash

            AssertIsInPlay(rart);
            GoToPlayCardPhase(gyrosaur);
            AssertIsInPlay(rart);
            DrawCard(gyrosaur);
            QuickHPCheck(-5, 0, 0, 0);
            AssertInTrash(rart);
        }
        [Test]
        public void TestRecklessAlienRacingTortoiseAutoUseFromPutInHand()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            MoveAllCardsFromHandToDeck(gyrosaur);

            Card rart = PlayCard("RecklessAlienRacingTortoise");
            QuickHPStorage(baron, gyrosaur, legacy, ra);

            PutInHand("Wipeout"); //Crash
            PutInHand("SphereOfDevastation"); // Crash
            PutInHand("WreckingBall"); // Crash

            AssertIsInPlay(rart);
            GoToPlayCardPhase(gyrosaur);
            AssertIsInPlay(rart);

            PutInHand("IndiscriminatePass"); // Crash

            QuickHPCheck(-5, 0, 0, 0);
            AssertInTrash(rart);
        }
        [Test]
        public void TestRecklessAlienRacingTortoiseAutoUseOnlyOnce()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "TimeCataclysm");
            StartGame();
            DestroyNonCharacterVillainCards();

            MoveAllCardsFromHandToDeck(gyrosaur);

            Card rart = PlayCard("RecklessAlienRacingTortoise");
            QuickHPStorage(baron, gyrosaur, legacy, ra);

            PutInHand("Wipeout"); // Crash
            PutInHand("SphereOfDevastation"); // Crash
            PutInHand("WreckingBall"); // Crash

            PlayCard("FixedPoint");

            AssertIsInPlay(rart);
            GoToPlayCardPhase(gyrosaur);
            AssertIsInPlay(rart);

            PutInHand("IndiscriminatePass"); // Crash

            QuickHPCheck(-5, 0, 0, 0);
            AssertIsInPlay(rart);

            GoToEndOfTurn();
            QuickHPCheckZero();
        }
        [Test]
        public void TestRecklessAlienRacingTortoiseAutoUseStabilizedUpToFive()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            MoveAllCardsFromHandToDeck(gyrosaur);

            Card rart = PlayCard("RecklessAlienRacingTortoise");
            PlayCard("GyroStabilizer");
            QuickHPStorage(baron, gyrosaur, legacy, ra);

            PutInHand("Wipeout"); // Crash
            PutInHand("SphereOfDevastation"); // Crash
            PutInHand("WreckingBall"); // Crash
            PutInHand("IndiscriminatePass"); // Crash

            DecisionSelectFunction = 0;
            GoToPlayCardPhase(gyrosaur);

            //one for gyro stabilizer, one for who to damage
            AssertMaxNumberOfDecisions(2);
            DecisionSelectWord = "5 crash cards";
            UsePower(rart);
            AssertInTrash(rart);
            QuickHPCheck(-6, 0, 0, 0);
        }
        [Test]
        public void TestRecklessAlienRacingTortoiseStabilizeSkip()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            MoveAllCardsFromHandToDeck(gyrosaur);

            Card rart = PlayCard("RecklessAlienRacingTortoise");
            PlayCard("GyroStabilizer");
            QuickHPStorage(baron, gyrosaur, legacy, ra);

            PutInHand("Wipeout"); // Crash
            PutInHand("SphereOfDevastation"); // Crash
            PutInHand("WreckingBall"); // Crash
            PutInHand("IndiscriminatePass"); // Crash

            DecisionSelectWord = "4 crash cards";

            AssertMaxNumberOfDecisions(2);
            GoToPlayCardPhase(gyrosaur);

            AssertInTrash(rart);
            QuickHPCheck(-5, 0, 0, 0);
        }
        [Test]
        public void TestRecklessAlienRacingTortoiseStabilizeSkipPart2()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            MoveAllCardsFromHandToDeck(gyrosaur);

            Card rart = PlayCard("RecklessAlienRacingTortoise");
            PlayCard("GyroStabilizer");
            QuickHPStorage(baron, gyrosaur, legacy, ra);

            PutInHand("Wipeout"); // Crash
            PutInHand("SphereOfDevastation"); // Crash
            PutInHand("WreckingBall"); // Crash
            PutInHand("IndiscriminatePass"); // Crash
            DecisionSelectWord = "3 crash cards";

            GoToPlayCardPhase(gyrosaur);
            DecisionSelectWord = "4 crash cards";

            UsePower(rart);
            AssertInTrash(rart);
            QuickHPCheck(-5, 0, 0, 0);
        }
        [Test]
        public void TestRecklessAlienRacingTortoiseNotLeakCrashStatus()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            MoveAllCardsFromHandToDeck(gyrosaur);
            PlayCard("RecklessAlienRacingTortoise");
            PutInHand("IndiscriminatePass"); // Crash
            PutInHand("WreckingBall"); // Crash
            PutInHand("SphereOfDevastation"); // Crash

            GoToStartOfTurn(gyrosaur);

            DecisionYesNo = false;
            Card wipeout = PutOnDeck("Wipeout");
            DrawCard(gyrosaur, optional: true);
            AssertOnTopOfDeck(wipeout);

            Card chase = PutOnDeck("AMerryChase");
            DrawCard(gyrosaur, optional: true);
            AssertOnTopOfDeck(chase);
        }

        [Test]
        public void TestRecklessAlienRacingTortoiseAutoUseIsActualPower()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            MoveAllCardsFromHandToDeck(gyrosaur);

            Card rart = PlayCard("RecklessAlienRacingTortoise");
            QuickHPStorage(baron, gyrosaur, legacy, ra);

            Card paparazzi = PlayCard("PaparazziOnTheScene");

            PutInHand("Wipeout"); // Crash
            PutInHand("SphereOfDevastation"); // Crash
            PutInHand("WreckingBall"); // Crash
            PutInHand("IndiscriminatePass"); // Crash

            AssertIsInPlay(rart);
            GoToStartOfTurn(gyrosaur);
            QuickHPCheck(0, 0, 0, 0);
            AssertInTrash(rart);
        }
        #endregion Test Reckless Alien Racing Tortoise

        #region Test Ricochet
        /* 
         * {Gyrosaur} deals 1 target 2 melee damage. {Gyrosaur} deals a second target X melee damage, where X is the amount of damage she dealt to the first target. 
         * Reduce the next damage dealt by non-hero targets damaged this way to 0.
         */
        [Test]
        public void TestRicochet()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            DecisionSelectCards = new Card[] { ra.CharacterCard, baron.CharacterCard };
            QuickHPStorage(baron, gyrosaur, legacy, ra);
            PlayCard("ImbuedFire");

            PlayCard("Ricochet");
            QuickHPCheck(-4, 0, 0, -3);

            AssertNumberOfStatusEffectsInPlay(1);
            DealDamage(baron, gyrosaur, 1, DamageType.Fire);
            QuickHPCheckZero();
            AssertNumberOfStatusEffectsInPlay(0);
            DealDamage(baron, gyrosaur, 1, DamageType.Fire);
            QuickHPCheck(0, -2, 0, 0);
        }
        [Test]
        public void TestRicochetNoDamageDealt()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DecisionSelectCards = new Card[] { baron.CharacterCard, mdp };
            QuickHPStorage(baron.CharacterCard, gyrosaur.CharacterCard, legacy.CharacterCard, ra.CharacterCard, mdp);
            PlayCard("ImbuedFire");

            PlayCard("Ricochet");
            QuickHPCheck(0, 0, 0, 0, -1);

            AssertNumberOfStatusEffectsInPlay(1);
            DealDamage(baron, gyrosaur, 1, DamageType.Fire);
            QuickHPCheck(0, -2, 0, 0, 0);
            DealDamage(mdp, gyrosaur, 1, DamageType.Fire);
            QuickHPCheckZero();
            AssertNumberOfStatusEffectsInPlay(0);
        }

        #endregion Test Ricochet

        #region Test Sphere of Devastation
        /*
         * Discard all Crash cards in your hand. {Gyrosaur} deals 1 target X+4 melee damage, where X is 4 times the number of cards discarded this way. 
         * If {Gyrosaur} dealt more than 10 damage this way, destroy all environment cards and each other player discards a card.
         */
        [Test]
        public void TestSphereOfDevastationBelowThreshold()
        {
            Card wipeout; //Crash
            Card aMerryChase;
            Card policeBackup;

            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            MoveAllCardsFromHandToDeck(gyrosaur);

            QuickHPStorage(baron, gyrosaur, legacy, ra);

            wipeout = PutInHand("Wipeout"); // Crash
            aMerryChase = PutInHand("AMerryChase");
            policeBackup = PutIntoPlay("PoliceBackup");
            QuickHandStorage(gyrosaur, legacy, ra);

            PlayCard("SphereOfDevastation");
            QuickHPCheck(-8, 0, 0, 0); // 1 Crash card discarded, should be 8 damage.
            QuickHandCheck(-1, 0, 0);
            AssertInTrash(wipeout);
            AssertInHand(aMerryChase);
            AssertIsInPlay(policeBackup);

            PlayCard("SphereOfDevastation");
            QuickHPCheck(-4, 0, 0, 0);
        }

        [Test]
        public void TestSphereOfDevastationAboveThreshold()
        {
            Card wipeout; // Crash
            Card indiscriminatePass; // Crash
            Card aMerryChase;
            Card policeBackup;

            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            MoveAllCardsFromHandToDeck(gyrosaur);

            wipeout = PutInHand("Wipeout");
            indiscriminatePass = PutInHand("IndiscriminatePass");
            aMerryChase = PutInHand("AMerryChase");
            policeBackup = PutIntoPlay("PoliceBackup");

            QuickHandStorage(gyrosaur, legacy, ra);
            QuickHPStorage(baron, gyrosaur, legacy, ra);
            PlayCard("SphereOfDevastation");
            QuickHPCheck(-12, 0, 0, 0);
            QuickHandCheck(-2, -1, -1);
            AssertInTrash(wipeout, indiscriminatePass, policeBackup);
            AssertInHand(aMerryChase);
        }
        [Test]
        public void TestSphereOfDevastationDamagePrevented()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();

            MoveAllCardsFromHandToDeck(gyrosaur);

            Card wipe = PutInHand("Wipeout");
            Card pass = PutInHand("IndiscriminatePass");
            Card chase = PutInHand("AMerryChase");
            Card police = PutIntoPlay("PoliceBackup");
            QuickHandStorage(gyrosaur, legacy, ra);
            QuickHPStorage(baron, gyrosaur, legacy, ra);
            PlayCard("SphereOfDevastation");
            QuickHPCheckZero();
            QuickHandCheck(-2, 0, 0);
            AssertInTrash(wipe, pass);
            AssertIsInPlay(police);
            AssertInHand(chase);
        }

        [Test]
        public void TestSphereOfDevastationOblivAeon()
        {
            SetupGameController(new string[] { "OblivAeon", "Cauldron.Gyrosaur", "Legacy", "Haka", "Ra", "Cauldron.WindmillCity", "MobileDefensePlatform", "InsulaPrimalis", "Cauldron.VaultFive", "Cauldron.Northspar" }, shieldIdentifier: "PrimaryObjective");
            StartGame();

            SwitchBattleZone(legacy);

            Card wipeout = PutInHand("Wipeout");
            Card indiscriminatePass = PutInHand("IndiscriminatePass");
            Card terrifyingMomentum = PutInHand("TerrifyingMomentum");

            PlayCard("SphereOfDevastation");

            AssertNumberOfCardsInHand(legacy, 4);
            AssertNumberOfCardsInHand(haka, 3);
            AssertNumberOfCardsInHand(ra, 3);




        }
        #endregion Test Sphere of Devastation

        #region Test Terrifying Momentum
        /*
         * {Gyrosaur} deals 1 target X+2 melee damage, where X is the number of Crash cards in your hand. 
         * If X is more than 4, redirect this damage to the non-hero target with the lowest HP. 
         * Draw a card.
         */
        [Test]
        public void TestTerrifyingMomentumBelowThreshold()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            MoveAllCardsFromHandToDeck(gyrosaur);

            //2 crash cards
            Card wipe = PutInHand("Wipeout"); // Crash
            Card pass = PutInHand("IndiscriminatePass"); // Crash
            Card chase = PutOnDeck("AMerryChase");

            Card innocents = PutIntoPlay("TargetingInnocents");
            DecisionSelectTarget = baron.CharacterCard;
            QuickHPStorage(baron.CharacterCard, innocents, gyrosaur.CharacterCard, legacy.CharacterCard, ra.CharacterCard);
            PlayCard("TerrifyingMomentum");
            QuickHPCheck(-4, 0, 0, 0, 0);
            AssertInHand(chase);

            PutInTrash(wipe);
            //only 1 crash card now
            PlayCard("TerrifyingMomentum"); // Crash
            QuickHPCheck(-3, 0, 0, 0, 0);
        }
        [Test]
        public void TestTerrifyingMomentumAboveThreshold()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            MoveAllCardsFromHandToDeck(gyrosaur);

            //5 crash cards
            Card wipe = PutInHand("Wipeout"); // Crash
            Card pass = PutInHand("IndiscriminatePass"); // Crash
            Card rico = PutInHand("Ricochet"); // Crash
            Card wreck = PutInHand("WreckingBall"); // Crash
            Card sphere = PutInHand("SphereOfDevastation"); // Crash
            Card chase = PutOnDeck("AMerryChase");

            Card innocents = PutIntoPlay("TargetingInnocents");
            DecisionSelectTarget = baron.CharacterCard;
            QuickHPStorage(baron.CharacterCard, innocents, gyrosaur.CharacterCard, legacy.CharacterCard, ra.CharacterCard);
            PlayCard("TerrifyingMomentum");
            QuickHPCheck(0, -7, 0, 0, 0);
            AssertInHand(chase);

            PutInTrash(wipe);
            //only 4 crash cards now
            PlayCard("TerrifyingMomentum");
            QuickHPCheck(-6, 0, 0, 0, 0);
        }
        #endregion Test Terrifying Momentum

        #region Test Wipeout
        /*
         * {Gyrosaur} deals up to X+1 targets 4 melee damage each, then deals herself X+1 melee damage, where X is the number of Crash cards in your hand.
         */
        [Test]
        public void TestWipeout()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            MoveAllCardsFromHandToDeck(gyrosaur);

            Card wipe1 = PutInHand("Wipeout");
            Card wipe2 = PutInHand("Wipeout");

            Card redist = PutIntoPlay("ElementalRedistributor");

            DecisionSelectTargets = new Card[] { baron.CharacterCard, redist };
            QuickHPStorage(baron.CharacterCard, redist, gyrosaur.CharacterCard, legacy.CharacterCard, ra.CharacterCard);

            PlayCard(wipe1);
            QuickHPCheck(-4, -4, -2, 0, 0);

            DecisionSelectTargetsIndex = 0;

            PlayCard(wipe2);
            QuickHPCheck(-4, 0, -1, 0, 0);
        }
        #endregion Test Wipeout

        #region Test Wrecking Ball
        /*
         * When this card enters play, {Gyrosaur} deals each target 1 melee damage.
         * Increase damage dealt to environment targets by 1.
         */
        [Test]
        public void TestWreckingBall()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card innocents = PlayCard("TargetingInnocents");
            Card traffic = PlayCard("TrafficPileup");

            QuickHPStorage(baron.CharacterCard, gyrosaur.CharacterCard, legacy.CharacterCard, ra.CharacterCard, innocents, traffic);
            PlayCard("WreckingBall");
            QuickHPCheck(-1, -1, -1, -1, -2, -2);

            DealDamage(legacy, traffic, 1, DTM);
            QuickHPCheck(0, 0, 0, 0, 0, -2);
        }
        #endregion Test Wrecking Ball

        #region Test Representative of Earth
        [Test()]
        public void TestGyrosaurAsRepresentativeOfEarth()
        {
            SetupGameController(new string[] { "BaronBlade", "Legacy", "Haka", "Tachyon", "TheCelestialTribunal" });
            StartGame();

            DecisionSelectFromBoxIdentifiers = new string[] { "Cauldron.GyrosaurCharacter" };
            DecisionSelectFromBoxTurnTakerIdentifier = "Cauldron.Gyrosaur";

            Card representative = PlayCard("RepresentativeOfEarth");

            PrintJournal();

            Card gyrosaurCharacter = FindCardInPlay("GyrosaurCharacter");
            PrintSeparator("SPECIAL STRINGS START");
            PrintSpecialStringsForCard(gyrosaurCharacter);
            PrintSeparator("SPECIAL STRINGS END");
            AssertNumberOfCardSpecialStrings(gyrosaurCharacter, 1);
            string potentialString1 = "Gyrosaur's hand has no crash cards.";
            string potentialString2 = "The Celestial Tribunal's hand has no crash cards.";
            AssertCardSpecialStringsDoesNotContain(gyrosaurCharacter, potentialString1);
            AssertCardSpecialStringsDoesNotContain(gyrosaurCharacter, potentialString2);

        }
        #endregion
    }
}
