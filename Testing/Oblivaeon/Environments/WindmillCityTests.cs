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
            QuickHPCheck(-1, 0, -1, 0, 0, -1, -1, -1);

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


        [Test()]
        public void TestGrayPharmaceuticalBuilding()
        {
            SetupGameController(new string[] { "OblivAeon", "Ra", "Legacy", "Haka", "Tachyon", "Luminary", "Cauldron.WindmillCity", "MobileDefensePlatform", "InsulaPrimalis", "Cauldron.VaultFive", "Cauldron.Northspar" }, shieldIdentifier: "PrimaryObjective");
            StartGame();

            GoToPlayCardPhase(windmill);
            Card grayBuilding = PlayCard("GrayPharmaceuticalBuilding");
            Card aeonWarrior = GetCard("AeonWarrior");
            Card aeonLocus = GetCard("AeonLocus");
            Card mere = PlayCard("Mere");
            Card tornado = PlayCard("BlazingTornado");

            PlayCard(oblivaeon, aeonWarrior, overridePlayLocation: bzTwo.FindScion().PlayArea);
            PlayCard(oblivaeon, aeonLocus, overridePlayLocation: bzOne.FindScion().PlayArea);

            SwitchBattleZone(haka);
            SwitchBattleZone(legacy);

            //in Bz 1: ra, tachyon, luminary, turret
            //in Bz 2: haka, legacy

            AssertCardsInBattleZone(bzOne, aeonLocus, ra.CharacterCard, tachyon.CharacterCard, luminary.CharacterCard);
            AssertCardsInBattleZone(bzTwo, aeonWarrior, haka.CharacterCard, legacy.CharacterCard);


            //Whenever a hero uses a power that deals damage, increase that damage by 2.
            DecisionSelectTargets = new Card[] { aeonLocus, aeonWarrior, luminary.CharacterCard, legacy.CharacterCard };

            //check with a power in bz one
            QuickHPStorage(aeonLocus);
            UsePower(ra.CharacterCard);
            QuickHPCheck(-4);

            //check with a power in bz two
            QuickHPStorage(aeonWarrior);
            UsePower(haka.CharacterCard);
            QuickHPCheck(-2);

            //After a hero uses a power on a non-character card, destroy that card.

            //bz 1
            UsePower(tornado);
            AssertInTrash(tornado);

            //bz 2
            UsePower(mere);
            AssertInPlayArea(haka, mere);
        }

        [Test()]
        public void TestInjuredWorker()
        {
            SetupGameController(new string[] { "OblivAeon", "Ra", "Legacy", "Haka", "Tachyon", "Luminary", "Cauldron.WindmillCity", "MobileDefensePlatform", "InsulaPrimalis", "Cauldron.VaultFive", "Cauldron.Northspar" }, shieldIdentifier: "PrimaryObjective");
            StartGame();

            GoToPlayCardPhase(windmill);
            Card worker = GetCard("InjuredWorker");
            Card turret = PlayCard("RegressionTurret");

            AssertCardSpecialString(worker, 1, "Hero with the highest HP: Haka.");

            SwitchBattleZone(haka);
            SwitchBattleZone(legacy);

            //in Bz 1: ra, tachyon, luminary, turret
            //in Bz 2: haka, legacy

            AssertCardsInBattleZone(bzOne, ra.CharacterCard, tachyon.CharacterCard, luminary.CharacterCard, turret);
            AssertCardsInBattleZone(bzTwo, haka.CharacterCard, legacy.CharacterCard);

            AssertCardSpecialString(worker, 1, "Hero with the highest HP: Ra.");

            //Then move this card next to the hero with the highest HP.           
            PlayCard(worker);
            AssertNextToCard(worker, ra.CharacterCard);

        }

        [Test()]
        public void TestIntrepidReporter()
        {
            SetupGameController(new string[] { "OblivAeon", "Ra", "Legacy", "Haka", "Tachyon", "Luminary", "Cauldron.WindmillCity", "MobileDefensePlatform", "InsulaPrimalis", "Cauldron.VaultFive", "Cauldron.Northspar" }, shieldIdentifier: "PrimaryObjective");
            StartGame();

            Card reporter = GetCard("IntrepidReporter");

            SwitchBattleZone(haka);
            SwitchBattleZone(legacy);

            //in Bz 1: ra, tachyon, luminary, turret
            //in Bz 2: haka, legacy

            AssertCardsInBattleZone(bzOne, ra.CharacterCard, tachyon.CharacterCard, luminary.CharacterCard);
            AssertCardsInBattleZone(bzTwo, haka.CharacterCard, legacy.CharacterCard);

            //When this card enters play, 2 players may each draw a card.
            AssertNextDecisionChoices(notIncluded: new List<TurnTaker>() { haka.TurnTaker, legacy.TurnTaker });
            PlayCard(reporter);          

        }

        [Test()]
        public void TestIronWasp()
        {
            SetupGameController(new string[] { "OblivAeon", "Ra", "Legacy", "Haka", "Tachyon", "Luminary", "Cauldron.WindmillCity", "MobileDefensePlatform", "InsulaPrimalis", "Cauldron.VaultFive", "Cauldron.Northspar" }, shieldIdentifier: "PrimaryObjective");
            StartGame();

            GoToBeforeStartOfTurn(windmill);
            Card wasp = PlayCard("IronWasp");

            SwitchBattleZone(haka);
            SwitchBattleZone(legacy);

            //in Bz 1: ra, tachyon, luminary
            //in Bz 2: haka, legacy

            AssertCardsInBattleZone(bzOne, ra.CharacterCard, tachyon.CharacterCard, luminary.CharacterCard);
            AssertCardsInBattleZone(bzTwo, haka.CharacterCard, legacy.CharacterCard);

            //At the start of the environment turn, this card deals each hero target X melee damage, where X is the current HP of this card.
            QuickHPStorage(ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard, tachyon.CharacterCard, luminary.CharacterCard);
            GoToStartOfTurn(windmill);
            QuickHPCheck(-8, 0, 0, -8, -8);

        }

        [Test()]
        public void TestRainOfDebris()
        {
            SetupGameController(new string[] { "OblivAeon", "Ra", "Legacy", "Haka", "Tachyon", "Luminary", "Cauldron.WindmillCity", "MobileDefensePlatform", "InsulaPrimalis", "Cauldron.VaultFive", "Cauldron.Northspar" }, shieldIdentifier: "PrimaryObjective");
            StartGame();

            GoToPlayCardPhase(windmill);
            Card rain = PlayCard("RainOfDebris");

            SwitchBattleZone(haka);
            SwitchBattleZone(legacy);

            //in Bz 1: ra, tachyon, luminary
            //in Bz 2: haka, legacy

            AssertCardsInBattleZone(bzOne, ra.CharacterCard, tachyon.CharacterCard, luminary.CharacterCard);
            AssertCardsInBattleZone(bzTwo, haka.CharacterCard, legacy.CharacterCard);

            //At the end of the environment turn, each hero may discard a card. This card deals any hero that did not discard a card this way 2 melee damage.
            Card raDiscard = GetRandomCardFromHand(ra);
            Card luminaryDiscard = GetRandomCardFromHand(luminary);
            DecisionSelectCards = new Card[] { raDiscard, null, luminaryDiscard };

            QuickHPStorage(ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard, tachyon.CharacterCard, luminary.CharacterCard);
            QuickHandStorage(ra, legacy, haka, tachyon, luminary);
            GoToEndOfTurn(windmill);
            QuickHPCheck(0, 0, 0, -2, 0);
            QuickHandCheck(-1, 0, 0, 0, -1);
            AssertInTrash(raDiscard);
            AssertInTrash(luminaryDiscard);
        }

        [Test()]
        public void TestSaveTheDay()
        {
            SetupGameController(new string[] { "OblivAeon", "Ra", "Fanatic", "Haka", "Tachyon", "Luminary", "Cauldron.WindmillCity", "MobileDefensePlatform", "InsulaPrimalis", "Cauldron.VaultFive", "Cauldron.Northspar" }, shieldIdentifier: "PrimaryObjective");
            StartGame();

            GoToPlayCardPhase(windmill);
            Card responder1 = PutOnDeck("DetectiveSedrick");
            Card saveDay = PlayCard("SaveTheDay");
            Card aeonWarrior = GetCard("AeonWarrior");
            Card aeonThrall = GetCard("AeonThrall");

            Card aeonLocus = GetCard("AeonLocus");
            Card gaze = PlayCard("WrathfulGaze");


            PlayCard(oblivaeon, aeonWarrior, overridePlayLocation: bzTwo.FindScion().PlayArea);
            PlayCard(oblivaeon, aeonThrall, overridePlayLocation: bzTwo.FindScion().PlayArea);
            PlayCard(oblivaeon, aeonLocus, overridePlayLocation: bzOne.FindScion().PlayArea);

            SwitchBattleZone(haka);
            SwitchBattleZone(fanatic);

            //in Bz 1: ra, tachyon, luminary
            //in Bz 2: haka, fanatic

            AssertCardsInBattleZone(bzOne, aeonLocus, ra.CharacterCard, tachyon.CharacterCard, luminary.CharacterCard);
            AssertCardsInBattleZone(bzTwo, aeonWarrior, aeonThrall, haka.CharacterCard, fanatic.CharacterCard);


            //set hitpoints to prep for tests
            SetHitPoints(responder1, 1);
            SetHitPoints(aeonLocus, 1);
            SetHitPoints(aeonWarrior, 1);

            //Whenever a hero card destroys a villain target, 1 Responder regains 1HP.

            DecisionSelectCards = new Card[] { aeonLocus, aeonWarrior, haka.CharacterCard};

            //bz one
            QuickHPStorage(responder1);
            UsePower(gaze);
            QuickHPCheck(1);

            //bz two
            QuickHPUpdate();
            PlayCard("FinalDive");
            QuickHPCheckZero();
        }

        [Test()]
        public void TestWCPDChariot()
        {
            SetupGameController(new string[] { "OblivAeon", "Ra", "Legacy", "Haka", "Tachyon", "Luminary", "Cauldron.WindmillCity", "MobileDefensePlatform", "InsulaPrimalis", "Cauldron.VaultFive", "Cauldron.Northspar" }, shieldIdentifier: "PrimaryObjective");
            StartGame();

            GoToPlayCardPhase(windmill);
            DestroyNonCharacterVillainCards();

            SetHitPoints(luminary, 15);

            Card chariot = PlayCard("WCPDChariot");
            Card aeonWarrior = GetCard("AeonWarrior");
            Card aeonLocus = GetCard("AeonLocus");

            PlayCard(oblivaeon, aeonWarrior, overridePlayLocation: bzTwo.FindScion().PlayArea);
            PlayCard(oblivaeon, aeonLocus, overridePlayLocation: bzOne.FindScion().PlayArea);

            AssertCardSpecialString(chariot, 0, "Non-environment target with the second lowest HP: Luminary.");

            SwitchBattleZone(haka);
            SwitchBattleZone(luminary);

            //in Bz 1: ra, tachyon, legacy
            //in Bz 2: haka, luminary

            AssertCardsInBattleZone(bzOne, aeonLocus, ra.CharacterCard, tachyon.CharacterCard, legacy.CharacterCard);
            AssertCardsInBattleZone(bzTwo, aeonWarrior, haka.CharacterCard, luminary.CharacterCard);

            AssertCardSpecialString(chariot, 0, "Non-environment target with the second lowest HP: Tachyon.");

            //At the end of the environment turn, this card deals the non-environment target with the second lowest HP {H - 1} projectile damage.
            //second lowest should be tachyon
            DecisionYesNo = false;
            QuickHPStorage(aeonLocus, aeonWarrior, ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard, tachyon.CharacterCard, luminary.CharacterCard);
            GoToEndOfTurn(windmill);
            QuickHPCheck(0, 0, 0, 0, 0, -4, 0);
        }

        [Test()]
        public void TestWCPDSquad()
        {
            SetupGameController(new string[] { "OblivAeon", "Ra", "Legacy", "Haka", "Tachyon", "Luminary", "Cauldron.WindmillCity", "MobileDefensePlatform", "InsulaPrimalis", "Cauldron.VaultFive", "Cauldron.Northspar" }, shieldIdentifier: "PrimaryObjective");
            StartGame();

            GoToPlayCardPhase(windmill);
            DestroyNonCharacterVillainCards();

            SetHitPoints(luminary, 15);

            Card squad = GetCard("WCPDSquad");
            Card aeonWarrior = GetCard("AeonWarrior");
            Card aeonLocus = GetCard("AeonLocus");

            PlayCard(oblivaeon, aeonWarrior, overridePlayLocation: bzTwo.FindScion().PlayArea);
            PlayCard(oblivaeon, aeonLocus, overridePlayLocation: bzOne.FindScion().PlayArea);

            SwitchBattleZone(haka);
            SwitchBattleZone(luminary);

            //in Bz 1: ra, tachyon, legacy
            //in Bz 2: haka, luminary

            AssertCardsInBattleZone(bzOne, aeonLocus, ra.CharacterCard, tachyon.CharacterCard, legacy.CharacterCard);
            AssertCardsInBattleZone(bzTwo, aeonWarrior, haka.CharacterCard, luminary.CharacterCard);

            //When this card enters play, it deals each villain target 1 projectile damage.
            QuickHPStorage(aeonLocus, aeonWarrior, ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard, tachyon.CharacterCard, luminary.CharacterCard);
            PlayCard(squad);
            QuickHPCheck(-1, 0, 0, 0, 0, 0, 0);
        }

        [Test()]
        public void TestStartOfVillainOblivaeonTurn()
        {
            SetupGameController(new string[] { "OblivAeon", "Ra", "Legacy", "Haka", "Tachyon", "Luminary", "RealmOfDiscord", "MobileDefensePlatform", "InsulaPrimalis", "RookCity", "Megalopolis" }, shieldIdentifier: "PrimaryObjective");
            StartGame();

            GoToPlayCardPhase(envOne);

            Card impendingDoom = MoveCard(oblivaeon, "ImpendingDoom", oblivaeon.TurnTaker.Deck);
            Card timeFlies = PlayCard("TimeFlies");
            GoToStartOfTurn(oblivaeon);
            AssertInPlayArea(scionTwo, impendingDoom);
        }
    }
}
