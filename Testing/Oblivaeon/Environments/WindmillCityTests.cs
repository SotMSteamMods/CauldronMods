using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace CauldronTests.Oblivaeon
{
    [TestFixture()]
    public class WindmillCityTests : CauldronBaseTest
    {

        #region WindmillCityHelperFunctions

        protected TurnTakerController windmill { get { return envOne; } }
        

        #endregion

        [Test()]
        public void TestWindmillCityLoads()
        {
            SetupGameController(new string[] { "OblivAeon", "Ra", "Legacy", "Haka", "Cauldron.WindmillCity", "MobileDefensePlatform", "InsulaPrimalis", "Cauldron.VaultFive", "Cauldron.Northspar" }, shieldIdentifier: "PrimaryObjective");
            StartGame();
            Assert.AreEqual(8, this.GameController.TurnTakerControllers.Count());
            AssertBattleZone(windmill, bzOne);

        }

        [Test()]
        public void TestBridgeDisaster()
        {
            SetupGameController(new string[] { "OblivAeon", "Ra", "Legacy", "Haka", "Tachyon", "Luminary", "Cauldron.WindmillCity", "MobileDefensePlatform", "InsulaPrimalis", "Cauldron.VaultFive", "Cauldron.Northspar" }, shieldIdentifier: "PrimaryObjective");
            StartGame();

            GoToPlayCardPhase(windmill);
            Card bridge = PlayCard("BridgeDisaster");
            Card turret = PlayCard("RegressionTurret");

            AssertCardSpecialString(bridge, 1, "4 hero targets with the highest HP: Haka, Legacy, Ra, Tachyon, Luminary.");

            SwitchBattleZone(haka);
            SwitchBattleZone(legacy);

            //in Bz 1: ra, tachyon, luminary, turret
            //in Bz 2: haka, legacy

            AssertCardsInBattleZone(bzOne, ra.CharacterCard, tachyon.CharacterCard, luminary.CharacterCard, turret);
            AssertCardsInBattleZone(bzTwo, haka.CharacterCard, legacy.CharacterCard);

            AssertCardSpecialString(bridge, 1, "4 hero targets with the highest HP: Ra, Tachyon, Luminary, Regression Turret.");

            //At the end of the environment turn, this card deals the {H - 1} hero targets with the highest HP 2 cold damage each.
            QuickHPStorage(ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard, tachyon.CharacterCard, luminary.CharacterCard, turret);
            GoToEndOfTurn(windmill);
            QuickHPCheck(-2, 0, 0, -2, -2, -2);

        }

        [Test()]
        public void TestCitywideCarnage()
        {
            SetupGameController(new string[] { "OblivAeon", "Ra", "Legacy", "Haka", "Tachyon", "Luminary", "Cauldron.WindmillCity", "MobileDefensePlatform", "InsulaPrimalis", "Cauldron.VaultFive", "Cauldron.Northspar" }, shieldIdentifier: "PrimaryObjective");
            StartGame();

            GoToPlayCardPhase(windmill);
            Card carnage = PlayCard("CitywideCarnage");
            Card turret = PlayCard("RegressionTurret");
            Card aeonWarrior = GetCard("AeonWarrior");
            Card aeonLocus = GetCard("AeonLocus");

            PlayCard(oblivaeon, aeonWarrior, overridePlayLocation: bzTwo.FindScion().PlayArea);
            PlayCard(oblivaeon, aeonLocus, overridePlayLocation: bzOne.FindScion().PlayArea);

            SwitchBattleZone(haka);
            SwitchBattleZone(legacy);

            //in Bz 1: ra, tachyon, luminary, turret
            //in Bz 2: haka, legacy

            AssertCardsInBattleZone(bzOne, aeonLocus, ra.CharacterCard, tachyon.CharacterCard, luminary.CharacterCard, turret);
            AssertCardsInBattleZone(bzTwo, aeonWarrior, haka.CharacterCard, legacy.CharacterCard);

            //At the end of the environment turn, this card deals each hero target 1 toxic damage and each villain target 1 energy damage.
            QuickHPStorage(aeonLocus, aeonWarrior, ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard, tachyon.CharacterCard, luminary.CharacterCard, turret);
            GoToEndOfTurn(windmill);
            QuickHPCheck(-1,0,-1, 0, 0, -1, -1, -1);

        }

        [Test()]
        public void TestCrackedWaterMain()
        {
            SetupGameController(new string[] { "OblivAeon", "Ra", "Legacy", "Haka", "Tachyon", "Luminary", "Cauldron.WindmillCity", "MobileDefensePlatform", "InsulaPrimalis", "Cauldron.VaultFive", "Cauldron.Northspar" }, shieldIdentifier: "PrimaryObjective");
            StartGame();

            GoToPlayCardPhase(windmill);
            Card waterMain = PlayCard("CrackedWaterMain");
            Card turret = PlayCard("RegressionTurret");
            Card aeonWarrior = GetCard("AeonWarrior");
            Card aeonLocus = GetCard("AeonLocus");

            PlayCard(oblivaeon, aeonWarrior, overridePlayLocation: bzTwo.FindScion().PlayArea);
            PlayCard(oblivaeon, aeonLocus, overridePlayLocation: bzOne.FindScion().PlayArea);

            SwitchBattleZone(haka);
            SwitchBattleZone(legacy);

            //in Bz 1: ra, tachyon, luminary, turret
            //in Bz 2: haka, legacy

            AssertCardsInBattleZone(bzOne, aeonLocus, ra.CharacterCard, tachyon.CharacterCard, luminary.CharacterCard, turret);
            AssertCardsInBattleZone(bzTwo, aeonWarrior, haka.CharacterCard, legacy.CharacterCard);

            //Reduce all damage dealt by non-environment cards by 1.

            //villain in bz 1 dealing damage
            QuickHPStorage(tachyon);
            DealDamage(aeonLocus, tachyon, 3, DamageType.Energy);
            QuickHPCheck(-2);

            //hero in bz 1 dealing damage
            QuickHPStorage(aeonLocus);
            DealDamage(tachyon, aeonLocus, 2, DamageType.Energy);
            QuickHPCheck(-1);

            //villain in bz 2 dealing damage
            QuickHPStorage(haka);
            DealDamage(aeonWarrior, haka, 3, DamageType.Energy);
            QuickHPCheck(-3);

            //hero in bz 2 dealing damage
            QuickHPStorage(aeonWarrior);
            DealDamage(haka, aeonWarrior, 2, DamageType.Energy);
            QuickHPCheck(-2);

        }

        [Test()]
        public void TestCrashedTanker()
        {
            SetupGameController(new string[] { "OblivAeon", "Ra", "Legacy", "Haka", "Tachyon", "Luminary", "Cauldron.WindmillCity", "MobileDefensePlatform", "InsulaPrimalis", "Cauldron.VaultFive", "Cauldron.Northspar" }, shieldIdentifier: "PrimaryObjective");
            StartGame();

            GoToPlayCardPhase(windmill);
            Card tanker = PlayCard("CrashedTanker");
            Card turret = PlayCard("RegressionTurret");

            AssertCardSpecialString(tanker, 1, "Target with the second highest HP: Haka.");

            SwitchBattleZone(haka);
            SwitchBattleZone(legacy);

            //in Bz 1: ra, tachyon, luminary, turret
            //in Bz 2: haka, legacy

            AssertCardsInBattleZone(bzOne, ra.CharacterCard, tachyon.CharacterCard, luminary.CharacterCard, turret);
            AssertCardsInBattleZone(bzTwo, haka.CharacterCard, legacy.CharacterCard);

            AssertCardSpecialString(tanker, 1, "Target with the second highest HP: Ra.");

            //At the end of the environment turn, this card deals the target with the second highest HP {H - 1} fire damage.
            QuickHPStorage(ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard, tachyon.CharacterCard, luminary.CharacterCard, turret);
            GoToEndOfTurn(windmill);
            QuickHPCheck(-4, 0, 0, 0, 0, 0);

        }

        [Test()]
        public void TestFrameJob()
        {
            SetupGameController(new string[] { "OblivAeon", "Ra", "Legacy", "Haka", "Tachyon", "Luminary", "Cauldron.WindmillCity", "MobileDefensePlatform", "InsulaPrimalis", "Cauldron.VaultFive", "Cauldron.Northspar" }, shieldIdentifier: "PrimaryObjective");
            StartGame();

            GoToPlayCardPhase(windmill);
            Card responder = PutOnDeck("DetectiveSedrick");
            Card frameJob = PlayCard("FrameJob");
            Card turret = PlayCard("RegressionTurret");
            Card aeonWarrior = GetCard("AeonWarrior");
            Card aeonLocus = GetCard("AeonLocus");

            AssertCardSpecialString(frameJob, 0, "Hero target with the highest HP: Legacy.");

            PlayCard(oblivaeon, aeonWarrior, overridePlayLocation: bzTwo.FindScion().PlayArea);
            PlayCard(oblivaeon, aeonLocus, overridePlayLocation: bzOne.FindScion().PlayArea);

            SwitchBattleZone(haka);
            SwitchBattleZone(legacy);

            //in Bz 1: ra, tachyon, luminary, turret
            //in Bz 2: haka, legacy

            AssertCardsInBattleZone(bzOne, aeonLocus, ra.CharacterCard, tachyon.CharacterCard, luminary.CharacterCard, turret);
            AssertCardsInBattleZone(bzTwo, aeonWarrior, haka.CharacterCard, legacy.CharacterCard);

            AssertCardSpecialString(frameJob, 0, "Hero target with the highest HP: Ra.");

            //Redirect all damage dealt by Responders to the hero target with the highest HP.
            QuickHPStorage(aeonWarrior, aeonLocus, ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard, tachyon.CharacterCard, luminary.CharacterCard, turret);
            DealDamage(responder, aeonLocus, 5, DamageType.Projectile);
            QuickHPCheck(0, 0, -5, 0, 0, 0, 0, 0);

        }

        [Test()]
        public void TestGearlock()
        {
            SetupGameController(new string[] { "OblivAeon", "Ra", "Legacy", "Haka", "Tachyon", "Luminary", "Cauldron.WindmillCity", "MobileDefensePlatform", "InsulaPrimalis", "Cauldron.VaultFive", "Cauldron.Northspar" }, shieldIdentifier: "PrimaryObjective");
            StartGame();

            GoToPlayCardPhase(windmill);
            Card gearlock = PlayCard("Gearlock");

            SwitchBattleZone(haka);
            SwitchBattleZone(legacy);

            //in Bz 1: ra, tachyon, luminary
            //in Bz 2: haka, legacy

            AssertCardsInBattleZone(bzOne, ra.CharacterCard, tachyon.CharacterCard, luminary.CharacterCard);
            AssertCardsInBattleZone(bzTwo, haka.CharacterCard, legacy.CharacterCard);

            //When this card is reduced to 0 HP, play the top card of each hero deck in turn order.
            Card raTop = PutOnDeck("FlameBarrier");
            Card legacyTop = PutOnDeck("NextEvolution");
            Card hakaTop = PutOnDeck("Mere");
            Card tachyonTop = PutOnDeck("PushingTheLimits");
            Card luminaryTop = PutOnDeck("RepairNanites");

            DealDamage(ra, gearlock, 20, DamageType.Fire, true);
            AssertInTrash(gearlock);
            AssertIsInPlay(raTop);
            AssertOnTopOfDeck(legacyTop);
            AssertOnTopOfDeck(hakaTop);
            AssertIsInPlay(tachyonTop);
            AssertIsInPlay(luminaryTop);
        }

    }
}
