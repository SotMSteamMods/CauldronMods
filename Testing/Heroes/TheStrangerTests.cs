using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using Cauldron.TheStranger;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CauldronTests
{
    [TestFixture()]
    public class TheStranger : BaseTest
    {
        #region TheStrangerHelperFunctions
        protected HeroTurnTakerController stranger { get { return FindHero("TheStranger"); } }
        private void SetupIncap(TurnTakerController villain)
        {
            SetHitPoints(stranger.CharacterCard, 1);
            DealDamage(villain, stranger, 2, DamageType.Melee);
        }
       

        #endregion

        [Test()]
        public void TestStrangerLoads()
        {
            SetupGameController("BaronBlade", "Cauldron.TheStranger", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(stranger);
            Assert.IsInstanceOf(typeof(TheStrangerCharacterCardController), stranger.CharacterCardController);

            Assert.AreEqual(26, stranger.CharacterCard.HitPoints);
        }

        [Test()]
        public void TestStrangerInnatePower()
        {
            SetupGameController("BaronBlade", "Cauldron.TheStranger", "Megalopolis");
            StartGame();

            //put a rune in hand
            PutInHand("MarkOfBinding");
            Card binding = GetCardFromHand("MarkOfBinding");

            GoToUsePowerPhase(stranger);
            AssertInHand(binding);
            DecisionSelectCard = binding;
            //Play a rune
            UsePower(stranger.CharacterCard);
            AssertIsInPlay(binding);

        }

        [Test()]
        public void TestStrangerIncap1()
        {
            SetupGameController("BaronBlade", "Cauldron.TheStranger", "Haka", "Megalopolis");
            StartGame();

            SetupIncap(baron);
            AssertIncapacitated(stranger);

            //One player may draw a card now.
            GoToUseIncapacitatedAbilityPhase(stranger);
            DecisionSelectTarget = haka.CharacterCard;
            QuickHandStorage(haka);
            UseIncapacitatedAbility(stranger, 0);
            //should have one more card in haka's hand
            QuickHandCheck(1);

        }

        [Test()]
        public void TestStrangerIncap2OnDeck()
        {
            SetupGameController("BaronBlade", "Cauldron.TheStranger", "Haka", "Megalopolis");
            StartGame();

            Card mere = GetCard("Mere");
            PutOnDeck(haka, mere);

            SetupIncap(baron);
            AssertIncapacitated(stranger);

            //Reveal the top card of a deck, then replace it or discard it
            GoToUseIncapacitatedAbilityPhase(stranger);
            AssertOnTopOfDeck(mere);
            //choose haka's deck
            DecisionSelectLocation = new LocationChoice(haka.TurnTaker.Deck);
            //choose to put back on the deck
            DecisionMoveCardDestination = new MoveCardDestination(haka.TurnTaker.Deck, false);

            UseIncapacitatedAbility(stranger, 1);

            AssertOnTopOfDeck(mere);

        }
        [Test()]
        public void TestStrangerIncap2ToTrash()
        {
            SetupGameController("BaronBlade", "Cauldron.TheStranger", "Haka", "Megalopolis");
            StartGame();

            Card mere = GetCard("Mere");
            PutOnDeck(haka, mere);

            SetupIncap(baron);
            AssertIncapacitated(stranger);

            //Reveal the top card of a deck, then replace it or discard it
            GoToUseIncapacitatedAbilityPhase(stranger);
            AssertOnTopOfDeck(mere);
            //choose haka's deck
            DecisionSelectLocation = new LocationChoice(haka.TurnTaker.Deck);
            //choose to discard
            DecisionMoveCardDestination = new MoveCardDestination(haka.TurnTaker.Trash, false);
            UseIncapacitatedAbility(stranger, 1);

            AssertOnTopOfTrash(haka, mere);

        }

        [Test()]
        public void TestStrangerIncap3_Choose2()
        {
            SetupGameController("BaronBlade", "Cauldron.TheStranger", "Haka", "Ra", "Megalopolis");
            StartGame();
            SetupIncap(baron);
            AssertIncapacitated(stranger);

            PutInHand("FlameBarrier");
            Card barrier = GetCardFromHand("FlameBarrier");
            PutInHand("PunishTheWeak");
            Card punish = GetCardFromHand("PunishTheWeak");
            //Up to 2 ongoing hero cards may be played now.
            GoToUseIncapacitatedAbilityPhase(stranger);
            DecisionSelectCards = new Card[] { barrier, punish };
            DecisionYesNo = true;
            UseIncapacitatedAbility(stranger, 2);
            AssertIsInPlay(barrier, punish);

        }

        [Test()]
        public void TestStrangerIncap3_Choose0()
        {
            SetupGameController("BaronBlade", "Cauldron.TheStranger", "Haka", "Ra", "Megalopolis");
            StartGame();
            SetupIncap(baron);
            AssertIncapacitated(stranger);

            PutInHand("FlameBarrier");
            Card barrier = GetCardFromHand("FlameBarrier");
            PutInHand("PunishTheWeak");
            Card punish = GetCardFromHand("PunishTheWeak");
            //Up to 2 ongoing hero cards may be played now.
            GoToUseIncapacitatedAbilityPhase(stranger);
            DecisionYesNo = false;
            UseIncapacitatedAbility(stranger, 2);
            AssertNotInPlay(barrier, punish);

        }

        [Test()]
        public void TestGlyphPrevention_FirstDamage()
        {
            SetupGameController("BaronBlade", "Cauldron.TheStranger", "Haka", "Ra", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(stranger);
            PutIntoPlay("GlyphOfPerception");
            
            //Once during your turn when TheStranger would deal himself damage, prevent that damage.
            QuickHPStorage(stranger);
            DecisionYesNo = true;
            DealDamage(stranger, stranger, 5, DamageType.Sonic);
            //since damage was prevented, no change in health
            QuickHPCheckZero();

            QuickHPStorage(stranger);
            DealDamage(stranger, stranger, 5, DamageType.Sonic);
            //since already used this turn, damage should be dealt
            QuickHPCheck(-5);

            GoToStartOfTurn(stranger);
            QuickHPStorage(stranger);
            DecisionYesNo = true;
            DealDamage(stranger, stranger, 5, DamageType.Sonic);
            //since new round, damage was prevented, no change in health
            QuickHPCheckZero();
        }
        [Test()]
        public void TestGlyphPrevention_SecondDamage()
        {
            SetupGameController("BaronBlade", "Cauldron.TheStranger", "Haka", "Ra", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(stranger);
            PutIntoPlay("GlyphOfDecay");

            //Once during your turn when TheStranger would deal himself damage, prevent that damage.
            QuickHPStorage(stranger);
            DecisionsYesNo = new bool[] { false, true, true };
            DealDamage(stranger, stranger, 5, DamageType.Sonic);
            //since said no, -5
            QuickHPCheck(-5);

            QuickHPStorage(stranger);
            DealDamage(stranger, stranger, 5, DamageType.Sonic);
            //since said yes, damage should be prevented
            QuickHPCheckZero();

            GoToStartOfTurn(stranger);
            QuickHPStorage(stranger);
            DecisionYesNo = true;
            DealDamage(stranger, stranger, 5, DamageType.Sonic);
            //since new round, damage was prevented, no change in health
            QuickHPCheckZero();
        }

        [Test()]
        public void TestRuneDestroySuccessful()
        {
            SetupGameController("BaronBlade", "Haka", "Cauldron.TheStranger", "Ra", "Megalopolis");
            StartGame();

            GoToEndOfTurn(haka);
            PutIntoPlay("MarkOfTheFaded");
            Card rune = GetCardInPlay("MarkOfTheFaded");
            //At the start of your turn you may destroy this card. If you do not, TheStranger deals himself 1 irreducible toxic damage.
            AssertIsInPlay(rune);
            //yes we want to destroy
            DecisionYesNo = true;
            QuickHPStorage(stranger);
            GoToStartOfTurn(stranger);
            //should have been destroyed and no damage dealt
            AssertInTrash(rune);
            QuickHPCheckZero();
        
        }

        [Test()]
        public void TestRuneDestroyFailed()
        {
            SetupGameController("BaronBlade", "Haka", "Cauldron.TheStranger", "Ra", "Megalopolis");
            StartGame();

            GoToEndOfTurn(haka);
            PutIntoPlay("MarkOfTheFaded");
            Card rune = GetCardInPlay("MarkOfTheFaded");
            //At the start of your turn you may destroy this card. If you do not, TheStranger deals himself 1 irreducible toxic damage.
            AssertIsInPlay(rune);
            //no we don't want to destroy
            DecisionYesNo = false;
            QuickHPStorage(stranger);
            GoToStartOfTurn(stranger);
            //should have not been destroyed and damage dealt
            AssertIsInPlay(rune);
            QuickHPCheck(-1);

        }

        [Test()]
        public void TestCorruptionDraw4()
        {
            SetupGameController("BaronBlade", "Haka", "Cauldron.TheStranger", "Ra", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(stranger);

            //Draw up to 4 cards.
            QuickHandStorage(stranger);

            //draw 4
            DecisionsYesNo = new bool[] { true, true, true, true };
            PlayCard("Corruption");
            QuickHandCheck(4);
        }

        [Test()]
        public void TestCorruptionDraw3()
        {
            SetupGameController("BaronBlade", "Haka", "Cauldron.TheStranger", "Ra", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(stranger);

            //Draw up to 4 cards.
            QuickHandStorage(stranger);
            //draw 3
            DecisionsYesNo = new bool[] { true, true, true, false };
            PlayCard("Corruption");
            QuickHandCheck(3);
        }
        [Test()]
        public void TestCorruptionDraw2()
        {
            SetupGameController("BaronBlade", "Haka", "Cauldron.TheStranger", "Ra", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(stranger);

            //Draw up to 4 cards.
            QuickHandStorage(stranger);
            //draw 2
            DecisionsYesNo = new bool[] { true, true, false };
            PlayCard("Corruption");
            QuickHandCheck(2);
        }

        [Test()]
        public void TestCorruptionDraw1()
        {
            SetupGameController("BaronBlade", "Haka", "Cauldron.TheStranger", "Ra", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(stranger);

            //Draw up to 4 cards.
            QuickHandStorage(stranger);
            //draw 1
            DecisionsYesNo = new bool[] { true, false };
            PlayCard("Corruption");
            QuickHandCheck(1);
        }


        [Test()]
        public void TestCorruptionNoDraw()
        {
            SetupGameController("BaronBlade", "Haka", "Cauldron.TheStranger", "Ra", "Megalopolis");
            StartGame();

            //Draw up to 4 cards.
            QuickHandStorage(stranger);
            DecisionsYesNo = new bool[] { false };
            PlayCard("Corruption");
            QuickHandCheck(0);
        }

        [Test()]
        public void TestCorruptionDamage_ZeroDrawn()
        {
            SetupGameController("BaronBlade", "Haka", "Cauldron.TheStranger", "Ra", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(stranger);

            //Draw up to 4 cards.
            QuickHPStorage(stranger);
            //draw 4
            DecisionsYesNo = new bool[] { false };
            PlayCard("Corruption");

            //For each card drawn this way, TheStranger deals himself 1 toxic damage.
            QuickHPCheck(0);

        }

        [Test()]
        public void TestCorruptionDamage_FourDrawn()
        {
            SetupGameController("BaronBlade", "Haka", "Cauldron.TheStranger", "Ra", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(stranger);

            //Draw up to 4 cards.
            QuickHPStorage(stranger);
            //draw 4
            DecisionsYesNo = new bool[] { true, true, true, true };
            PlayCard("Corruption");

            //For each card drawn this way, TheStranger deals himself 1 toxic damage.
            QuickHPCheck(-4);
            
        }

        [Test()]
        public void TestCorruptionDamage_ThreeDrawn()
        {
            SetupGameController("BaronBlade", "Haka", "Cauldron.TheStranger", "Ra", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(stranger);

            //Draw up to 4 cards.
            QuickHPStorage(stranger);
            //draw 3
            DecisionsYesNo = new bool[] { true, true, true, false };
            PlayCard("Corruption");

            //For each card drawn this way, TheStranger deals himself 1 toxic damage.
            QuickHPCheck(-3);

        }

        [Test()]
        public void TestCorruptionDamage_TwoDrawn()
        {
            SetupGameController("BaronBlade", "Haka", "Cauldron.TheStranger", "Ra", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(stranger);

            //Draw up to 4 cards.
            QuickHPStorage(stranger);
            //draw 2
            DecisionsYesNo = new bool[] { true, true, false };
            PlayCard("Corruption");

            //For each card drawn this way, TheStranger deals himself 1 toxic damage.
            QuickHPCheck(-2);

        }

        [Test()]
        public void TestCorruptionDamage_OneDrawn()
        {
            SetupGameController("BaronBlade", "Haka", "Cauldron.TheStranger", "Ra", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(stranger);

            //Draw up to 4 cards.
            QuickHPStorage(stranger);
            //draw 1
            DecisionsYesNo = new bool[] { true, false };
            PlayCard("Corruption");

            //For each card drawn this way, TheStranger deals himself 1 toxic damage.
            QuickHPCheck(-1);

        }


    }
}
