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
        public void TestFrameJob()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.WindmillCity");
            StartGame();
            DestroyNonCharacterVillainCards();
            Card responder = PutOnDeck("DetectiveSedrick");
            Card frameJob = GetCard("FrameJob");
            IEnumerable<Card> nonResponders = FindCardsWhere(c => windmill.TurnTaker.Deck.HasCard(c) && !IsResponder(c) && c != frameJob).Take(4);
            PutOnDeck(windmill, nonResponders);
            //When this card enters play, reveal cards from the top of the environment deck until a Responder is revealed, put it into play, and discard the other revealed cards.
            PlayCard(frameJob);
            AssertInPlayArea(windmill, responder);
            AssertInTrash(nonResponders);
            AssertNumberOfCardsInRevealed(windmill, 0);

            //Redirect all damage dealt by Responders to the hero target with the highest HP.
            QuickHPStorage(baron, ra, legacy, haka);
            DealDamage(responder, baron, 5, DamageType.Projectile);
            QuickHPCheck(0, 0, -5, 0);

            //At the start of the environment turn, destroy this card.
            GoToStartOfTurn(windmill);
            AssertInTrash(frameJob);

            //check redirection is gone
            QuickHPUpdate();
            DealDamage(responder, baron, 5, DamageType.Projectile);
            QuickHPCheck(-5, 0, 0, 0);
        }

        [Test()]
        public void TestGearlock()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.WindmillCity");
            StartGame();
            DestroyNonCharacterVillainCards();

            Card gearlock = PlayCard("Gearlock");

            //Reduce damage dealt to this card by {H - 1}.
            //Whenever this card is dealt damage, it deals the source of that damage 3 lightning damage.
            QuickHPStorage(baron.CharacterCard, ra.CharacterCard, legacy.CharacterCard, haka.CharacterCard, gearlock);
            DealDamage(ra, gearlock, 4, DamageType.Fire);
            QuickHPCheck(0, -3, 0, 0, -2);

            //When this card is reduced to 0 HP, play the top card of each hero deck in turn order.
            Card baronTop = PutOnDeck("MobileDefensePlatform");
            Card raTop = PutOnDeck("FlameBarrier");
            Card legacyTop = PutOnDeck("NextEvolution");
            Card hakaTop = PutOnDeck("Mere");

            DealDamage(baron, gearlock, 20, DamageType.Fire);
            AssertInTrash(gearlock);
            AssertOnTopOfDeck(baronTop);
            AssertIsInPlay(raTop);
            AssertIsInPlay(legacyTop);
            AssertIsInPlay(hakaTop);
        }

        [Test()]
        public void TestGrayPharmaceuticalBuilding_Increase()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Fanatic", "VoidGuardMainstay/VoidGuardRoadWarriorMainstayCharacter", "Cauldron.WindmillCity");
            StartGame();
            DestroyNonCharacterVillainCards();

            //Whenever a hero uses a power that deals damage, increase that damage by 2.
            Card grayBuilding = PlayCard("GrayPharmaceuticalBuilding");

            DecisionSelectTarget = baron.CharacterCard;
            GoToPlayCardPhase(ra);
            //non power damage not changed
            QuickHPStorage(baron);
            DealDamage(ra, baron, 3, DamageType.Fire);
            QuickHPCheck(-3);

            //check with a power
            GoToUsePowerPhase(ra);
            QuickHPUpdate();
            UsePower(ra.CharacterCard);
            QuickHPCheck(-4);

            GoToDrawCardPhase(ra);

            //repeat to ensure not stacking
            QuickHPUpdate();
            UsePower(ra.CharacterCard);
            QuickHPCheck(-4);

            //check on fanatic
            GoToUsePowerPhase(fanatic);
            QuickHPUpdate();
            UsePower(fanatic.CharacterCard);
            QuickHPCheck(-6);

            //try in the same phase
            QuickHPUpdate();
            UsePower(fanatic.CharacterCard);
            QuickHPCheck(-6);

            //try a status effect
            UsePower(voidMainstay);
            DecisionYesNo = true;
            QuickHPUpdate();
            DealDamage(baron, voidMainstay, 1, DamageType.Melee);
            QuickHPCheck(-3);
        }

        [Test()]
        public void TestGrayPharmaceuticalBuilding_Destroy()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Fanatic", "Cauldron.WindmillCity");
            StartGame();
            DestroyNonCharacterVillainCards();

            //After a hero uses a power on a non-character card, destroy that card.
            Card grayBuilding = PlayCard("GrayPharmaceuticalBuilding");

            DecisionSelectTarget = baron.CharacterCard;
            UsePower(ra.CharacterCard);
            AssertInPlayArea(ra, ra.CharacterCard);

            Card tornado = PlayCard("BlazingTornado");
            UsePower(tornado);
            AssertInTrash(tornado);

            //At the start of the environment turn, destroy this card.
            GoToStartOfTurn(windmill);
            AssertInTrash(grayBuilding);

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
        public void TestIronWasp()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.WindmillCity");
            StartGame();
            DestroyNonCharacterVillainCards();
            Card wasp = PlayCard("IronWasp");
            GoToEndOfTurn(haka);
            //At the start of the environment turn, this card deals each hero target X melee damage, where X is the current HP of this card.
            QuickHPStorage(baron, ra, legacy, haka);
            GoToStartOfTurn(windmill);
            QuickHPCheck(0, -8, -8, -8);
        }
        [Test()]
        public void TestIronWaspDynamic()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.WindmillCity");
            StartGame();
            DestroyNonCharacterVillainCards();
            Card wasp = PlayCard("IronWasp");
            PlayCard("FlameBarrier");
            GoToEndOfTurn(haka);
            //At the start of the environment turn, this card deals each hero target X melee damage, where X is the current HP of this card.
            QuickHPStorage(baron, ra, legacy, haka);
            GoToStartOfTurn(windmill);
            QuickHPCheck(0, -8, -6, -6);
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
        public void TestRainOfDebris_MultiCharHero()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "TheSentinels", "Cauldron.WindmillCity");
            StartGame();
            DestroyNonCharacterVillainCards();
            Card responder1 = PlayCard("DetectiveSedrick");
            Card responder2 = PlayCard("IntrepidReporter");
            SetHitPoints(responder1, 4);

            GoToPlayCardPhase(windmill);
            //When this card enters play, it deals the Responder wth the lowest HP 2 melee damage. 
            QuickHPStorage(baron.CharacterCard, ra.CharacterCard, legacy.CharacterCard, mainstay, idealist, writhe, medico, responder1, responder2);
            Card rain = PlayCard("RainOfDebris");
            QuickHPCheck(0, 0, 0, 0, 0, 0, 0, -2, 0);

            //At the end of the environment turn, each hero may discard a card. This card deals any hero that did not discard a card this way 2 melee damage.
            DiscardAllCards(sentinels);
            QuickHandStorage(ra, legacy, sentinels);
            QuickHPUpdate();
            GoToEndOfTurn(windmill);
            QuickHPCheck(0, 0, 0, -2, -2, -2, -2, 0, 0);
            QuickHandCheck(-1, -1, 0);
        }

        [Test()]
        public void TestSaveTheDay()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Fanatic", "Cauldron.WindmillCity");
            StartGame();
            DestroyNonCharacterVillainCards();
            Card battalion = PlayCard("BladeBattalion");
            Card responder = PutOnDeck("DetectiveSedrick");
            //When this card enters play, play the top card of the environment deck.
            Card saveDay = PlayCard("SaveTheDay");
            AssertInPlayArea(windmill, responder);
            SetHitPoints(responder, 3);
            SetHitPoints(battalion, 3);

            //Whenever a hero card destroys a villain target, 1 Responder regains 1HP.
            DecisionSelectCards = new Card[] { battalion, baron.CharacterCard };
            QuickHPStorage(responder);
            PlayCard("FinalDive");
            QuickHPCheck(1);
            


        }

        [Test()]
        public void TestWCPDChariot()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.WindmillCity");
            StartGame();
            DestroyNonCharacterVillainCards();
            GoToPlayCardPhase(windmill);
            Card chariot = PlayCard("WCPDChariot");
            //At the end of the environment turn, this card deals the non-environment target with the second lowest HP {H - 1} projectile damage.
            //If a hero target would be dealt damage this way, that hero may discard 2 cards to redirect that damage to a non-environment target.
            DecisionYesNo = true;
            DecisionSelectTarget = baron.CharacterCard;
            QuickHPStorage(baron, ra, legacy, haka);
            QuickHandStorage(ra, legacy, haka);
            GoToEndOfTurn(windmill);
            QuickHPCheck(-2, 0, 0, 0);
            QuickHandCheck(0, -2, 0);
        }

        [Test()]
        public void TestWCPDChariot_CheckForNoRedirects()
        {
            SetupGameController("BaronBlade", "Ra", "Legacy", "Haka", "Cauldron.WindmillCity");
            StartGame();
            DestroyNonCharacterVillainCards();
            GoToPlayCardPhase(windmill);
            SetHitPoints(baron, 31);
            Card chariot = PlayCard("WCPDChariot");
            //At the end of the environment turn, this card deals the non-environment target with the second lowest HP {H - 1} projectile damage.
            //If a hero target would be dealt damage this way, that hero may discard 2 cards to redirect that damage to a non-environment target.
            DecisionYesNo = true;
            DecisionSelectTarget = baron.CharacterCard;
            QuickHPStorage(baron, ra, legacy, haka);
            QuickHandStorage(ra, legacy, haka);
            GoToEndOfTurn(windmill);
            QuickHPCheck(-2, 0, 0, 0);
            QuickHandCheck(0, 0, 0);

            DealDamage(chariot, ra, 3, DamageType.Fire);
            QuickHandCheckZero();
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
