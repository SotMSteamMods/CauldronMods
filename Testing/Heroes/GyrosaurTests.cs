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
        [Test]
        public void TestGyrosaurInnate2CrashInHand()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            PutInHand("SphereOfDevastation");
            PutInHand("Ricochet");

            DecisionSelectTargets = new Card[] { baron.CharacterCard, gyrosaur.CharacterCard, legacy.CharacterCard, ra.CharacterCard };
            QuickHPStorage(baron, gyrosaur, legacy, ra);
            QuickHandStorage(gyrosaur);
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
            DecisionSelectTargets = new Card[] { baron.CharacterCard, gyrosaur.CharacterCard, legacy.CharacterCard, ra.CharacterCard };
            QuickHandStorage(gyrosaur);
            QuickHPStorage(baron, gyrosaur, legacy, ra);
            UsePower(gyrosaur);
            QuickHandCheck(1);
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
            QuickHandStorage(gyrosaur);
            QuickHPStorage(baron, gyrosaur, legacy, ra);
            AssertNoDecision();

            //with 0 cards in hand Gyro Stabilizer cannot bump it up to the threshold.
            //Therefore it should not present a decision.
            UsePower(gyrosaur);
            QuickHandCheck(1);
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
            PutInHand("SphereOfDevastation");
            Card omni = PutOnDeck("Omnivore");
            DecisionSelectTargets = new Card[] { baron.CharacterCard, gyrosaur.CharacterCard, legacy.CharacterCard, ra.CharacterCard };
            QuickHandStorage(gyrosaur);
            QuickHPStorage(baron, gyrosaur, legacy, ra);

            DecisionDoNotSelectFunction = true;

            //with 1 crash in hand Gyro Stabilizer can bump it up to 2.
            //We should get a decision, and be able to stand pat for a draw.
            UsePower(gyrosaur);
            QuickHandCheck(1);
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
            QuickHandStorage(gyrosaur);
            QuickHPStorage(baron, gyrosaur, legacy, ra);

            DecisionSelectFunction = 2;

            //with 1 crash in hand Gyro Stabilizer can bump it up to 2.
            //We should get a decision, and be able to increase it for damage.
            UsePower(gyrosaur);
            QuickHandCheck(0);
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
            PutInHand("SphereOfDevastation");
            PutInHand("Wipeout");

            Card omni = PutOnDeck("Omnivore");
            DecisionSelectTargets = new Card[] { baron.CharacterCard, gyrosaur.CharacterCard, legacy.CharacterCard, ra.CharacterCard };
            QuickHandStorage(gyrosaur);
            QuickHPStorage(baron, gyrosaur, legacy, ra);

            DecisionDoNotSelectFunction = true;

            //with 2 crash in hand, Gyro Stabilizer can bump it down to 1.
            //We should get a decision, and be able to leave it alone for damage.
            UsePower(gyrosaur);
            QuickHandCheck(0);
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
            PutInHand("SphereOfDevastation");
            PutInHand("Wipeout");

            Card omni = PutOnDeck("Omnivore");
            DecisionSelectTargets = new Card[] { baron.CharacterCard, gyrosaur.CharacterCard, legacy.CharacterCard, ra.CharacterCard };
            QuickHandStorage(gyrosaur);
            QuickHPStorage(baron, gyrosaur, legacy, ra);

            DecisionSelectFunction = 0;

            //with 2 crash in hand, Gyro Stabilizer can bump it down to 1.
            //We should get a decision, and be able to decrease it for a draw.
            UsePower(gyrosaur);
            QuickHandCheck(1);
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
            PutInHand("SphereOfDevastation");
            PutInHand("Wipeout");
            PutInHand("Ricochet");

            Card omni = PutOnDeck("Omnivore");
            DecisionSelectTargets = new Card[] { baron.CharacterCard, gyrosaur.CharacterCard, legacy.CharacterCard, ra.CharacterCard };
            QuickHandStorage(gyrosaur);
            QuickHPStorage(baron, gyrosaur, legacy, ra);

            AssertMaxNumberOfDecisions(3);

            //with 3 crash in hand, Gyro Stabilizer cannot push them past the threshold.
            //We should not get a decision for it.
            UsePower(gyrosaur);
            QuickHandCheck(0);
            QuickHPCheck(-1, -1, -1, 0);
            AssertOnTopOfDeck(omni);
        }
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
        //Gyro Stabilizer's "adjust crash-in-hand count" is done as an ActivatesEffect and tested on individual cards
        [Test]
        public void TestGyroStabilizerDiscardToDraw([Values(0, 1, 2, 3)] int numToDiscard)
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();

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
            QuickHandStorage(gyrosaur);
            PlayCard("GyroStabilizer");
            QuickHandCheckZero();
            AssertNumberOfCardsInTrash(gyrosaur, numToDiscard);
        }
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
            QuickHandStorage(gyrosaur);
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

            QuickHandStorage(gyrosaur);
            Card ball = PutOnDeck("WreckingBall");
            DrawCard(gyrosaur);
            AssertIsInPlay(ball);
            AssertInTrash(spin1, spin2);
            QuickHandCheckZero();
        }
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
            QuickHPStorage(baron, gyrosaur, legacy, ra);
            PlayCard("IndiscriminatePass");
            QuickHPCheck(-4, 0, 0, 0);
        }
        [Test]
        public void TestIndiscriminatePass1Crash()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            MoveAllCardsFromHandToDeck(gyrosaur);
            PutInHand("WreckingBall");

            Card traffic = PlayCard("TrafficPileup");
            Card batt = PlayCard("BladeBattalion");
            AssertNextDecisionChoices(new Card[] { legacy.CharacterCard, ra.CharacterCard }, new Card[] { gyrosaur.CharacterCard, baron.CharacterCard, batt, traffic });
            DecisionSelectTargets = new Card[] { legacy.CharacterCard, baron.CharacterCard };
            QuickHPStorage(baron, gyrosaur, legacy, ra);
            PlayCard("IndiscriminatePass");
            QuickHPCheck(-4, 0, -2, 0);
        }
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
            QuickHPStorage(gyrosaur);
            PlayCard("Omnivore");
            QuickHPCheck(3);
            AssertInTrash(mdp);
            AssertIsInPlay(traffic);
            PlayCard("Omnivore");
            QuickHPCheck(2);
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
            QuickHPStorage(gyrosaur);
            PlayCard("Omnivore");
            QuickHPCheck(3);
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
        [Test]
        public void TestOnARollDraw()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();

            MoveAllCardsFromHandToDeck(gyrosaur);
            PutInHand("GyroStabilizer");

            Card chase = PutOnDeck("AMerryChase");
            GoToPlayCardPhaseAndPlayCard(gyrosaur, "OnARoll");
            QuickHandStorage(gyrosaur);
            GoToEndOfTurn();
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

            Card traffic = PlayCard("TrafficPileup");
            Card roll = PlayCard("OnARoll");

            PutInHand("Wipeout");
            PutInHand("WreckingBall");
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
        public void TestProtectiveEscort()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            DecisionSelectCard = legacy.CharacterCard;
            DecisionSelectDamageType = DamageType.Projectile;
            QuickHandStorage(gyrosaur);
            QuickHPStorage(baron, gyrosaur, legacy, ra);

            PlayCard("ProtectiveEscort");
            QuickHandCheck(2);

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
            DealDamage(baron, legacy, 1, DamageType.Melee);
            QuickHPCheck(0, 0, -2, 0);
        }
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
                PutInHand("WreckingBall");
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

            Card ball = PutInHand("WreckingBall");
            Card pass = PutInHand("IndiscriminatePass");
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

            Card ball = PutInHand("WreckingBall");
            Card pass = PutOnDeck("IndiscriminatePass");
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

            Card ball = PutInHand("WreckingBall");
            Card pass = PutOnDeck("IndiscriminatePass");
            Card chase = PutOnDeck("AMerryChase");
            DecisionSelectFunction = 1;

            PlayCard("HostageSituation");
            AssertNoDecision();
            UsePower(shell);
            AssertInHand(pass, ball);
            AssertInTrash(chase);
        }
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

            PutInHand("Wipeout");
            UsePower(rart);
            QuickHPCheck(-2, 0, 0, 0);
        }
        [Test]
        public void TestRecklessAlienRacingTortoiseAutouseStartOfTurn()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            MoveAllCardsFromHandToDeck(gyrosaur);

            Card rart = PlayCard("RecklessAlienRacingTortoise");
            QuickHPStorage(baron, gyrosaur, legacy, ra);

            PutInHand("Wipeout");
            PutInHand("SphereOfDevastation");
            PutInHand("WreckingBall");
            PutInHand("IndiscriminatePass");

            AssertIsInPlay(rart);
            GoToStartOfTurn(gyrosaur);
            QuickHPCheck(-5, 0, 0, 0);
            AssertInTrash(rart);
        }
        [Test]
        public void TestRecklessAlienRacingTortoiseAutouseFromDrawCrash()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            MoveAllCardsFromHandToDeck(gyrosaur);

            Card rart = PlayCard("RecklessAlienRacingTortoise");
            QuickHPStorage(baron, gyrosaur, legacy, ra);

            PutInHand("Wipeout");
            PutInHand("SphereOfDevastation");
            PutInHand("WreckingBall");

            PutOnDeck("IndiscriminatePass");

            AssertIsInPlay(rart);
            GoToPlayCardPhase(gyrosaur);
            AssertIsInPlay(rart);
            DrawCard(gyrosaur);
            QuickHPCheck(-5, 0, 0, 0);
            AssertInTrash(rart);
        }
        [Test]
        public void TestRecklessAlienRacingTortoiseAutouseFromPutInHand()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            MoveAllCardsFromHandToDeck(gyrosaur);

            Card rart = PlayCard("RecklessAlienRacingTortoise");
            QuickHPStorage(baron, gyrosaur, legacy, ra);

            PutInHand("Wipeout");
            PutInHand("SphereOfDevastation");
            PutInHand("WreckingBall");


            AssertIsInPlay(rart);
            GoToPlayCardPhase(gyrosaur);
            AssertIsInPlay(rart);

            PutInHand("IndiscriminatePass");

            QuickHPCheck(-5, 0, 0, 0);
            AssertInTrash(rart);
        }
        [Test]
        public void TestRecklessAlienRacingTortoiseAutouseOnlyOnce()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "TimeCataclysm");
            StartGame();
            DestroyNonCharacterVillainCards();

            MoveAllCardsFromHandToDeck(gyrosaur);

            Card rart = PlayCard("RecklessAlienRacingTortoise");
            QuickHPStorage(baron, gyrosaur, legacy, ra);

            PutInHand("Wipeout");
            PutInHand("SphereOfDevastation");
            PutInHand("WreckingBall");

            PlayCard("FixedPoint");


            AssertIsInPlay(rart);
            GoToPlayCardPhase(gyrosaur);
            AssertIsInPlay(rart);

            PutInHand("IndiscriminatePass");

            QuickHPCheck(-5, 0, 0, 0);
            AssertIsInPlay(rart);

            GoToEndOfTurn();
            QuickHPCheckZero();
        }
        [Test]
        public void TestRecklessAlienRacingTortoiseAutouseStabilizedUpToFive()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            MoveAllCardsFromHandToDeck(gyrosaur);

            Card rart = PlayCard("RecklessAlienRacingTortoise");
            PlayCard("GyroStabilizer");
            QuickHPStorage(baron, gyrosaur, legacy, ra);

            PutInHand("Wipeout");
            PutInHand("SphereOfDevastation");
            PutInHand("WreckingBall");
            PutInHand("IndiscriminatePass");

            DecisionSelectFunction = 0;
            GoToPlayCardPhase(gyrosaur);

            //one for gyro stabilizer, one for who to damage
            AssertMaxNumberOfDecisions(2);
            DecisionSelectFunction = 2;
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

            PutInHand("Wipeout");
            PutInHand("SphereOfDevastation");
            PutInHand("WreckingBall");
            PutInHand("IndiscriminatePass");

            DecisionDoNotSelectFunction = true;

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

            PutInHand("Wipeout");
            PutInHand("SphereOfDevastation");
            PutInHand("WreckingBall");
            PutInHand("IndiscriminatePass");
            DecisionSelectFunction = 0;

            GoToPlayCardPhase(gyrosaur);
            DecisionDoNotSelectFunction = true;
            UsePower(rart);
            AssertInTrash(rart);
            QuickHPCheck(-5, 0, 0, 0);
        }
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
        [Test]
        public void TestSphereOfDevastationBelowThreshold()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            MoveAllCardsFromHandToDeck(gyrosaur);

            QuickHPStorage(baron, gyrosaur, legacy, ra);
            Card wipe = PutInHand("Wipeout");
            Card chase = PutInHand("AMerryChase");
            Card police = PutIntoPlay("PoliceBackup");
            QuickHandStorage(gyrosaur, legacy, ra);
            PlayCard("SphereOfDevastation");
            QuickHPCheck(-8, 0, 0, 0);
            QuickHandCheck(-1, 0, 0);
            AssertInTrash(wipe);
            AssertInHand(chase);
            AssertIsInPlay(police);

            PlayCard("SphereOfDevastation");
            QuickHPCheck(-4, 0, 0, 0);
        }
        [Test]
        public void TestSphereOfDevastationAboveThreshold()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            MoveAllCardsFromHandToDeck(gyrosaur);

            Card wipe = PutInHand("Wipeout");
            Card pass = PutInHand("IndiscriminatePass");
            Card chase = PutInHand("AMerryChase");
            Card police = PutIntoPlay("PoliceBackup");
            QuickHandStorage(gyrosaur, legacy, ra);
            QuickHPStorage(baron, gyrosaur, legacy, ra);
            PlayCard("SphereOfDevastation");
            QuickHPCheck(-12, 0, 0, 0);
            QuickHandCheck(-2, -1, -1);
            AssertInTrash(wipe, pass, police);
            AssertInHand(chase);
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
        public void TestTerrifyingMomentumBelowThreshold()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            MoveAllCardsFromHandToDeck(gyrosaur);

            //2 crash cards
            Card wipe = PutInHand("Wipeout");
            Card pass = PutInHand("IndiscriminatePass");
            Card chase = PutOnDeck("AMerryChase");

            Card innocents = PutIntoPlay("TargetingInnocents");
            DecisionSelectTarget = baron.CharacterCard;
            QuickHPStorage(baron.CharacterCard, innocents);
            PlayCard("TerrifyingMomentum");
            QuickHPCheck(-4, 0);
            AssertInHand(chase);

            PutInTrash(wipe);
            //only 1 crash card now
            PlayCard("TerrifyingMomentum");
            QuickHPCheck(-3, 0);
        }
        [Test]
        public void TestTerrifyingMomentumAboveThreshold()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            MoveAllCardsFromHandToDeck(gyrosaur);

            //5 crash cards
            Card wipe = PutInHand("Wipeout");
            Card pass = PutInHand("IndiscriminatePass");
            Card rico = PutInHand("Ricochet");
            Card wreck = PutInHand("WreckingBall");
            Card sphere = PutInHand("SphereOfDevastation");
            Card chase = PutOnDeck("AMerryChase");

            Card innocents = PutIntoPlay("TargetingInnocents");
            DecisionSelectTarget = baron.CharacterCard;
            QuickHPStorage(baron.CharacterCard, innocents);
            PlayCard("TerrifyingMomentum");
            QuickHPCheck(0, -7);
            AssertInHand(chase);

            PutInTrash(wipe);
            //only 4 crash cards now
            PlayCard("TerrifyingMomentum");
            QuickHPCheck(-6, 0);
        }
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
            QuickHPStorage(baron.CharacterCard, redist, gyrosaur.CharacterCard);

            PlayCard(wipe1);
            QuickHPCheck(-4, -4, -2);

            DecisionSelectTargetsIndex = 0;

            PlayCard(wipe2);
            QuickHPCheck(-4, 0, -1);
        }
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
        [Test]
        public void TestRecklessAlienRacingTortoiseNotLeakCrashStatus()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            MoveAllCardsFromHandToDeck(gyrosaur);
            PlayCard("RecklessAlienRacingTortoise");
            PutInHand("IndiscriminatePass");
            PutInHand("WreckingBall");
            PutInHand("SphereOfDevastation");

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
        public void TestHyperspinNotLeakCrashStatus()
        {
            SetupGameController("BaronBlade", "Cauldron.Gyrosaur", "Legacy", "Ra", "Megalopolis");
            StartGame();
            DestroyNonCharacterVillainCards();

            MoveAllCardsFromHandToDeck(gyrosaur);
            DecisionYesNo = false;

            PlayCard("Hyperspin");

            Card pass = PutOnDeck("IndiscriminatePass");
            DrawCard(gyrosaur, optional: true);
            AssertOnTopOfDeck(pass);

            Card chase = PutOnDeck("AMerryChase");
            DrawCard(gyrosaur, optional: true);
            AssertOnTopOfDeck(chase);
        }
    }
}
