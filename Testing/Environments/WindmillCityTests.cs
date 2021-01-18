using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace CauldronTests
{
    [TestFixture()]
    public class WindmillCityTests : CauldronBaseTest
    {

        #region WindmillCityHelperFunctions

        protected TurnTakerController windmill { get { return FindEnvironment(); } }
        protected bool IsEmergency(Card card)
        {
            return card.DoKeywordsContain("emergency");
        }
        protected bool IsResponder(Card card)
        {
            return card.DoKeywordsContain("responder");
        }

        #endregion

        [Test()]
        public void TestWindmillCityWorks()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.WindmillCity");
            StartGame();
            Assert.AreEqual(5, this.GameController.TurnTakerControllers.Count());

        }

        [Test()]
        [Sequential]
        public void DecklistTest_Emergency_IsEmergency([Values("CitywideCarnage", "BridgeDisaster", "CrackedWaterMain", "CrashedTanker", "RainOfDebris", "InjuredWorker")] string emergency)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.WindmillCity");
            StartGame();

            GoToPlayCardPhase(windmill);

            Card card = PlayCard(emergency);
            AssertIsInPlay(card);
            AssertCardHasKeyword(card, "emergency", false);
        }

        [Test()]
        [Sequential]
        public void DecklistTest_Responder_IsResponder([Values("DetectiveSedrick", "IntrepidReporter", "WCPDSquad")] string responder)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.WindmillCity");
            StartGame();

            GoToPlayCardPhase(windmill);

            Card card = PlayCard(responder);
            AssertIsInPlay(card);
            AssertCardHasKeyword(card, "responder", false);
        }

        [Test()]
        [Sequential]
        public void DecklistTest_Supercriminal_IsSupercriminal([Values("IronWasp", "Gearlock")] string supercriminal)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.WindmillCity");
            StartGame();

            GoToPlayCardPhase(windmill);

            Card card = PlayCard(supercriminal);
            AssertIsInPlay(card);
            AssertCardHasKeyword(card, "supercriminal", false);
        }

        [Test()]
        [Sequential]
        public void DecklistTest_Device_IsDevice([Values("WCPDChariot")] string device)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.WindmillCity");
            StartGame();

            GoToPlayCardPhase(windmill);

            Card card = PlayCard(device);
            AssertIsInPlay(card);
            AssertCardHasKeyword(card, "device", false);
        }
        [Test()]
        [Sequential]
        public void DecklistTest_Drone_IsDrone([Values("WCPDChariot")] string drone)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.WindmillCity");
            StartGame();

            GoToPlayCardPhase(windmill);

            Card card = PlayCard(drone);
            AssertIsInPlay(card);
            AssertCardHasKeyword(card, "drone", false);
        }

        [Test()]
        [Sequential]
        public void DecklistTest_NoKeyword_HasNoKeyword([Values("GrayPharmaceuticalBuilding", "FrameJob", "SaveTheDay")] string keywordLess)
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.WindmillCity");
            StartGame();

            GoToPlayCardPhase(windmill);

            Card card = PlayCard(keywordLess);
            AssertIsInPlay(card);
            Assert.IsFalse(card.Definition.Keywords.Any(), $"{card.Title} has keywords when it shouldn't.");
        }
        [Test()]
        public void TestBridgeDisaster()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.WindmillCity");
            StartGame();
            DestroyNonCharacterVillainCards();
            Card responder1 = PlayCard("DetectiveSedrick");
            Card responder2 = PlayCard("IntrepidReporter");
            SetHitPoints(responder1, 4);

            GoToPlayCardPhase(windmill);
            //When this card enters play, it deals the Responder with the lowest HP 2 cold damage.
            QuickHPStorage(baron.CharacterCard, ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard, responder1, responder2);
            Card bridge = PlayCard("BridgeDisaster");
            QuickHPCheck(0, 0, 0, 0, -2, 0);

            //At the end of the environment turn, this card deals the {H - 1} hero targets with the highest HP 2 cold damage each.
            QuickHPUpdate();
            GoToEndOfTurn(windmill);
            QuickHPCheck(0, 0, -2, -2, 0, 0);

        }

        [Test()]
        public void TestCitywideCarnage()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.WindmillCity");
            StartGame();
            DestroyNonCharacterVillainCards();
            Card responder1 = PlayCard("DetectiveSedrick");
            Card responder2 = PlayCard("IntrepidReporter");
            SetHitPoints(responder1, 4);

            GoToPlayCardPhase(windmill);
            //When this card enters play, it deals each Responder 2 energy damage.
            QuickHPStorage(baron.CharacterCard, ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard, responder1, responder2);
            Card carnage = PlayCard("CitywideCarnage");
            QuickHPCheck(0, 0, 0, 0, -2, -2);

            //At the end of the environment turn, this card deals each hero target 1 toxic damage and each villain target 1 energy damage.
            QuickHPUpdate();
            GoToEndOfTurn(windmill);
            QuickHPCheck(-1, -1, -1, -1, 0, 0);

        }

        [Test()]
        public void TestCrackedWaterMain()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.WindmillCity");
            StartGame();
            DestroyNonCharacterVillainCards();
            Card responder1 = PlayCard("DetectiveSedrick");
            Card responder2 = PlayCard("IntrepidReporter");
            SetHitPoints(responder1, 4);

            GoToPlayCardPhase(windmill);
            //When this card enters play, it deals 1 Responder 2 melee damage.
            DecisionSelectTarget = responder1;
            QuickHPStorage(baron.CharacterCard, ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard, responder1, responder2);
            Card waterMain = PlayCard("CrackedWaterMain");
            QuickHPCheck(0, 0, 0, 0, -2, 0);

            //villains have damage reduced
            QuickHPStorage(ra);
            DealDamage(baron, ra, 2, DamageType.Melee);
            QuickHPCheck(-1);

            //heroes have damage reduced
            QuickHPUpdate();
            DealDamage(haka, ra, 2, DamageType.Melee);
            QuickHPCheck(-1);

        }

        [Test()]
        public void TestCrashedTanker()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.WindmillCity");
            StartGame();
            DestroyNonCharacterVillainCards();
            Card responder1 = PlayCard("DetectiveSedrick");
            Card responder2 = PlayCard("IntrepidReporter");
            SetHitPoints(responder1, 4);

            GoToPlayCardPhase(windmill);
            //When this card enters play, it deals the Responder with the lowest HP 2 fire damage.
            QuickHPStorage(baron.CharacterCard, ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard, responder1, responder2);
            Card tanker = PlayCard("CrashedTanker");
            QuickHPCheck(0, 0, 0, 0, -2, 0);

            //At the end of the environment turn, this card deals the target with the second highest HP {H - 1} fire damage.
            QuickHPUpdate();
            GoToEndOfTurn(windmill);
            QuickHPCheck(0, 0, 0, -2, 0, 0);

        }

        [Test()]
        public void TestDetectiveSedrick()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.WindmillCity");
            StartGame();
            DestroyNonCharacterVillainCards();
            Card emergency = PlayCard("BridgeDisaster");
            //When this card enters play, it deals 1 target 3 projectile damage.
            QuickHPStorage(baron, ra, legacy, haka);
            DecisionSelectCards = new Card[] { haka.CharacterCard, emergency, haka.CharacterCard };
            Card sedrick = PlayCard("DetectiveSedrick");
            QuickHPCheck(0, 0, 0, -3);
            //At the start of the environment turn, the players may destroy 1 Emergency card. If a card is destroyed this way, 1 hero target regains 2HP.
            QuickHPUpdate();
            GoToStartOfTurn(windmill);
            AssertInTrash(emergency);
            QuickHPCheck(0, 0, 0, 2);
        }

        [Test()]
        public void TestDetectiveSedrick_OptionalDestroy()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.WindmillCity");
            StartGame();
            DestroyNonCharacterVillainCards();
            Card emergency = PlayCard("BridgeDisaster");
            //When this card enters play, it deals 1 target 3 projectile damage.
            QuickHPStorage(baron, ra, legacy, haka);
            DecisionSelectCards = new Card[] { haka.CharacterCard, null };
            Card sedrick = PlayCard("DetectiveSedrick");
            QuickHPCheck(0, 0, 0, -3);
            //At the start of the environment turn, the players may destroy 1 Emergency card. If a card is destroyed this way, 1 hero target regains 2HP.
            QuickHPUpdate();
            GoToStartOfTurn(windmill);
            AssertInPlayArea(windmill, emergency);
            QuickHPCheck(0, 0, 0, 0);
        }

        [Test()]
        public void TestInjuredWorker()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.WindmillCity");
            StartGame();
            DestroyNonCharacterVillainCards();
            Card responder1 = PlayCard("DetectiveSedrick");
            Card responder2 = PlayCard("IntrepidReporter");
            SetHitPoints(responder1, 4);

            GoToPlayCardPhase(windmill);
            //When this card enters play, it deals the Responder wth the lowest HP 2 melee damage. 
            QuickHPStorage(baron.CharacterCard, ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard, responder1, responder2);
            Card rain = PlayCard("InjuredWorker");
            QuickHPCheck(0, 0, 0, 0, -2, 0);
            //Then move this card next to the hero with the highest HP.           
            AssertNextToCard(rain, haka.CharacterCard);

            //Increase the first damage dealt to that hero each turn by 1.
            QuickHPStorage(haka);
            DealDamage(baron, haka, 2, DamageType.Melee);
            QuickHPCheck(-3);

            //only first damage
            QuickHPUpdate();
            DealDamage(baron, haka, 2, DamageType.Melee);
            QuickHPCheck(-2);

            //resets next turn
            GoToNextTurn();
            QuickHPUpdate();
            DealDamage(baron, haka, 2, DamageType.Melee);
            QuickHPCheck(-3);
        }

        [Test()]
        public void TestIntrepidReporter()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.WindmillCity");
            StartGame();
            DestroyNonCharacterVillainCards();
            Card emergency = PlayCard("BridgeDisaster");
            //When this card enters play, 2 players may each draw a card.
            DecisionSelectTurnTakers = new TurnTaker[] { legacy.TurnTaker, ra.TurnTaker, legacy.TurnTaker };
            DecisionSelectCards = new Card[] { emergency };
            QuickHandStorage(ra, legacy, haka);
            Card reporter = PlayCard("IntrepidReporter");
            QuickHandCheck(1, 1, 0);

            //At the start of the environment turn, the players may destroy 1 Emergency card. If a card is destroyed this way, 1 player may draw a card
            QuickHandUpdate();
            GoToStartOfTurn(windmill);
            AssertInTrash(emergency);
            QuickHandCheck(0, 1, 0);
        }

        [Test()]
        public void TestIntrepidReporter_OptionalDestroy()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.WindmillCity");
            StartGame();
            DestroyNonCharacterVillainCards();
            Card emergency = PlayCard("BridgeDisaster");
            //When this card enters play, 2 players may each draw a card.
            DecisionSelectTurnTakers = new TurnTaker[] { legacy.TurnTaker, ra.TurnTaker, legacy.TurnTaker };
            DecisionSelectCards = new Card[] { null };
            QuickHandStorage(ra, legacy, haka);
            Card reporter = PlayCard("IntrepidReporter");
            QuickHandCheck(1, 1, 0);

            //At the start of the environment turn, the players may destroy 1 Emergency card. If a card is destroyed this way, 1 player may draw a card
            QuickHandUpdate();
            GoToStartOfTurn(windmill);
            AssertInPlayArea(windmill, emergency);
            QuickHandCheck(0, 0, 0);
        }

        [Test()]
        public void TestRainOfDebris()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.WindmillCity");
            StartGame();
            DestroyNonCharacterVillainCards();
            Card responder1 = PlayCard("DetectiveSedrick");
            Card responder2 = PlayCard("IntrepidReporter");
            SetHitPoints(responder1, 4);

            GoToPlayCardPhase(windmill);
            //When this card enters play, it deals the Responder wth the lowest HP 2 melee damage. 
            QuickHPStorage(baron.CharacterCard, ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard, responder1, responder2);
            Card rain = PlayCard("RainOfDebris");
            QuickHPCheck(0, 0, 0, 0, -2, 0);

            //At the end of the environment turn, each hero may discard a card. This card deals any hero that did not discard a card this way 2 melee damage.
            Card raDiscard = GetRandomCardFromHand(ra);
            Card hakaDiscard = GetRandomCardFromHand(haka);
            DecisionSelectCards = new Card[] { raDiscard, null, hakaDiscard };
            QuickHPUpdate();
            GoToEndOfTurn(windmill);
            QuickHPCheck(0, 0, -2, 0, 0, 0);
            AssertInTrash(raDiscard);
            AssertInTrash(hakaDiscard);
        }

        [Test()]
        public void TestWCPDSquad()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.WindmillCity");
            StartGame();
            DestroyNonCharacterVillainCards();
            Card battalion = PlayCard("BladeBattalion");
            Card emergency = PlayCard("BridgeDisaster");
            GoToEndOfTurn(haka);
            //When this card enters play, it deals each villain target 1 projectile damage.
            QuickHPStorage(baron.CharacterCard, battalion, ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard);
            DecisionAutoDecideIfAble = true;
            DecisionSelectCards = new Card[] { emergency, baron.CharacterCard };
            Card squad = PlayCard("WCPDSquad");
            QuickHPCheck(-1, -1, 0, 0, 0);
            //At the start of the environment turn, the players may destroy 1 Emergency card. If a card is destroyed this way, this card deals 1 target 3 projectile damage.
            QuickHPUpdate();
            GoToStartOfTurn(windmill);
            AssertInTrash(emergency);
            QuickHPCheck(-3, 0, 0, 0,0);
        }

        [Test()]
        public void TestWCPDSquad_OptionalDestroy()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.WindmillCity");
            StartGame();
            DestroyNonCharacterVillainCards();
            Card battalion = PlayCard("BladeBattalion");
            Card emergency = PlayCard("BridgeDisaster");
            GoToEndOfTurn(haka);
            //When this card enters play, it deals each villain target 1 projectile damage.
            QuickHPStorage(baron.CharacterCard, battalion, ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard);
            DecisionAutoDecideIfAble = true;
            DecisionSelectCards = new Card[] { null };
            Card squad = PlayCard("WCPDSquad");
            QuickHPCheck(-1, -1, 0, 0, 0);
            //At the start of the environment turn, the players may destroy 1 Emergency card. If a card is destroyed this way, this card deals 1 target 3 projectile damage.
            QuickHPUpdate();
            GoToStartOfTurn(windmill);
            AssertInPlayArea(windmill, emergency);
            QuickHPCheck(0, 0, 0, 0, 0);
        }

    }
}
