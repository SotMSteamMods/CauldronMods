using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

using Cauldron.Drift;

namespace CauldronTests
{
    [TestFixture()]
    public class DriftDualVariantTests : CauldronBaseTest
    {
        protected const string AttenuationField = "AttenuationField";
        protected const string BorrowedTime = "BorrowedTime";
        protected const string DanceOfTheDragons = "DanceOfTheDragons";
        protected const string DestroyersAdagio = "DestroyersAdagio";
        protected const string DriftStep = "DriftStep";
        protected const string FutureFocus = "FutureFocus";
        protected const string ImposedSynchronization = "ImposedSynchronization";
        protected const string KnightsHeritage = "KnightsHeritage";
        protected const string MakeEverySecondCount = "MakeEverySecondCount";
        protected const string OutOfSync = "OutOfSync";
        protected const string PastFocus = "PastFocus";
        protected const string ResourcefulDaydreamer = "ResourcefulDaydreamer";
        protected const string Sabershard = "Sabershard";
        protected const string ThrowingShard = "ThrowingShard";
        protected const string TransitionShock = "TransitionShock";

        protected const string ShiftTrack = "ShiftTrack";
        protected const string PastDriftCharacter = "PastDriftCharacter";
        protected const string FutureDriftCharacter = "FutureDriftCharacter";

        [Test()]
        [Order(0)]
        public void TestDriftLoad_Dual()
        {
            SetupGameController("BaronBlade", "Cauldron.Drift/DualDriftCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");

            Assert.AreEqual(6, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(drift);
            Assert.IsInstanceOf(typeof(DualDriftCharacterCardController), drift.CharacterCardController);

            foreach (var card in drift.HeroTurnTaker.GetAllCards())
            {
                var cc = GetCardController(card);
                Assert.IsTrue(cc.GetType() != typeof(CardController), $"{card.Identifier} is does not have a CardController");
            }

            //Assert.AreEqual(26, drift.CharacterCard.HitPoints);
            AssertMaximumHitPoints(GetCard("PastDriftCharacter"), 15);
            AssertMaximumHitPoints(GetCard("FutureDriftCharacter"), 16);
        }

        [Test]
        public void TestDriftCharacter_StartWith([Values("PastDriftCharacter", "FutureDriftCharacter")] string characterId)
        {
            SetupGameController("BaronBlade", "Cauldron.Drift/DualDriftCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            Card track = FindCardsWhere((Card c) => c.Identifier == $"Dual{ShiftTrack}1", false).FirstOrDefault();
            Card character = FindCardsWhere((Card c) => c.Identifier == characterId).FirstOrDefault();
            Card other = FindCardsWhere((Card c) => c.IsHeroCharacterCard && c.Owner == drift.TurnTaker && c.Identifier != characterId && !c.Identifier.Contains("Red") && !c.Identifier.Contains("Blue")).FirstOrDefault();
            DecisionSelectCards = new Card[] { track, character };
            StartGame(false);

            AssertIsInPlay(character.Identifier);
            AssertOffToTheSide(other.Identifier);
        }

        [Test()]
        public void TestDriftCharacter_SwitchActiveHero()
        {
            SetupGameController(new string[] { "BaronBlade", "Cauldron.Drift/DualDriftCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis" }, randomSeed: new int?(-2054413546));
            Card futureDrift = GetCard(FutureDriftCharacter);
            Card shiftTrack = GetCard("DualShiftTrack2");
            DecisionSelectCards = new Card[] { shiftTrack, futureDrift, baron.CharacterCard, null };
            StartGame(resetDecisions: false);

            //Start with Future
            AssertIsInPlay(FutureDriftCharacter);
            GoToShiftPosition(2);

            int position1 = 2;

            PrintSeparator("Preparing to Shift Right");

            //Shift Right 1
            DecisionSelectWords = new string[] { "Swap the active Drift, then Shift {ShiftR}", "Swap the active Drift, then Shift {ShiftR}" };
            DecisionSelectFunction = 1;
            PlayCard(DriftStep);
            AssertTrackPosition(3);

            AssertIsInPlay(PastDriftCharacter);

            PrintSeparator("Switching Back");
            //Switch back
            GoToNextTurn();

            Card pastDrift = GetCard(PastDriftCharacter);
            DecisionYesNo = true;
            DealDamage(baron, pastDrift, 2, DamageType.Radiant);

            PrintSeparator("Switch back successful");
            //Assert in secondary position
            AssertIsInPlay(FutureDriftCharacter);
            AssertTrackPosition(position1);
        }


        [Test()]
        public void TestDriftCharacter_SwitchActiveHeroOffOfDamage()
        {
            SetupGameController(new string[] { "BaronBlade", "Cauldron.Drift/DualDriftCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis" }, randomSeed: new int?(-2054413546));
            Card futureDrift = GetCard(FutureDriftCharacter);
            Card shiftTrack = GetCard("DualShiftTrack2");
            DecisionSelectCards = new Card[] { shiftTrack, futureDrift };
            StartGame(resetDecisions: false);

            //Start with Future
            AssertIsInPlay(FutureDriftCharacter);
            GoToShiftPosition(2);

            DecisionsYesNo = new bool[] { true, true };

            DealDamage(baron, futureDrift, 3, DamageType.Melee);

            PrintSpecialStringsForCard(GetShiftTrack());
            
        }

        [Test()]
        public void TestDriftCharacter_SwitchActiveHeroOffOfCardPlay()
        {
            SetupGameController(new string[] { "BaronBlade", "Cauldron.Drift/DualDriftCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis" }, randomSeed: new int?(-2054413546));
            Card futureDrift = GetCard(FutureDriftCharacter);
            Card shiftTrack = GetCard("DualShiftTrack4");
            Card resourcefulDreamer = PutInTrash("ResourcefulDaydreamer");
            Card cardToDiscard = PutInHand("DanceOfTheDragons");
            Card cardToPlay = PutInHand("Sabershard");
            Card cardToNotPlay = PutInHand("TransitionShock");

            DecisionSelectCards = new Card[] { shiftTrack, futureDrift, cardToDiscard, cardToPlay, cardToNotPlay };
            StartGame(resetDecisions: false);

            //Start with Future in spot 4
            AssertIsInPlay(FutureDriftCharacter);
            AssertTrackPosition(4);
            // Switch during the villain turn to Past in spot 

            DecisionsYesNo = new bool[] { true, true };

            DealDamage(baron, futureDrift, 3, DamageType.Melee);
            AssertIsInPlay(PastDriftCharacter);
            AssertTrackPosition(4);
            GoToShiftPosition(2);

            GoToNextTurn();

            //Switch on play of resourceful dreamer, resulting in the future effect only
            QuickHandStorage(drift);
            PlayCard(resourcefulDreamer);
            //draw 2, discard 1, power use plays 1, no second play
            QuickHandCheck(0);

            AssertInTrash(cardToDiscard);
            AssertInPlayArea(drift, cardToPlay);
            AssertInHand(cardToNotPlay);
        }

        [Test()]
        public void TestDriftCharacter_UIPrompts()
        {
            SetupGameController(new string[] { "BaronBlade", "Cauldron.Drift/DualDriftCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis" }, randomSeed: new int?(-2054413546));
            Card pastDrift = GetCard(PastDriftCharacter);
            Card shiftTrack = GetCard("DualShiftTrack2");
            DecisionSelectCards = new Card[] { shiftTrack, pastDrift };
            StartGame(resetDecisions: false);

            //Start with Past
            AssertIsInPlay(PastDriftCharacter);

            DecisionsYesNo = new bool[] {true, true};
            DecisionSelectWords = new string[] {"Swap the active Drift, then Shift {ShiftR}", "Swap the active Drift, then Shift {ShiftR}" };

            Card sabershard = PlayCard("Sabershard");
            UsePower(sabershard);

            Card futureDrift = GetCard(FutureDriftCharacter);

            DealDamage(baron, futureDrift, 3, DamageType.Melee);

            GoToShiftPosition(4);

            GoToNextTurn();
            DealDamage(baron, futureDrift, 3, DamageType.Melee);

            AssertTrackPosition(2);

            GoToNextTurn();
            UsePower(sabershard);

            AssertTrackPosition(4);
        }


        [Test()]
        public void TestDriftCharacter_Dual_Past_InnatePower()
        {
            SetupGameController("BaronBlade", "Cauldron.Drift/DualDriftCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            Card shiftTrack4 = GetCard("DualShiftTrack4");
            Card pastDrift = GetCard(PastDriftCharacter);
            Card[] top2 = GetTopCardsOfDeck(drift, 2).ToArray();


            DecisionSelectCards = new Card[] { shiftTrack4, pastDrift, top2[0] };

            StartGame(resetDecisions: false);

   
            int shiftPosition = CurrentShiftPosition();
            DecisionMoveCardDestinations = new MoveCardDestination[]
            {
                new MoveCardDestination(drift.TurnTaker.Trash),
                new MoveCardDestination(drift.TurnTaker.Deck)
            };

            //Reveal the top 2 cards of 1 hero deck. Replace or discard each of them in any order. Shift {DriftLL}.

            UsePower(drift);
            AssertTrackPosition(shiftPosition - 2);
            AssertOnTopOfDeck(top2[1]);
            AssertInTrash(top2[0]);
        }

        [Test()]
        public void TestDriftCharacter_Dual_Future_InnatePower()
        {
            SetupGameController("BaronBlade", "Cauldron.Drift/DualDriftCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            Card futureDrift = GetCard(FutureDriftCharacter);
            Card shiftTrack = GetCard("DualShiftTrack2");
            Card fFocus = GetCard(FutureFocus);

            DecisionSelectCards = new Card[] { shiftTrack, futureDrift, fFocus };
            StartGame(resetDecisions: false);


            PutInHand(fFocus);
            //Play an ongoing card. At the end of your next turn, return it from play to your hand. Shift {DriftRR}.
            int shiftPosition = CurrentShiftPosition();
            UsePower(drift);
            AssertIsInPlay(fFocus);
            AssertTrackPosition(shiftPosition + 2);

            GoToEndOfTurn(drift);
            AssertInHand(fFocus);
        }

        [Test()]
        public void TestDriftCharacter_Future_Incap0()
        {
            SetupGameController("Apostate", "Cauldron.Drift/DualDriftCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            Card futureDrift = GetCard(FutureDriftCharacter);
            Card shiftTrack = GetCard("DualShiftTrack2");
            DecisionSelectCards = new Card[] { shiftTrack, futureDrift };
            StartGame(resetDecisions: false);

            DecisionYesNo = true;
            DestroyCard(drift);
            //One player may draw a card now.

            DecisionSelectTurnTaker = bunker.TurnTaker;
            QuickHandStorage(bunker);
            UseIncapacitatedAbility(drift, 0);
            QuickHandCheck(1);
        }

        [Test()]
        public void TestDriftCharacter_Future_Incap1()
        {
            SetupGameController("Apostate", "Cauldron.Drift/DualDriftCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            Card futureDrift = GetCard(FutureDriftCharacter);
            Card shiftTrack = GetCard("DualShiftTrack2");
            DecisionSelectCards = new Card[] { shiftTrack, futureDrift, apostate.CharacterCard };
            StartGame(resetDecisions: false);

            DestroyCard(drift);
            //Reveal the top card of a hero deck and replace it. If that card has a power on it. Play it and that hero uses that power.

            Card moko = PutOnDeck("TaMoko");
            Card battle = PutOnDeck("HakaOfBattle");
            Card mere = PutOnDeck("Mere");

            //Mere
            QuickHandStorage(haka);
            QuickHPStorage(apostate);
            UseIncapacitatedAbility(drift, 1);
            QuickHandCheck(1);
            QuickHPCheck(-2);
            AssertIsInPlay(mere);

            //TaMoko
            QuickHandStorage(haka);
            QuickHPStorage(apostate);
            UseIncapacitatedAbility(drift, 1);
            QuickHandCheck(0);
            QuickHPCheck(0);
            AssertOnTopOfDeck(moko);
        }

        [Test()]
        public void TestDriftCharacter_Future_Incap2()
        {
            SetupGameController("Apostate", "Cauldron.Drift/DualDriftCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            Card futureDrift = GetCard(FutureDriftCharacter);
            Card shiftTrack = GetCard("DualShiftTrack2");
            DecisionSelectCards = new Card[] { shiftTrack, futureDrift, apostate.CharacterCard };
            StartGame(resetDecisions: false);

            DestroyCard(drift);
            SetHitPoints(apostate, 17);

            //One target regains 2 HP.

            QuickHPStorage(apostate);
            UseIncapacitatedAbility(drift, 2);
            QuickHPCheck(2);
        }

        [Test()]
        public void TestDriftCharacter_Past_Incap0()
        {
            SetupGameController("Apostate", "Cauldron.Drift/DualDriftCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            Card pastDrift = GetCard(PastDriftCharacter);
            Card shiftTrack = GetCard("DualShiftTrack2");
            DecisionSelectCards = new Card[] { shiftTrack, pastDrift };
            StartGame(resetDecisions: false);

            DecisionYesNo = true;
            GoToEndOfTurn(apostate);
            AssertIsInPlay(PastDriftCharacter);
            DestroyCard(drift);
            //One player may draw a card now.

            DecisionSelectTurnTaker = bunker.TurnTaker;
            QuickHandStorage(bunker);
            UseIncapacitatedAbility(drift, 0);
            QuickHandCheck(1);
        }

        [Test()]
        public void TestDriftCharacter_Past_Incap1()
        {
            SetupGameController("Apostate", "Cauldron.Drift/DualDriftCharacter", "Haka", "Tempest", "TheScholar", "Megalopolis");
            Card pastDrift = GetCard(PastDriftCharacter);
            Card shiftTrack = GetCard("DualShiftTrack2");
            DecisionSelectCards = new Card[] { shiftTrack, pastDrift, apostate.CharacterCard };
            StartGame(resetDecisions: false);

            DecisionYesNo = true;
            GoToEndOfTurn(apostate);
            AssertIsInPlay(PastDriftCharacter);
            DestroyCard(drift);

            //Select a card in a hero trash with a power on it. That hero uses that power, then shuffles that card into their deck.
            Card mere = PutInTrash("Mere");

            QuickHandStorage(haka);
            QuickHPStorage(apostate);
            QuickShuffleStorage(haka.TurnTaker.Deck);
            UseIncapacitatedAbility(drift, 1);
            QuickHandCheck(1);
            QuickHPCheck(-2);
            AssertInDeck(mere);
            QuickShuffleCheck(1);

            Card hurricane = PutInTrash("LocalizedHurricane");
            DecisionPowerIndex = 1;

            //Make sure we can use other indexes
            QuickHPStorage(apostate);
            QuickShuffleStorage(tempest.TurnTaker.Deck);
            UseIncapacitatedAbility(drift, 1);
            QuickHPCheck(0);
            QuickShuffleCheck(1);

            AssertInDeck(hurricane);
        }

        [Test()]
        public void TestDriftCharacter_Past_Incap2()
        {
            SetupGameController("Apostate", "Cauldron.Drift/DualDriftCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            Card pastDrift = GetCard(PastDriftCharacter);
            Card shiftTrack = GetCard("DualShiftTrack2");
            DecisionSelectCards = new Card[] { shiftTrack, pastDrift };
            StartGame(resetDecisions: false);

            DecisionYesNo = true;
            GoToEndOfTurn(apostate);
            AssertIsInPlay(PastDriftCharacter);
            DestroyCard(drift);
            Card popo = PlayCard("PoliceBackup");

            //Destroy an environment card.
            UseIncapacitatedAbility(drift, 2);
            AssertInTrash(popo);
        }

        [Test, Sequential]
        public void TestShiftTrackSetup([Values(1, 2, 3, 4)] int decision)
        {
            SetupGameController("BaronBlade", "Cauldron.Drift/DualDriftCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            Card track = FindCardsWhere((Card c) => c.Identifier.Contains($"Dual{ShiftTrack}{decision}"), false).FirstOrDefault();
            Card character = FindCardsWhere((Card c) => c.Identifier == PastDriftCharacter).FirstOrDefault();

            DecisionSelectCards = new Card[] { track, character };
            StartGame(false);

            Assert.AreEqual(decision, CurrentShiftPosition());
            AssertIsInPlay(track);
        }

        [Test()]
        public void TestDualDriftAndProgeny()
        {
            SetupGameController("Progeny", "Cauldron.Drift/DualDriftCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            Card futureDrift = GetCard(FutureDriftCharacter);
            Card shiftTrack = GetCard("DualShiftTrack2");
            DecisionAutoDecideIfAble = true;
            DecisionSelectCards = new Card[] {shiftTrack, futureDrift, null };

            StartGame(resetDecisions: false);


            AssertInPlayArea(drift,futureDrift);

            SetHitPoints(futureDrift, 8);

            GoToStartOfTurn(progeny);

            DealDamage(progeny, futureDrift, 8, DamageType.Radiant);
            AssertIncapacitated(drift);

            GoToStartOfTurn(progeny);

            AssertNotFlipped(progeny);

        }
    }
}
