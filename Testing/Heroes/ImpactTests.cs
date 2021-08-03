using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using Cauldron.Impact;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CauldronTests
{
    [TestFixture()]
    public class ImpactTests : CauldronBaseTest
    {
        #region ImpactHelperFunctions
        private void SetupIncap(TurnTakerController villain)
        {
            SetHitPoints(impact.CharacterCard, 1);
            DealDamage(villain, impact, 2, DamageType.Melee);
        }

        private DamageType DTM = DamageType.Melee;

        #endregion
        [Test]
        public void TestImpactLoads()
        {
            SetupGameController("BaronBlade", "Cauldron.Impact", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(impact);
            Assert.IsInstanceOf(typeof(ImpactCharacterCardController), impact.CharacterCardController);

            Assert.AreEqual(29, impact.CharacterCard.HitPoints);
        }
        [Test]
        public void TestImpactDecklist()
        {
            SetupGameController("BaronBlade", "Cauldron.Impact", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            AssertHasKeyword("ongoing", new[]
            {
                "DecayingOrbit",
                "GravitationalLensing",
                "GraviticOrb",
                "HurledObstruction",
                "InescapablePull",
                "LocalMicrogravity",
                "Meditate",
                "RepulsionField",
                "SlingshotTrajectory",
                "SpatialFinesse"
            });

            AssertHasKeyword("limited", new[]
            {
                "GravitationalLensing",
                "LocalMicrogravity",
                "RepulsionField"
            });

            AssertHasKeyword("one-shot", new[]
            {
                "AcceleratedCollision",
                "CrushingRift",
                "EscapeVelocity",
                "MassDriver"
            });
        }
        [Test]
        public void TestImpactPowerSimple()
        {
            SetupGameController("BaronBlade", "Cauldron.Impact", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DestroyCard("MobileDefensePlatform");

            QuickHPStorage(baron);
            UsePower(impact);
            QuickHPCheck(-1);
        }
        [Test]
        public void TestImpactPowerDestroyOngoing()
        {
            SetupGameController("BaronBlade", "Cauldron.Impact", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DestroyCard("MobileDefensePlatform");

            DecisionYesNo = true;
            QuickHPStorage(baron);
            Card moko = PlayCard("TaMoko");
            UsePower(impact);
            QuickHPCheck(-3);
            AssertInTrash(moko);
            UsePower(impact);
            QuickHPCheck(-1);
        }
        [Test]
        public void TestImpactPowerDestroyOngoingIsOptional()
        {
            SetupGameController("BaronBlade", "Cauldron.Impact", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DestroyCard("MobileDefensePlatform");

            DecisionYesNo = false;
            QuickHPStorage(baron);
            Card moko = PlayCard("TaMoko");
            UsePower(impact);
            QuickHPCheck(-1);
            AssertIsInPlay(moko);
            UsePower(impact);
            QuickHPCheck(-1);
        }
        [Test]
        public void TestImpactPowerNextDamagePrevented()
        {
            SetupGameController("CitizensHammerAndAnvilTeam", "Cauldron.Impact", "Megalopolis");
            StartGame();

            PlayCard("ScorchingSnap");
            AssertNumberOfStatusEffectsInPlay(1);

            Card shield = GetCardInPlay("HammerAndShield");
            DecisionSelectTarget = shield;
            QuickHPStorage(shield);

            UsePower(impact);
            DealDamage(impact, shield, 1, DamageType.Melee);
            QuickHPCheck(-1);
        }
        [Test]
        public void TestImpactPowerNoDamagePossible()
        {
            SetupGameController("CitizensHammerAndAnvilTeam", "Cauldron.Impact", "TheWraith", "WagnerMarsBase");
            StartGame();

            DecisionSelectTarget = impact.CharacterCard;
            PlayCard("ThroatJab");

            Card shield = GetCardInPlay("HammerAndShield");
            DecisionSelectTarget = shield;
            QuickHPStorage(shield);

            AssertNextMessageContains("from dealing damage");
            UsePower(impact);
        }
        [Test]
        public void TestImpactIncap1()
        {
            SetupGameController("BaronBlade", "Cauldron.Impact", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();
            DestroyCard("MobileDefensePlatform");

            SetupIncap(baron);
            AssertIncapLetsHeroUsePower(impact, 0, haka);
        }
        [Test]
        public void TestImpactIncap2()
        {
            SetupGameController("BaronBlade", "Cauldron.Impact", "Haka", "Ra", "TheScholar", "Megalopolis");
            StartGame();
            DestroyCard("MobileDefensePlatform");
            SetupIncap(baron);
            DecisionSelectCards = new Card[] { haka.CharacterCard, baron.CharacterCard, ra.CharacterCard, baron.CharacterCard };
            PlayCard("TheStaffOfRa");

            QuickHPStorage(baron);
            UseIncapacitatedAbility(impact, 1);
            QuickHPCheck(-1);
            UseIncapacitatedAbility(impact, 1);
            QuickHPCheck(-2);
        }
        [Test]
        public void TestImpactIncap3()
        {
            SetupGameController("BaronBlade", "Cauldron.Impact", "Haka", "Bunker", "TheScholar", "DokThorathCapital");
            StartGame();
            DestroyCard("MobileDefensePlatform");

            Card mil = PlayCard("ThorathianMilitary");
            QuickHPStorage(mil);
            DealDamage(haka, mil, 1, DamageType.Melee);
            QuickHPCheck(0);
            UseIncapacitatedAbility(impact, 2);
            DealDamage(haka, mil, 1, DamageType.Melee);
            QuickHPCheck(-1);

            GoToStartOfTurn(impact);
            DealDamage(haka, mil, 1, DamageType.Melee);
            QuickHPCheck(0);
        }
        [Test]
        public void TestAcceleratedCollision()
        {
            SetupGameController("BaronBlade", "Cauldron.Impact", "Haka", "Bunker", "TheScholar", "DokThorathCapital");
            StartGame();
            DestroyCard("MobileDefensePlatform");

            PutOnDeck(impact, impact.HeroTurnTaker.Hand.Cards);
            Card coll1 = PutInHand("AcceleratedCollision");
            Card coll2 = PutInHand("AcceleratedCollision");
            Card fin = PutInHand("SpatialFinesse");
            Assert.AreNotSame(coll1, coll2, "Somehow managed to get the same card twice");

            DecisionSelectCards = new Card[] { baron.CharacterCard, coll2, baron.CharacterCard, null };
            QuickHPStorage(baron);
            PlayCard(coll1);
            //got hit with 2 copies of Accelerated Collision
            QuickHPCheck(-4);

            AssertInTrash(coll1, coll2);
            //extra play is optional
            AssertInHand(fin);
        }
        [Test]
        public void TestCrushingRift()
        {
            SetupGameController("BaronBlade", "Cauldron.Impact", "Haka", "Bunker", "TheScholar", "DokThorathCapital");
            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            SetHitPoints(mdp, 9);
            QuickHPStorage(mdp);
            PlayCard("CrushingRift");
            QuickHPCheck(-4);
        }
        [Test]
        public void TestCrushingRiftIgnoresImmunity()
        {
            SetupGameController("BaronBlade", "Cauldron.Impact", "Haka", "Bunker", "TheScholar", "RealmOfDiscord");
            StartGame();

            PlayCard("ClaustrophobicDelusion");
            PlayCard("BladeBattalion");
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            SetHitPoints(mdp, 9);
            QuickHPStorage(mdp);
            DealDamage(impact, mdp, 1, DamageType.Melee);
            QuickHPCheck(0);
            PlayCard("CrushingRift");
            QuickHPCheck(-4);
        }
        [Test]
        public void TestCrushingRiftIgnoredUndamagedAndCharacter()
        {
            SetupGameController("BaronBlade", "Cauldron.Impact", "Haka", "Bunker", "TheScholar", "TheWraith", "RealmOfDiscord");
            StartGame();

            Card batt = PlayCard("BladeBattalion");
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            SetHitPoints(batt, 4);
            SetHitPoints(baron, 20);
            QuickHPStorage(baron.CharacterCard, mdp, batt);
            AssertNoDecision();
            PlayCard("CrushingRift");
            QuickHPCheck(0, 0, -2);
        }
        [Test]
        public void TestDecayingOrbitPlay()
        {
            SetupGameController("BaronBlade", "Cauldron.Impact", "Haka", "Bunker", "TheScholar", "TheWraith", "RealmOfDiscord");
            StartGame();
            Card mdp = GetCardInPlay("MobileDefensePlatform");

            DecisionSelectCards = new Card[] { mdp };

            QuickHPStorage(mdp);
            PlayCard("DecayingOrbit");
            QuickHPCheck(-2);
        }
        [Test]
        public void TestDecayingOrbitStartOfTurnOptional()
        {
            SetupGameController("BaronBlade", "Cauldron.Impact", "Haka", "Bunker", "TheScholar", "TheWraith", "RealmOfDiscord");
            StartGame();
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DecisionSelectCards = new Card[] { mdp, null };
            
            Card orbit = PlayCard("DecayingOrbit");

            QuickHPStorage(mdp);
            GoToStartOfTurn(impact);
            QuickHPCheck(0);
            AssertIsInPlay(orbit);
        }
        [Test]
        public void TestDecayingOrbitStartOfTurnDamage()
        {
            SetupGameController("BaronBlade", "Cauldron.Impact", "Haka", "Bunker", "TheScholar", "TheWraith", "RealmOfDiscord");
            StartGame();
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            Card tamoko = PlayCard("TaMoko");
            Card lash = PlayCard("BacklashField");
            DecisionSelectCards = new Card[] { mdp, mdp };

            Card orbit = PlayCard("DecayingOrbit");

            //decide who to deal damage to, then Orbit is the only choice to destroy
            AssertMaxNumberOfDecisions(1);

            QuickHPStorage(mdp);
            GoToStartOfTurn(impact);
            QuickHPCheck(-2);
            AssertInTrash(orbit);
            AssertIsInPlay(tamoko);
        }
        [Test]
        public void TestEscapeVelocityBottomOfDeck()
        {
            SetupGameController("BaronBlade", "Cauldron.Impact", "Haka", "Bunker", "TheVisionary", "RealmOfDiscord");
            StartGame();
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            Card batt = PlayCard("BladeBattalion");
            Card turret = PlayCard("PoweredRemoteTurret");
            Card decoy = PlayCard("DecoyProjection");
            Card bubbles = PlayCard("ExplosiveBubbles");
            Card bonds = PlayCard("EtherealBonds");

            var viableTargets = new Card[] { mdp, turret, decoy, bonds };
            SetHitPoints(viableTargets, 1);
            SetHitPoints(haka, 1);
            SetHitPoints(baron, 1);

            AssertNextDecisionChoices(viableTargets, new Card[] { batt, bubbles, haka.CharacterCard, baron.CharacterCard });

            PlayCard("EscapeVelocity");
            AssertOnBottomOfDeck(mdp, 1);
            AssertOnBottomOfDeck(turret);
            AssertOnBottomOfDeck(decoy);
            AssertIsInPlay(bonds);
        }
        [Test]
        public void TestEscapeVelocityBottomOfDeckOneTarget()
        {
            SetupGameController("BaronBlade", "Cauldron.Impact", "Haka", "Bunker", "TheVisionary", "RealmOfDiscord");
            StartGame();
            Card mdp = GetCardInPlay("MobileDefensePlatform");

            var viableTargets = new Card[] { mdp };
            SetHitPoints(viableTargets, 1);

            PlayCard("EscapeVelocity");
            AssertOnBottomOfDeck(mdp);
        }
        [Test]
        public void TestEscapeVelocityDestroyOngoing()
        {
            SetupGameController("BaronBlade", "Cauldron.Impact", "Haka", "Bunker", "TheVisionary", "RealmOfDiscord");
            StartGame();

            Card lash = PlayCard("BacklashField");
            Card moko = PlayCard("TaMoko");

            DecisionSelectCards = new Card[] { lash, null };

            PlayCard("EscapeVelocity");
            AssertInTrash(lash);
            //make sure it is optional
            PlayCard("EscapeVelocity");
            AssertIsInPlay(moko);
        }
        [Test]
        public void TestGravitationalLensingPlay()
        {
            SetupGameController("BaronBlade", "Cauldron.Impact", "Haka", "Bunker", "TheVisionary", "RealmOfDiscord");
            StartGame();

            Card escape = PutOnDeck("EscapeVelocity");
            Card orbit = PutOnDeck("DecayingOrbit");

            DecisionSelectCard = escape;
            PlayCard("GravitationalLensing");
            AssertInTrash(orbit);
            AssertOnTopOfDeck(escape);
        }
        [Test]
        public void TestGravitationalLensingDamage()
        {
            SetupGameController("BaronBlade", "Cauldron.Impact", "Haka", "Bunker", "TheVisionary", "RealmOfDiscord");
            StartGame();

            PlayCard("GravitationalLensing");
            Card mdp = GetCardInPlay("MobileDefensePlatform");

            QuickHPStorage(mdp);
            DealDamage(impact, mdp, 1, DTM);
            QuickHPCheck(-2);
        }
        [Test]
        public void TestGraviticOrbPlay()
        {
            SetupGameController("BaronBlade", "Cauldron.Impact", "Haka", "Bunker", "TheVisionary", "RealmOfDiscord");
            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");

            DecisionSelectCard = mdp;
            QuickHPStorage(mdp);
            Card orb = PlayCard("GraviticOrb");

            QuickHPCheck(-2);
            AssertNextToCard(orb, mdp);
            DestroyCard(mdp);
            AssertIsInPlay(orb);
            AssertNotNextToCard(orb, mdp);
        }

        [Test]
        public void TestGraviticOrbPrevent()
        {
            SetupGameController("BaronBlade", "Cauldron.Impact", "Haka", "Bunker", "TheVisionary", "RealmOfDiscord");
            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");

            DecisionSelectCard = mdp;
            Card orb = PlayCard("GraviticOrb");

            QuickHPStorage(impact);
            DealDamage(mdp, impact, 5, DTM);
            QuickHPCheckZero();
            AssertInTrash(orb);
            DealDamage(mdp, impact, 5, DTM);
            QuickHPCheck(-5);
        }


        [Test]
        public void TestGraviticOrbDoubleOrb()
        {
            SetupGameController("BaronBlade", "Cauldron.Impact", "Haka", "Bunker", "TheVisionary", "RealmOfDiscord");
            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");

            DecisionSelectCard = mdp;
            Card orb1 = PlayCard("GraviticOrb", 0);
            Card orb2 = PlayCard("GraviticOrb", 1);

            DecisionAmbiguousCard = orb1;
            DealDamage(mdp, impact, 5, DTM);
            QuickHPCheckZero();
            AssertInTrash(orb1);
            AssertNotInTrash(orb2);
        }


        [Test]
        public void TestHurledObstructionPlay()
        {
            SetupGameController("BaronBlade", "Cauldron.Impact", "Haka", "Bunker", "TheVisionary", "RealmOfDiscord");
            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            DecisionSelectCards = new Card[] { mdp, haka.CharacterCard, null };
            QuickHPStorage(mdp, impact.CharacterCard, haka.CharacterCard, bunker.CharacterCard);
            PlayCard("HurledObstruction");
            QuickHPCheck(-1, 0, -1, 0);
        }
        [Test]
        public void TestHurledObstructionDamageReduction()
        {
            SetupGameController("BaronBlade", "Cauldron.Impact", "Haka", "Bunker", "TheVisionary", "RealmOfDiscord");
            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");

            PlayCard("HurledObstruction");

            QuickHPStorage(mdp, impact.CharacterCard, haka.CharacterCard);
            //reduces damage from villains to anywhere
            DealDamage(mdp, impact, 1, DTM);
            DealDamage(baron, mdp, 1, DTM);
            QuickHPCheck(0, 0, 0);

            //does not reduce other damage
            DealDamage(haka, impact, 1, DTM);
            DealDamage(impact, mdp, 1, DTM);
            QuickHPCheck(-1, -1, 0);
        }
        [Test]
        public void TestHurledObstructionSelfDestroy()
        {
            SetupGameController("BaronBlade", "Cauldron.Impact", "Haka", "Bunker", "TheVisionary", "RealmOfDiscord");
            StartGame();

            Card hurl = PlayCard("HurledObstruction");
            GoToStartOfTurn(impact);
            AssertInTrash(hurl);
        }
        [Test]
        public void TestInescapablePullDraw()
        {
            SetupGameController("BaronBlade", "Cauldron.Impact", "Haka", "Bunker", "TheVisionary", "RealmOfDiscord");
            StartGame();

            Card pull = PutInHand("InescapablePull");
            QuickHandStorage(impact);
            PlayCard(pull);
            QuickHandCheck(0);
        }
        [Test]
        public void TestInescapablePullDamage()
        {
            SetupGameController("BaronBlade", "Cauldron.Impact", "Haka", "Bunker", "TheVisionary", "RealmOfDiscord");
            StartGame();
            DestroyCard("MobileDefensePlatform");

            Card pull = PlayCard("InescapablePull");

            //can't hit anything if there's nothing that's been damaged
            QuickHPStorage(baron, impact, haka, bunker, visionary);
            UsePower(pull);
            QuickHPCheckZero();

            AssertMaxNumberOfDecisions(1);

            //no choice, must hit haka
            DealDamage(impact, haka, 1, DTM);
            UsePower(pull);
            QuickHPCheck(0, 0, -5, 0, 0);

            //may choose haka or blade
            DealDamage(impact, baron, 1, DTM);
            UsePower(pull);
            QuickHPCheck(-5, 0, 0, 0, 0);

            //turn reset, no damage options
            GoToStartOfTurn(impact);
            UsePower(pull);
            QuickHPCheckZero();
        }
        [Test]
        public void TestLocalMicrogravityPlay()
        {
            SetupGameController("BaronBlade", "Cauldron.Impact", "Haka", "Bunker", "TheVisionary", "RealmOfDiscord");
            StartGame();

            SetHitPoints(impact, 20);
            QuickHPStorage(impact);
            PlayCard("LocalMicrogravity");
            QuickHPCheck(1);
        }
        [Test]
        public void TestLocalMicrogravityPower()
        {
            SetupGameController("BaronBlade", "Cauldron.Impact", "Haka", "Bunker", "TheVisionary", "RealmOfDiscord");
            StartGame();
            DestroyCard("MobileDefensePlatform");

            Card micro = PlayCard("LocalMicrogravity");
            QuickHPStorage(baron);
            UsePower(micro);
            QuickHPCheck(-3);
            AssertInTrash(micro);
        }

        [Test]
        public void TestLocalMicrogravityPrevention()
        {
            SetupGameController("BaronBlade", "Cauldron.Impact", "Haka", "Bunker", "TheVisionary", "RealmOfDiscord");
            StartGame();

            GoToStartOfTurn(visionary);
            PlayCard("LocalMicrogravity");
            QuickHPStorage(impact);

            //not in non-environment
            DealDamage(haka, impact, 1, DTM);
            QuickHPCheck(-1);

            GoToStartOfTurn(FindEnvironment());
            DealDamage(haka, impact, 5, DTM);
            QuickHPCheck(0);

            //only once
            DealDamage(haka, impact, 5, DTM);
            QuickHPCheck(-5);
        }


        [Test]
        public void TestLocalMicrogravityCricketInteraction()
        {
            //from issue #617

            SetupGameController("BaronBlade", "Cauldron.Impact", "Cauldron.Cricket", "Bunker", "TheVisionary", "RealmOfDiscord");
            StartGame();

            GoToStartOfTurn(visionary);
            var micro = PlayCard("LocalMicrogravity");
            var distort = PlayCard("AcousticDistortion");

            GoToStartOfTurn(FindEnvironment());

            DecisionAmbiguousCard = micro;
            DecisionRedirectTarget = bunker.CharacterCard;

            QuickHPStorage(impact, bunker);
            DealDamage(bunker, impact, 5, DTM);
            QuickHPCheck(0, 0);
            PrintSeparator("Check only once");
            //only once
            DealDamage(bunker, impact, 5, DTM);
            QuickHPCheck(0, -5);
        }

        [Test]
        public void TestMassDriverDamage()
        {
            SetupGameController("BaronBlade", "Cauldron.Impact", "Haka", "Bunker", "TheVisionary", "RealmOfDiscord");
            StartGame();

            DestroyCard("MobileDefensePlatform");
            Card orb1 = PlayCard("DecayingOrbit");
            PlayCard("DecayingOrbit");

            QuickHPStorage(baron);

            //2 ongoings in play, 2 damage
            PlayCard("MassDriver");
            QuickHPCheck(-2);

            //1 ongoing in play, 1 damage
            DestroyCard(orb1);
            PlayCard("MassDriver");
            QuickHPCheck(-1);

            //does not count ally cards
            PlayCard("TaMoko");
            PlayCard("MassDriver");
            QuickHPCheck(-1);
        }
        [Test]
        public void TestMassDriverDestruction()
        {
            SetupGameController("BaronBlade", "Cauldron.Impact", "Haka", "Bunker", "TheVisionary", "Megalopolis");
            StartGame();

            Card traffic = PlayCard("TrafficPileup");
            Card police = PlayCard("PoliceBackup");

            DecisionSelectCard = traffic;

            PlayCard("MassDriver");
            AssertInTrash(traffic);
            AssertIsInPlay(police);
        }
        [Test]
        public void TestMeditateDraw()
        {
            SetupGameController("BaronBlade", "Cauldron.Impact", "Haka", "Bunker", "TheVisionary", "Megalopolis");
            StartGame();

            Card med = PutInHand("Meditate");
            Card orb = PutOnDeck("GraviticOrb");

            QuickHandStorage(impact);
            PlayCard(med);
            AssertInHand(orb);
            QuickHandCheck(0);
        }
        [Test]
        public void TestMeditateDamage()
        {
            SetupGameController("BaronBlade", "Cauldron.Impact", "Haka", "Bunker", "TheVisionary", "Megalopolis");
            StartGame();

            PlayCard("LivingForceField");
            DestroyCard("MobileDefensePlatform");
            DecisionYesNo = true;

            Card meditate = PlayCard("Meditate");

            QuickHPStorage(baron);
            DealDamage(impact, baron, 5, DTM);
            //5 -> 4 on the original damage, 4 -> 3 on the repeat
            QuickHPCheck(-7);

            AssertInTrash(meditate);
        }
        [Test]
        public void TestMeditateTriggerOptional()
        {
            SetupGameController("BaronBlade", "Cauldron.Impact", "Haka", "Bunker", "TheVisionary", "Megalopolis");
            StartGame();

            PlayCard("LivingForceField");
            DestroyCard("MobileDefensePlatform");
            DecisionYesNo = false;

            Card meditate = PlayCard("Meditate");

            QuickHPStorage(baron);
            DealDamage(impact, baron, 5, DTM);
            //5 -> 4 on the original damage, no repeat
            QuickHPCheck(-4);

            AssertIsInPlay(meditate);
        }
        [Test]
        public void TestRepulsionFieldPlay()
        {
            SetupGameController("BaronBlade", "Cauldron.Impact", "Haka", "Bunker", "TheVisionary", "Megalopolis");
            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            Card batt = PlayCard("BladeBattalion");
            Card traffic = PlayCard("TrafficPileup");

            QuickHPStorage(mdp, batt, traffic, impact.CharacterCard);
            PlayCard("RepulsionField");
            QuickHPCheck(-1, -1, -1, 0);
        }
        [Test]
        public void TestRepulsionFieldReduction()
        {
            SetupGameController("BaronBlade", "Cauldron.Impact", "Haka", "Bunker", "TheVisionary", "Megalopolis");
            StartGame();

            Card traffic = PlayCard("TrafficPileup");
            Card mdp = GetCardInPlay("MobileDefensePlatform");

            PlayCard("RepulsionField");

            QuickHPStorage(impact, haka);

            DealDamage(traffic, impact, 2, DTM);
            DealDamage(traffic, haka, 2, DTM);
            QuickHPCheck(-1, -2);

            DealDamage(mdp, impact, 2, DTM);
            DealDamage(mdp, haka, 2, DTM);
            QuickHPCheck(-1, -2);

            DealDamage(impact, impact, 2, DTM);
            DealDamage(impact, haka, 2, DTM);
            QuickHPCheck(-1, -2);
        }
        [Test]
        public void TestSlingshotTrajectoryPlay()
        {
            SetupGameController("BaronBlade", "Cauldron.Impact", "Haka", "Bunker", "TheVisionary", "Megalopolis");
            StartGame();

            DestroyCard("MobileDefensePlatform");

            QuickHPStorage(baron);
            PlayCard("SlingshotTrajectory");
            QuickHPCheck(-2);
        }
        [Test]
        public void TestSlingshotTrajectoryNoDestroy()
        {
            SetupGameController("BaronBlade", "Cauldron.Impact", "Haka", "Bunker", "TheVisionary", "Megalopolis");
            StartGame();

            DestroyCard("MobileDefensePlatform");

            Card sling = PlayCard("SlingshotTrajectory");

            QuickHPStorage(baron, impact, haka, bunker);
            DecisionSelectCards = new Card[] { null, baron.CharacterCard };
            UsePower(sling);
            QuickHPCheck(-2, 0, 0, 0);
        }
        [Test]
        public void TestSlingshotTrajectoryTwoDestroy()
        {
            SetupGameController("BaronBlade", "Cauldron.Impact", "Haka", "Bunker", "TheVisionary", "Megalopolis");
            StartGame();

            DestroyCard("MobileDefensePlatform");

            Card sling = PlayCard("SlingshotTrajectory");
            Card orb1 = PlayCard("GraviticOrb");
            Card orb2 = PlayCard("GraviticOrb");

            QuickHPStorage(baron, impact, haka, bunker);
            DecisionSelectCards = new Card[] { orb1, orb2, null, baron.CharacterCard, impact.CharacterCard, haka.CharacterCard, bunker.CharacterCard };
            UsePower(sling);
            QuickHPCheck(-2, -2, -2, 0);
            AssertInTrash(orb1, orb2);
        }
        [Test]
        public void TestSpatialFinessePlay()
        {
            SetupGameController("BaronBlade", "Cauldron.Impact", "Haka", "Bunker", "TheVisionary", "Megalopolis");
            StartGame();

            PutOnDeck(impact, impact.HeroTurnTaker.Hand.Cards);
            Card fin1 = PutInHand("SpatialFinesse");
            Card fin2 = PutInHand("SpatialFinesse");

            QuickHandStorage(impact);
            PlayCard(fin1);
            QuickHandCheck(1);
            AssertIsInPlay(fin1);

            PlayCard(fin2);
            QuickHandCheck(1);
            AssertInTrash(fin1);
            AssertIsInPlay(fin2);
        }
        [Test]
        public void TestSpatialFinesseRescue()
        {
            SetupGameController("BaronBlade", "Cauldron.Impact", "Haka", "Bunker", "TheVisionary", "Megalopolis");
            StartGame();

            DestroyCard("MobileDefensePlatform");

            Card orbit = PlayCard("DecayingOrbit");
            Card fin = PlayCard("SpatialFinesse");

            DecisionYesNo = true;
            QuickHPStorage(baron);

            DestroyCard(orbit);
            QuickHPCheck(-2);
            AssertIsInPlay(orbit);
            AssertInTrash(fin);
        }
        [Test]
        public void TestSpatialFinesseRescueOptional()
        {
            SetupGameController("BaronBlade", "Cauldron.Impact", "Haka", "Bunker", "TheVisionary", "Megalopolis");
            StartGame();

            DestroyCard("MobileDefensePlatform");

            Card orbit = PlayCard("DecayingOrbit");
            Card fin = PlayCard("SpatialFinesse");

            DecisionYesNo = false;
            QuickHPStorage(baron);

            DestroyCard(orbit);
            QuickHPCheck(0);
            AssertIsInPlay(fin);
            AssertInTrash(orbit);
        }
    }
}