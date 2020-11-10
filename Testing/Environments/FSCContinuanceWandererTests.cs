using Cauldron.FSCContinuanceWanderer;

using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace CauldronTests
{
    [TestFixture()]
    class FSCContinuanceWandererTests : BaseTest
    {
        private Game baseGame = new Game(new string[] { "Spite", "Legacy", "Ra", "Haka", "Cauldron.FSCContinuanceWanderer" });
        private Game vengeanceGame = new Game(new string[] { "ErmineTeam", "Legacy", "BiomancerTeam", "Ra", "FrictionTeam", "Haka", "Cauldron.FSCContinuanceWanderer" });
        protected TurnTakerController fsc { get { return FindEnvironment(); } }

        [Test()]
        public void TestLoadFSC()
        {
            SetupGameController(baseGame);
            Assert.AreEqual(5, this.GameController.TurnTakerControllers.Count());
        }

        [Test()]
        public void TestFSCDecklist()
        {
            SetupGameController(baseGame);
            Card borg = GetCard("CombatCyborg");
            AssertIsTarget(borg, 4);
            AssertCardHasKeyword(borg, "time monster", false);

            Card paradox = GetCard("ParadoxIntrusion");
            AssertIsTarget(paradox, 4);
            AssertCardHasKeyword(paradox, "time monster", false);

            Card behemoth = GetCard("PrehistoricBehemoth");
            AssertIsTarget(behemoth, 8);
            AssertCardHasKeyword(behemoth, "time monster", false);

            Card glitch = GetCard("VortexGlitch");
            AssertCardHasKeyword(glitch, "time vortex", false);

            Card interference = GetCard("VortexInterference");
            AssertCardHasKeyword(interference, "time vortex", false);

            Card surge = GetCard("VortexSurge");
            AssertCardHasKeyword(surge, "time vortex", false);
        }

        [Test()]
        public void TestCombatCyborgEndDamageVillain()
        {
            SetupGameController("BaronBlade", "Legacy", "Ra", "Haka", "Cauldron.FSCContinuanceWanderer");
            StartGame();
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            int mdpHitpoints = mdp.HitPoints ?? default;
            QuickHPStorage(ra, legacy, haka, baron);
            PlayCard("CombatCyborg");
            //At the end of the environment turn, this card deals non-environment the target with the lowest HP (H)-2 projectile damage.
            GoToEndOfTurn(env);
            QuickHPCheck(0, 0, 0, 0);
            Assert.AreEqual(mdpHitpoints - 1, mdp.HitPoints);
        }

        [Test()]
        public void TestCombatCyborgEndDamageHero()
        {
            SetupGameController(baseGame);
            StartGame();
            GoToPlayCardPhase(env);
            QuickHPStorage(ra, legacy, haka, spite);
            PlayCard("CombatCyborg");
            //At the end of the environment turn, this card deals non-environment the target with the lowest HP (H)-2 projectile damage.
            GoToEndOfTurn(env);
            QuickHPCheck(-1, 0, 0, 0);
        }

        [Test()]
        public void TestCombatCyborgEndNotDamageEnvironment()
        {
            SetupGameController(baseGame);
            StartGame();
            Card intrusion = GetCard("ParadoxIntrusion");
            GoToPlayCardPhase(env);
            PlayCard("CombatCyborg");
            PlayCard(intrusion);
            QuickHPStorage(intrusion);
            //At the end of the environment turn, this card deals non-environment the target with the lowest HP (H)-2 projectile damage.
            GoToEndOfTurn(env);
            QuickHPCheck(0);
        }

        [Test()]
        public void TestCombatCyborgReduceDamage()
        {
            SetupGameController(baseGame);
            StartGame();
            Card intrusion = GetCard("ParadoxIntrusion");
            Card borg = GetCard("CombatCyborg");
            PlayCard(borg);
            PlayCard(intrusion);
            //Reduce damage dealt to environment targets by 2.
            QuickHPStorage(intrusion);
            DealDamage(ra, intrusion, 3, DamageType.Fire);
            QuickHPCheck(-1);
            //Reduce damage dealt to environment targets by 2.
            QuickHPStorage(borg);
            DealDamage(ra, borg, 3, DamageType.Fire);
            QuickHPCheck(-1);
        }
        [Test()]
        public void TestHeartOfTheWandererDestroySelf()
        {
            SetupGameController(baseGame);
            StartGame();
            Card heart = GetCard("HeartOfTheWanderer");
            PlayCard(heart);
            //At the end of the environment turn, destroy this card.
            GoToStartOfTurn(env);
            AssertInPlayArea(env, heart);
            GoToEndOfTurn(env);
            AssertInTrash(env, heart);
        }

        [Test()]
        public void TestHeartOfTheWandererDiscard()
        {
            SetupGameController(baseGame);
            StartGame();
            Card heart = GetCard("HeartOfTheWanderer");
            Card lab = GetCard("LabRaid");
            PutOnDeck(spite, lab);
            DecisionMoveCardDestinations = new MoveCardDestination[] {
                new MoveCardDestination(spite.TurnTaker.Trash),
                new MoveCardDestination(legacy.TurnTaker.Trash),
                new MoveCardDestination(ra.TurnTaker.Trash),
                new MoveCardDestination(haka.TurnTaker.Trash),
                new MoveCardDestination(fsc.TurnTaker.Trash)
            };
            PlayCard(heart);
            //When this card enters play, reveal the top card of each deck in turn order and either discard it or replace it.
            AssertNumberOfCardsInTrash(spite, 1);
            AssertNumberOfCardsInTrash(legacy, 1);
            AssertNumberOfCardsInTrash(ra, 1);
            AssertNumberOfCardsInTrash(haka, 1);
            AssertNumberOfCardsInTrash(fsc, 1);
        }

        [Test()]
        public void TestHeartOfTheWandererReturn()
        {
            SetupGameController(baseGame);
            StartGame();
            Card heart = GetCard("HeartOfTheWanderer");
            DecisionMoveCardDestinations = new MoveCardDestination[] {
                new MoveCardDestination(spite.TurnTaker.Deck),
                new MoveCardDestination(legacy.TurnTaker.Deck),
                new MoveCardDestination(ra.TurnTaker.Deck),
                new MoveCardDestination(haka.TurnTaker.Deck),
                new MoveCardDestination(fsc.TurnTaker.Deck)
            };
            PlayCard(heart);
            //When this card enters play, reveal the top card of each deck in turn order and either discard it or replace it.
            AssertNumberOfCardsInTrash(spite, 0);
            AssertNumberOfCardsInTrash(legacy, 0);
            AssertNumberOfCardsInTrash(ra, 0);
            AssertNumberOfCardsInTrash(haka, 0);
            AssertNumberOfCardsInTrash(fsc, 0);
        }

        [Test()]
        public void TestHeartOfTheWandererTeamVillainDiscard()
        {
            SetupGameController(vengeanceGame);
            StartGame();
            Card heart = GetCard("HeartOfTheWanderer");
            DecisionMoveCardDestinations = new MoveCardDestination[] {
                new MoveCardDestination(ermineTeam.TurnTaker.Trash),
                new MoveCardDestination(legacy.TurnTaker.Trash),
                new MoveCardDestination(biomancerTeam.TurnTaker.Trash),
                new MoveCardDestination(ra.TurnTaker.Trash),
                new MoveCardDestination(frictionTeam.TurnTaker.Trash),
                new MoveCardDestination(haka.TurnTaker.Trash),
                new MoveCardDestination(fsc.TurnTaker.Trash)
            };
            PlayCard(heart);
            //When this card enters play, reveal the top card of each deck in turn order and either discard it or replace it.
            AssertNumberOfCardsInTrash(ermineTeam, 1);
            AssertNumberOfCardsInTrash(legacy, 1);
            AssertNumberOfCardsInTrash(biomancerTeam, 1);
            AssertNumberOfCardsInTrash(ra, 1);
            AssertNumberOfCardsInTrash(frictionTeam, 1);
            AssertNumberOfCardsInTrash(haka, 1);
            AssertNumberOfCardsInTrash(fsc, 1);
        }

        [Test()]
        public void TestParadoxIntrusionEndTurnDamage0Vortex()
        {
            SetupGameController("Spite", "Guise", "Parse", "Haka", "Cauldron.FSCContinuanceWanderer");
            StartGame();
            GoToPlayCardPhase(env);
            PlayCard("ParadoxIntrusion");
            //At the end of the environment turn, this card deals the hero target with the highest HP {H} energy damage.
            //Then, this card deals X villain targets 2 energy damage each, where x is the number of time vortex cards in the environment trash.
            QuickHPStorage(haka, spite);
            GoToEndOfTurn(env);
            QuickHPCheck(-3, 0);
        }

        [Test()]
        public void TestParadoxIntrusionEndTurnDamage2Vortex()
        {
            SetupGameController("LaCapitan", "Guise", "Parse", "Haka", "Cauldron.FSCContinuanceWanderer");
            StartGame();
            Card boat = GetCardInPlay("LaParadojaMagnifica");
            PutInTrash("VortexSurge", "VortexGlitch");
            GoToPlayCardPhase(env);
            PlayCard("ParadoxIntrusion");
            //At the end of the environment turn, this card deals the hero target with the highest HP {H} energy damage.
            //Then, this card deals X villain targets 2 energy damage each, where x is the number of time vortex cards in the environment trash.
            QuickHPStorage(haka.CharacterCard, capitan.CharacterCard, boat);
            GoToEndOfTurn(env);
            QuickHPCheck(-3, -2, -2);
        }

        [Test()]
        public void TestPrehistoricBehemothEndDamage()
        {
            //This card is immune to damage dealt by targets with less than 10HP.
            SetupGameController("LaCapitan", "Guise", "Parse", "Haka", "Cauldron.FSCContinuanceWanderer");
            StartGame();
            GoToPlayCardPhase(env);
            PlayCard("PrehistoricBehemoth");
            //At the end of the environment turn, this card deals the {H - 2} hero target 2 melee damage each.
            QuickHPStorage(haka, parse, guise, capitan);
            GoToEndOfTurn(env);
            QuickHPCheck(-2, 0, 0, 0);
        }

        [Test()]
        public void TestPrehistoricBehemothImmune()
        {
            //This card is immune to damage dealt by targets with less than 10HP.
            SetupGameController("LaCapitan", "Guise", "Parse", "Haka", "Cauldron.FSCContinuanceWanderer");
            StartGame();
            Card behemoth = GetCard("PrehistoricBehemoth");
            PlayCard(behemoth);
            //Source HP > 10
            QuickHPStorage(behemoth);
            DealDamage(haka, behemoth, 2, DamageType.Melee);
            QuickHPCheck(-2);
            //Source HP < 10
            SetHitPoints(haka, 8);
            QuickHPStorage(behemoth);
            DealDamage(haka, behemoth, 2, DamageType.Melee);
            QuickHPCheck(0);
        }

        [Test()]
        public void TestSuperimposedRealities()
        {
            Assert.IsTrue(false);
        }

        [Test()]
        public void TestTemporalAccelerationDestroySelf()
        {
            //When this card enters play, play the top card of the villain deck. Then, play the top card of each hero deck in turn order.
            SetupGameController(baseGame);
            StartGame();
            Card accel = GetCard("TemporalAcceleration");
            PlayCard(accel);
            //At the end of the environment turn, destroy this card.
            GoToStartOfTurn(env);
            AssertInPlayArea(env, accel);
            GoToEndOfTurn(env);
            AssertInTrash(env, accel);
        }

        [Test()]
        public void TestTemporalAccelerationPlayCards()
        {
            SetupGameController("BaronBlade", "Legacy", "Ra", "Haka", "Cauldron.FSCContinuanceWanderer");
            StartGame();
            Card field = GetCard("BacklashField");
            PutOnDeck(baron, field);
            Card ring = GetCard("TheLegacyRing");
            PutOnDeck(legacy, ring);
            Card staff = GetCard("TheStaffOfRa");
            PutOnDeck(ra, staff);
            Card mere = GetCard("Mere");
            PutOnDeck(haka, mere);
            //When this card enters play, play the top card of the villain deck. Then, play the top card of each hero deck in turn order.
            PlayCard(GetCard("TemporalAcceleration"));
            AssertInPlayArea(baron, field);
            AssertInPlayArea(legacy, ring);
            AssertInPlayArea(ra, staff);
            AssertInPlayArea(haka, mere);
        }

        [Test()]
        public void TestTemporalReset()
        {
            //This card is immune to damage dealt by targets with less than 10HP.
            SetupGameController("LaCapitan", "Guise", "Parse", "Haka", "Cauldron.FSCContinuanceWanderer");
            StartGame();
            Card behemoth = GetCard("PrehistoricBehemoth");
            PlayCard(behemoth);
            Assert.IsTrue(false);
        }
    }
}
