using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using Cauldron.TheStranger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Handelabra.Sentinels.Engine.Controller.ChronoRanger;

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
            DecisionSelectCards = new Card[] { binding, baron.CharacterCard };
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

        [Test()]
        public void TestFlickeringWeb_Play3()
        {
            SetupGameController("BaronBlade", "Haka", "Cauldron.TheStranger", "Ra", "Megalopolis");
            StartGame();

            Card boneLeech = GetCard("MarkOfTheBoneLeech");
            Card faded = GetCard("MarkOfTheFaded");
            Card twistedShadow = GetCard("MarkOfTheTwistedShadow");
            PutInHand(boneLeech);
            PutInHand(faded);
            PutInHand(twistedShadow);

            GoToPlayCardPhase(stranger);
            //You may play up to 3 Runes now.
            //play 3 cards
            DecisionsYesNo = new bool[] { true, true, true };
            //choose the 3 cards to play
            DecisionSelectCards = new Card[] { boneLeech, haka.CharacterCard, faded, haka.CharacterCard, twistedShadow, haka.CharacterCard };
            PlayCard("FlickeringWeb");
            AssertIsInPlay(boneLeech, faded, twistedShadow);

        }


        [Test()]
        public void TestFlickeringWeb_Play0()
        {
            SetupGameController("BaronBlade", "Haka", "Cauldron.TheStranger", "Ra", "Megalopolis");
            StartGame();

            Card boneLeech = GetCard("MarkOfTheBoneLeech");
            Card faded = GetCard("MarkOfTheFaded");
            Card twistedShadow = GetCard("MarkOfTheTwistedShadow");
            PutInHand(boneLeech);
            PutInHand(faded);
            PutInHand(twistedShadow);

            GoToPlayCardPhase(stranger);
            //You may play up to 3 Runes now.
            //play 0 cards
            DecisionsYesNo = new bool[] { false };

            PlayCard("FlickeringWeb");
            AssertInHand(boneLeech, faded, twistedShadow);

        }

        [Test()]
        public void TestGlyphOfCombustionPrevention_FirstDamage()
        {
            SetupGameController("BaronBlade", "Cauldron.TheStranger", "Haka", "Ra", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(stranger);
            PutIntoPlay("GlyphOfCombustion");

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
        public void TestGlyphOfCombustionPrevention_SecondDamage()
        {
            SetupGameController("BaronBlade", "Cauldron.TheStranger", "Haka", "Ra", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(stranger);
            PutIntoPlay("GlyphOfCombustion");

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
        public void TestGlyphOfCombustion_DestroyGlyph()
        {
            SetupGameController("BaronBlade", "Cauldron.TheStranger", "Haka", "Ra", "Megalopolis");
            StartGame();

            //put a glyph in play
            Card glyph = GetCard("GlyphOfDecay");
            PlayCard(glyph);

            GoToPlayCardPhase(stranger);
            PutIntoPlay("GlyphOfCombustion");
            //Whenever a Rune or Glyph is destroyed, TheStranger may deal 1 target 1 fire damage

            QuickHPStorage(haka);
            DecisionSelectTarget = haka.CharacterCard;
            //say yes to dealing damage
            DecisionYesNo = true;
            DestroyCard(glyph, baron.CharacterCard);
            //since a glyph was destroyed, 1 damage should have been dealt to Haka;
            QuickHPCheck(-1);
        }

        [Test()]
        public void TestGlyphOfCombustion_DestroyRune()
        {
            SetupGameController("BaronBlade", "Cauldron.TheStranger", "Haka", "Ra", "Megalopolis");
            StartGame();

            //put a rune in play
            Card rune = GetCard("MarkOfBinding");
            PlayCard(rune);

            GoToPlayCardPhase(stranger);
            PutIntoPlay("GlyphOfCombustion");
            //Whenever a Rune or Glyph is destroyed, TheStranger may deal 1 target 1 fire damage

            QuickHPStorage(haka);
            DecisionSelectTarget = haka.CharacterCard;
            //say yes to dealing damage
            DecisionYesNo = true;
            DestroyCard(rune, baron.CharacterCard);
            //since a rune was destroyed, 1 damage should have been dealt to Haka;
            QuickHPCheck(-1);
        }

        [Test()]
        public void TestGlyphOfDecayPrevention_FirstDamage()
        {
            SetupGameController("BaronBlade", "Cauldron.TheStranger", "Haka", "Ra", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(stranger);
            PutIntoPlay("GlyphOfDecay");

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
        public void TestGlyphOfDecayPrevention_SecondDamage()
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
        public void TestGlyphOfDecayPower_Play()
        {
            SetupGameController("BaronBlade", "Cauldron.TheStranger", "Haka", "Ra", "Megalopolis");
            StartGame();

            //put a rune in hand
            Card rune = GetCard("MarkOfBinding");
            PutInHand(rune);

            GoToPlayCardPhase(stranger);
            PutIntoPlay("GlyphOfDecay");
            Card decay = GetCardInPlay("GlyphOfDecay");
            //Power: You may play a Rune. 
            GoToUsePowerPhase(stranger);
            DecisionSelectCards = new Card[] { rune, baron.CharacterCard };
            UsePower(decay);
            AssertIsInPlay(rune);

        }

        [Test()]
        public void TestGlyphOfDecayPower_Damage()
        {
            SetupGameController("BaronBlade", "Cauldron.TheStranger", "Haka", "Ra", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(stranger);
            PutIntoPlay("GlyphOfDecay");
            Card decay = GetCardInPlay("GlyphOfDecay");
            //{TheStranger} deals 1 target 1 toxic damage.
            GoToUsePowerPhase(stranger);
            DecisionSelectTarget = haka.CharacterCard;
            QuickHPStorage(haka);
            UsePower(decay);
            //1 damage should be dealt
            QuickHPCheck(-1);
        }

        [Test()]
        public void TestGlyphOfInnervationPrevention_FirstDamage()
        {
            SetupGameController("BaronBlade", "Cauldron.TheStranger", "Haka", "Ra", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(stranger);
            PutIntoPlay("GlyphOfInnervation");

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
        public void TestGlyphOfInnervationPrevention_SecondDamage()
        {
            SetupGameController("BaronBlade", "Cauldron.TheStranger", "Haka", "Ra", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(stranger);
            PutIntoPlay("GlyphOfInnervation");

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
        public void TestGlyphOfInnervationPower()
        {
            SetupGameController("BaronBlade", "Cauldron.TheStranger", "Haka", "Ra", "Megalopolis");
            StartGame();

            
            PutIntoPlay("GlyphOfInnervation");
            Card innervation = GetCardInPlay("GlyphOfInnervation");
            //Power: Draw a card.
            GoToUsePowerPhase(stranger);
            QuickHandStorage(stranger);
            UsePower(innervation);
            QuickHandCheck(1);

        }

        [Test()]
        public void TestGlyphOfPerceptionPrevention_FirstDamage()
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
        public void TestGlyphOfPerceptionPrevention_SecondDamage()
        {
            SetupGameController("BaronBlade", "Cauldron.TheStranger", "Haka", "Ra", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(stranger);
            PutIntoPlay("GlyphOfPerception");

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
        public void TestGlyphOfPerception_PlayResponse()
        {
            SetupGameController("BaronBlade", "Cauldron.TheStranger", "Haka", "Ra", "Megalopolis");
            StartGame();

            //put a rune in the hand to play
            Card boneLeech = GetCard("MarkOfTheBoneLeech");
            Card faded = GetCard("MarkOfTheFaded");
            PutInHand(boneLeech);
            PutInHand(faded);

            PutIntoPlay("GlyphOfPerception");
            Card innervation = GetCardInPlay("GlyphOfPerception");
            //When a villain target enters play, you may play a Rune.

            GoToPlayCardPhase(stranger);
            //say yes we want to play a card
            DecisionYesNo = true;
            DecisionSelectCards = new Card[] { boneLeech, haka.CharacterCard };
            //cause a villain target to enter play
            PlayCard("ElementalRedistributor");
            AssertIsInPlay(boneLeech);

            //cause an environment target to enter play
            DecisionSelectCard = faded;
            PlayCard("PoliceBackup");
            AssertInHand(faded);
        }

        [Test()]
        public void TestMarkOfBindingDestroySuccessful()
        {
            SetupGameController("BaronBlade", "Haka", "Cauldron.TheStranger", "Ra", "Megalopolis");
            StartGame();

            GoToEndOfTurn(haka);
            PutIntoPlay("MarkOfBinding");
            Card rune = GetCardInPlay("MarkOfBinding");
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
        public void TestMarkOfBindingDestroyFailed()
        {
            SetupGameController("BaronBlade", "Haka", "Cauldron.TheStranger", "Ra", "Megalopolis");
            StartGame();

            GoToEndOfTurn(haka);
            PutIntoPlay("MarkOfBinding");
            Card rune = GetCardInPlay("MarkOfBinding");
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
        public void TestMarkOfBindingPutNextToTarget()
        {
            SetupGameController("BaronBlade", "Haka", "Cauldron.TheStranger", "Ra", "Megalopolis");
            StartGame();
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            GoToPlayCardPhase(stranger);

            //Play this next to a non-hero target. Reduce damage dealt by that target by 1. 
            DecisionSelectCard = mdp;
            PutIntoPlay("MarkOfBinding");
            Card rune = GetCardInPlay("MarkOfBinding");
            AssertNextToCard(rune, mdp);


        }


        [Test()]
        public void TestMarkOfBindingReduceDamage()
        {
            SetupGameController("BaronBlade", "Haka", "Cauldron.TheStranger", "Ra", "Megalopolis");
            StartGame();
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            GoToPlayCardPhase(stranger);

            //Play this next to a non-hero target. Reduce damage dealt by that target by 1. 
            DecisionSelectCard = mdp;
            PutIntoPlay("MarkOfBinding");
            Card rune = GetCardInPlay("MarkOfBinding");
            QuickHPStorage(haka);
            DealDamage(mdp, haka.CharacterCard, 5, DamageType.Melee);
            //should be reduced by 1, so 4 damage taken
            QuickHPCheck(-4);


        }

        [Test()]
        public void TestMarkOfBreakingDestroySuccessful()
        {
            SetupGameController("BaronBlade", "Haka", "Cauldron.TheStranger", "Ra", "Megalopolis");
            StartGame();

            GoToEndOfTurn(haka);
            PutIntoPlay("MarkOfBreaking");
            Card rune = GetCardInPlay("MarkOfBreaking");
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
        public void TestMarkOfBreakingDestroyFailed()
        {
            SetupGameController("BaronBlade", "Haka", "Cauldron.TheStranger", "Ra", "Megalopolis");
            StartGame();

            GoToEndOfTurn(haka);
            PutIntoPlay("MarkOfBreaking");
            Card rune = GetCardInPlay("MarkOfBreaking");
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
        public void TestMarkOfBreakingPutNextToTarget()
        {
            SetupGameController("BaronBlade", "Haka", "Cauldron.TheStranger", "Ra", "Megalopolis");
            StartGame();
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            GoToPlayCardPhase(stranger);

            //Play this next to a target. Increase damage dealt to that target by 1. 
            DecisionSelectCard = mdp;
            PutIntoPlay("MarkOfBreaking");
            Card rune = GetCardInPlay("MarkOfBreaking");
            AssertNextToCard(rune, mdp);


        }


        [Test()]
        public void TestMarkOfBreakingIncreaseDamage()
        {
            SetupGameController("BaronBlade", "Haka", "Cauldron.TheStranger", "Ra", "Megalopolis");
            StartGame();
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            GoToPlayCardPhase(stranger);

            //Play this next to a target. Increase damage dealt to that target by 1.
            DecisionSelectCard = mdp;
            PutIntoPlay("MarkOfBreaking");
            Card rune = GetCardInPlay("MarkOfBreaking");
            QuickHPStorage(mdp);
            DealDamage(haka.CharacterCard, mdp, 5, DamageType.Melee);
            //should be increase by 1, so 6 damage taken
            QuickHPCheck(-6);


        }

        [Test()]
        public void TestMarkOfQuickeningDestroySuccessful()
        {
            SetupGameController("BaronBlade", "Haka", "Cauldron.TheStranger", "Ra", "Megalopolis");
            StartGame();

            GoToEndOfTurn(haka);
            PutIntoPlay("MarkOfQuickening");
            Card rune = GetCardInPlay("MarkOfQuickening");
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
        public void TestMarkOfQuickeningDestroyFailed()
        {
            SetupGameController("BaronBlade", "Haka", "Cauldron.TheStranger", "Ra", "Megalopolis");
            StartGame();

            GoToEndOfTurn(haka);
            PutIntoPlay("MarkOfQuickening");
            Card rune = GetCardInPlay("MarkOfQuickening");
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
        public void TestMarkOfQuickeningPutNextToTarget()
        {
            SetupGameController("BaronBlade", "Haka", "Cauldron.TheStranger", "Ra", "Megalopolis");
            StartGame();
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            GoToPlayCardPhase(stranger);

            //Play this next to a hero. They may play an additional card during their play phase.
            DecisionSelectCard = haka.CharacterCard;
            PutIntoPlay("MarkOfQuickening");
            Card rune = GetCardInPlay("MarkOfQuickening");
            AssertNextToCard(rune, haka.CharacterCard);


        }


        [Test()]
        public void TestMarkOfQuickeningExtraPlay()
        {
            SetupGameController("BaronBlade", "Cauldron.TheStranger", "Haka", "Ra", "Megalopolis");
            StartGame();
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            GoToPlayCardPhase(stranger);

            //Play this next to a hero. They may play an additional card during their play phase.
            DecisionSelectCard = haka.CharacterCard;
            PutIntoPlay("MarkOfQuickening");
            Card rune = GetCardInPlay("MarkOfQuickening");
            GoToPlayCardPhase(haka);
            //check that haka can play 2 cards
            AssertPhaseActionCount(new int?(2));

        }

        [Test()]
        public void TestMarkOfTheBloodThornDestroySuccessful()
        {
            SetupGameController("BaronBlade", "Haka", "Cauldron.TheStranger", "Ra", "Megalopolis");
            StartGame();

            GoToEndOfTurn(haka);
            PutIntoPlay("MarkOfTheBloodThorn");
            Card rune = GetCardInPlay("MarkOfTheBloodThorn");
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
        public void TestMarkOfTheBloodThornDestroyFailed()
        {
            SetupGameController("BaronBlade", "Haka", "Cauldron.TheStranger", "Ra", "Megalopolis");
            StartGame();

            GoToEndOfTurn(haka);
            PutIntoPlay("MarkOfTheBloodThorn");
            Card rune = GetCardInPlay("MarkOfTheBloodThorn");
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
        public void TestMarkOfTheBloodThornPutNextToTarget()
        {
            SetupGameController("BaronBlade", "Haka", "Cauldron.TheStranger", "Ra", "Megalopolis");
            StartGame();
            GoToPlayCardPhase(stranger);

            //Play this next to a hero target. The first time that target is dealt damage each turn, it deals 1 target 1 toxic damage.
            DecisionSelectCard = haka.CharacterCard;
            PutIntoPlay("MarkOfTheBloodThorn");
            Card rune = GetCardInPlay("MarkOfTheBloodThorn");
            AssertNextToCard(rune, haka.CharacterCard);


        }


        [Test()]
        public void TestMarkOfTheBloodThornDealDamage()
        {
            SetupGameController("BaronBlade", "Cauldron.TheStranger", "Haka", "Ra", "Megalopolis");
            StartGame();
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            GoToPlayCardPhase(stranger);

            //Play this next to a hero target. The first time that target is dealt damage each turn, it deals 1 target 1 toxic damage.
            DecisionSelectCards = new Card[] { haka.CharacterCard, mdp };
            PutIntoPlay("MarkOfTheBloodThorn");
            Card rune = GetCardInPlay("MarkOfTheBloodThorn");

            //counter damage should have occurred
            QuickHPStorage(mdp);
            DealDamage(mdp, haka.CharacterCard, 5, DamageType.Cold);
            QuickHPCheck(-1);

            //counter damage should not have occurred
            QuickHPStorage(mdp);
            DealDamage(mdp, haka.CharacterCard, 5, DamageType.Cold);
            QuickHPCheckZero();

        }

        [Test()]
        public void TestMarkOfTheBoneLeechDestroySuccessful()
        {
            SetupGameController("BaronBlade", "Haka", "Cauldron.TheStranger", "Ra", "Megalopolis");
            StartGame();

            GoToEndOfTurn(haka);
            PutIntoPlay("MarkOfTheBoneLeech");
            Card rune = GetCardInPlay("MarkOfTheBoneLeech");
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
        public void TestMarkOfTheBoneLeechDestroyFailed()
        {
            SetupGameController("BaronBlade", "Haka", "Cauldron.TheStranger", "Ra", "Megalopolis");
            StartGame();

            GoToEndOfTurn(haka);
            PutIntoPlay("MarkOfTheBoneLeech");
            Card rune = GetCardInPlay("MarkOfTheBoneLeech");
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
        public void TestMarkOfTheBoneLeechPutNextToTarget()
        {
            SetupGameController("BaronBlade", "Haka", "Cauldron.TheStranger", "Ra", "Megalopolis");
            StartGame();
            GoToPlayCardPhase(stranger);

            //Play this next to a hero target.The first time that target deals damage each turn, it regains 1HP.DecisionSelectCard = haka.CharacterCard;
            PutIntoPlay("MarkOfTheBoneLeech");
            Card rune = GetCardInPlay("MarkOfTheBoneLeech");
            AssertNextToCard(rune, haka.CharacterCard);


        }


        [Test()]
        public void TestMarkOfTheBoneLeechGainHP()
        {
            SetupGameController("BaronBlade", "Cauldron.TheStranger", "Haka", "Ra", "Megalopolis");
            StartGame();
            //give room to gain hp
            SetHitPoints(haka.CharacterCard, 20);

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            GoToPlayCardPhase(stranger);

            //Play this next to a hero target. The first time that target deals damage each turn, it regains 1HP.            DecisionSelectCards = new Card[] { haka.CharacterCard, mdp };
            DecisionSelectCard = haka.CharacterCard;
            PutIntoPlay("MarkOfTheBoneLeech");
            Card rune = GetCardInPlay("MarkOfTheBoneLeech");

            //hp should have been gained
            QuickHPStorage(haka);
            DealDamage(haka.CharacterCard, mdp, 3, DamageType.Cold);
            QuickHPCheck(1);

            //only gain hp on first damage dealt
            QuickHPStorage(haka);
            DealDamage(haka.CharacterCard, mdp, 3, DamageType.Cold);
            QuickHPCheckZero();

        }

    }
}
