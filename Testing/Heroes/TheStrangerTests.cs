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
    public class TheStrangerTests : CauldronBaseTest
    {
        #region TheStrangerHelperFunctions
        private void SetupIncap(TurnTakerController villain)
        {
            SetHitPoints(stranger.CharacterCard, 1);
            DealDamage(villain, stranger, 2, DamageType.Melee);
        }


        #endregion

        [Test()]
        public void TestStrangerLoads()
        {
            SetupGameController("BaronBlade", "Cauldron.TheStranger", "Haka", "Ra", "Megalopolis");

            Assert.AreEqual(5, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(stranger);
            Assert.IsInstanceOf(typeof(TheStrangerCharacterCardController), stranger.CharacterCardController);

            Assert.AreEqual(26, stranger.CharacterCard.HitPoints);
        }

        [Test()]
        public void TestStrangerInnatePower()
        {
            SetupGameController("BaronBlade", "Cauldron.TheStranger", "Haka", "Ra", "Megalopolis");
            StartGame();

            //put runes in hand
            PutInHand("MarkOfBinding");
            PutInHand("MarkOfBreaking");
            Card binding = GetCardFromHand("MarkOfBinding");

            GoToUsePowerPhase(stranger);
            AssertInHand(binding);
            DecisionSelectCard = binding;
            DecisionNextToCard = baron.CharacterCard;
            //Play a rune
            UsePower(stranger.CharacterCard);
            AssertIsInPlay(binding);
        }

        [Test()]
        public void TestStrangerIncap1()
        {
            SetupGameController("BaronBlade", "Cauldron.TheStranger", "Haka", "Ra", "Megalopolis");
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
            SetupGameController("BaronBlade", "Cauldron.TheStranger", "Haka", "Ra", "Megalopolis");
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
            SetupGameController("BaronBlade", "Cauldron.TheStranger", "Haka", "Ra", "Megalopolis");
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
        [Sequential]
        public void TestGlyphPrevention_FirstDamage(
            [Values("GlyphOfInnervation", "GlyphOfCombustion", "GlyphOfPerception", "GlyphOfDecay")] string gylph
            )
        {
            SetupGameController("BaronBlade", "Cauldron.TheStranger", "Haka", "Ra", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(stranger);
            PutIntoPlay(gylph);

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
        [Sequential]
        public void TestGlyphPrevention_OutOfTurnDamage(
            [Values("GlyphOfInnervation", "GlyphOfCombustion", "GlyphOfPerception", "GlyphOfDecay")] string gylph
            )
        {
            SetupGameController("BaronBlade", "Cauldron.TheStranger", "Haka", "Ra", "Megalopolis");
            StartGame();

            PutIntoPlay(gylph);

            GoToPlayCardPhase(haka);

            //Once during your turn when TheStranger would deal himself damage, prevent that damage.
            QuickHPStorage(stranger);
            DecisionYesNo = true; //we say yes, but shouldn't be prompted at all
            DealDamage(stranger, stranger, 5, DamageType.Sonic);
            AssertNoDecision();
            //since damage was prevented, no change in health
            QuickHPCheck(-5);
        }


        [Test()]
        [Sequential]
        public void TestGlyphPrevention_SecondDamage(
            [Values("GlyphOfInnervation", "GlyphOfCombustion", "GlyphOfPerception", "GlyphOfDecay")] string gylph
            )
        {
            SetupGameController("BaronBlade", "Cauldron.TheStranger", "Haka", "Ra", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(stranger);
            PutIntoPlay(gylph);

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
        public void TestRuneFallsOff()
        {
            SetupGameController("BaronBlade", "Haka", "Cauldron.TheStranger", "Ra", "Megalopolis");
            StartGame();

            Card mdp = GetCardInPlay("MobileDefensePlatform");

            GoToEndOfTurn(haka);
            DecisionSelectCard = mdp;
            Card rune = PutIntoPlay("MarkOfBinding");
            AssertNextToCard(rune, mdp);

            DestroyCard(mdp, baron.CharacterCard);
            AssertInPlayArea(baron, rune);

            DecisionYesNo = true;
            QuickHPStorage(stranger);
            GoToStartOfTurn(stranger);
            //should have been destroyed and no damage dealt
            AssertInTrash(rune);
            QuickHPCheckZero();

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
        public void TestCorruption_CannotDraw()
        {
            SetupGameController("BaronBlade", "Haka", "Cauldron.TheStranger", "Ra", "Megalopolis");
            StartGame();

            PlayCard("TrafficPileup");
            GoToPlayCardPhase(stranger);

            Card corruption = PutOnDeck("Corruption");
            AssertNoDecision();
            //Draw up to 4 cards.
            QuickHPStorage(stranger);
            QuickHandStorage(stranger);
            DecisionYesNo = true;
            PlayCard(corruption);

            //For each card drawn this way, TheStranger deals himself 1 toxic damage.
            QuickHPCheck(0);
            QuickHandCheckZero();
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
            DecisionDoNotSelectCard = SelectionType.PlayCard;

            PlayCard("FlickeringWeb");
            AssertInHand(boneLeech, faded, twistedShadow);

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
        public void TestGlyphOfDecayPower_NoPlayIfNoRunes()
        {
            SetupGameController("BaronBlade", "Cauldron.TheStranger", "Haka", "Ra", "Megalopolis");
            StartGame();

            //discard hand
            DiscardAllCards(stranger);
            //put a non-rune in hand
            Card glyph = GetCard("GlyphOfCombustion");
            PutInHand(glyph);

            GoToPlayCardPhase(stranger);
            PutIntoPlay("GlyphOfDecay");
            Card decay = GetCardInPlay("GlyphOfDecay");
            //Power: You may play a Rune. 
            GoToUsePowerPhase(stranger);
            UsePower(decay);
            AssertNotInPlay(glyph);
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
            DecisionDoNotSelectCard = SelectionType.PlayCard;
            QuickHPStorage(haka);
            UsePower(decay);
            //1 damage should be dealt
            QuickHPCheck(-1);
        }

        [Test()]
        public void TestGlyphOfInnervationPower()
        {
            SetupGameController("BaronBlade", "Cauldron.TheStranger", "Haka", "Ra", "Megalopolis");
            StartGame();

            Card innervation = PlayCard("GlyphOfInnervation");
            //Power: Draw a card.
            GoToUsePowerPhase(stranger);
            QuickHandStorage(stranger);
            UsePower(innervation);
            QuickHandCheck(1);
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
        public void TestGlyphOfPerception_PlayResponse_NoPlayIfNoRunes()
        {
            SetupGameController("BaronBlade", "Cauldron.TheStranger", "Haka", "Ra", "Megalopolis");
            StartGame();

            //discard hand
            DiscardAllCards(stranger);
            //put a non-rune in hand
            Card glyph = GetCard("GlyphOfCombustion");
            PutInHand(glyph);

            PutIntoPlay("GlyphOfPerception");
            //cause a villain target to enter play
            PlayCard("ElementalRedistributor");
            AssertNotInPlay(glyph);
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
            Card livingForceField = PlayCard("LivingForceField");
            GoToPlayCardPhase(stranger);

            //Play this next to a non-hero target. Reduce damage dealt by that target by 1. 
            DecisionSelectCard = mdp;
            AssertNextDecisionChoices(
                new List<Card>() { baron.CharacterCard, mdp },
                new List<Card>() { livingForceField, haka.CharacterCard, stranger.CharacterCard, ra.CharacterCard });

            PutIntoPlay("MarkOfBinding");
            Card rune = GetCardInPlay("MarkOfBinding");
            AssertNextToCard(rune, mdp);
        }

        [Test()]
        public void TestMarkOfBinding_Oblivaeon_0Heroes()
        {
            SetupGameController(new string[] { "OblivAeon", "Cauldron.TheStranger", "Legacy", "Haka", "Tachyon", "Luminary", "Cauldron.WindmillCity", "Cauldron.FSCContinuanceWanderer", "InsulaPrimalis", "Cauldron.VaultFive", "Cauldron.Northspar" }, shieldIdentifier: "PrimaryObjective");
            StartGame();
            DestroyNonCharacterVillainCards();

            SwitchBattleZone(oblivaeon);
            //Play this next to a non-hero target. Reduce damage dealt by that target by 1. 
            //since there are no heroes in this battlezone, it should go to the trash
            Card binding = PlayCard("MarkOfBinding");
            AssertInTrash(binding);

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
        public void TestMarkOfQuickeningModifiesCurrentPhase()
        {
            SetupGameController("BaronBlade", "Cauldron.TheStranger", "Haka", "Ra", "Megalopolis");
            StartGame();
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            GoToPlayCardPhase(stranger);

            //Play this next to a hero. They may play an additional card during their play phase.
            DecisionSelectCard = stranger.CharacterCard;
            Card mark = PlayCard("MarkOfQuickening");

            AssertPhaseActionCount(new int?(1));

            DestroyCard(mark);
            AssertPhaseActionCount(0);
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

            //Play this next to a hero target. The first time that target deals damage each turn, it regains 1HP.
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


        [Test()]
        public void TestMarkOfTheFadedDestroySuccessful()
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
        public void TestMarkOfTheFadedDestroyFailed()
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
        public void TestMarkOfTheFadedPutNextToTarget()
        {
            SetupGameController("BaronBlade", "Haka", "Cauldron.TheStranger", "Ra", "Megalopolis");
            StartGame();
            GoToPlayCardPhase(stranger);

            //Play this next to a hero target. When that target would be dealt damage by a non-hero card, you may redirect that damage to a hero with higher HP.
            PutIntoPlay("MarkOfTheFaded");
            Card rune = GetCardInPlay("MarkOfTheFaded");
            AssertNextToCard(rune, haka.CharacterCard);
        }


        [Test()]
        public void TestMarkOfTheFadedRedirect()
        {
            SetupGameController("BaronBlade", "Cauldron.TheStranger", "Haka", "Ra", "Megalopolis");
            StartGame();

            SetHitPoints(haka.CharacterCard, 20);

            Card mdp = GetCardInPlay("MobileDefensePlatform");
            GoToPlayCardPhase(stranger);

            //Play this next to a hero target. When that target would be dealt damage by a non-hero card, you may redirect that damage to a hero with higher HP.
            DecisionSelectCards = new Card[] { haka.CharacterCard, ra.CharacterCard };
            PutIntoPlay("MarkOfTheFaded");
            Card rune = GetCardInPlay("MarkOfTheFaded");

            QuickHPStorage(haka, ra);
            DecisionSelectTarget = ra.CharacterCard;

            DealDamage(mdp, haka.CharacterCard, 3, DamageType.Cold);
            //damage should been redirected to Ra
            QuickHPCheck(0, -3);
        }

        [Test()]
        public void TestMarkOfTheTwistedShadowDestroySuccessful()
        {
            SetupGameController("BaronBlade", "Haka", "Cauldron.TheStranger", "Ra", "Megalopolis");
            StartGame();

            GoToEndOfTurn(haka);
            PutIntoPlay("MarkOfTheTwistedShadow");
            Card rune = GetCardInPlay("MarkOfTheTwistedShadow");
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
        public void TestMarkOfTheTwistedShadowDestroyFailed()
        {
            SetupGameController("BaronBlade", "Haka", "Cauldron.TheStranger", "Ra", "Megalopolis");
            StartGame();

            GoToEndOfTurn(haka);
            PutIntoPlay("MarkOfTheTwistedShadow");
            Card rune = GetCardInPlay("MarkOfTheTwistedShadow");
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
        public void TestMarkOfTheTwistedShadowPutNextToTarget()
        {
            SetupGameController("BaronBlade", "Haka", "Cauldron.TheStranger", "Ra", "Megalopolis");
            StartGame();
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            GoToPlayCardPhase(stranger);

            // Play this next to a hero target.Increase damage dealt by that target by 1.
            DecisionSelectCard = haka.CharacterCard;
            PutIntoPlay("MarkOfTheTwistedShadow");
            Card rune = GetCardInPlay("MarkOfTheTwistedShadow");
            AssertNextToCard(rune, haka.CharacterCard);
        }


        [Test()]
        public void TestMarkOfTheTwistedShadowIncreaseDamage()
        {
            SetupGameController("BaronBlade", "Haka", "Cauldron.TheStranger", "Ra", "Megalopolis");
            StartGame();
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            GoToPlayCardPhase(stranger);

            // Play this next to a hero target.Increase damage dealt by that target by 1.
            DecisionSelectCard = haka.CharacterCard;
            PutIntoPlay("MarkOfTheTwistedShadow");
            Card rune = GetCardInPlay("MarkOfTheTwistedShadow");
            QuickHPStorage(mdp);
            DealDamage(haka.CharacterCard, mdp, 5, DamageType.Melee);
            //should be increased by 1, so 6 damage taken
            QuickHPCheck(-6);
        }

        [Test()]
        public void TestMarkOfDestructionPutNextToTarget()
        {
            SetupGameController("BaronBlade", "Haka", "Cauldron.TheStranger", "Ra", "Megalopolis");
            StartGame();
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            GoToPlayCardPhase(stranger);

            //Play this next to a non-character card. If either card is destroyed, destroy the other. 
            DecisionSelectCard = mdp;
            PutIntoPlay("MarkOfDestruction");
            Card rune = GetCardInPlay("MarkOfDestruction");
            AssertNextToCard(rune, mdp);
        }

        [Test()]
        public void TestMarkOfDestruction_Oblivaeon_0Heroes()
        {
            SetupGameController(new string[] { "OblivAeon", "Cauldron.TheStranger", "Legacy", "Haka", "Tachyon", "Luminary", "Cauldron.WindmillCity", "Cauldron.FSCContinuanceWanderer", "InsulaPrimalis", "Cauldron.VaultFive", "Cauldron.Northspar" }, shieldIdentifier: "PrimaryObjective");
            StartGame();
            DestroyNonCharacterVillainCards();

            SwitchBattleZone(oblivaeon);
            //Play this next to a non-character card. If either card is destroyed, destroy the other. 
            //since there are no non-character cards in this battlezone, it should go to the trash
            Card destruction = PlayCard("MarkOfDestruction");
            AssertInTrash(destruction);

        }


        [Test()]
        public void TestMarkOfDestruction_NextToDestroyed()
        {
            SetupGameController("BaronBlade", "Haka", "Cauldron.TheStranger", "Ra", "Megalopolis");
            StartGame();
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            GoToPlayCardPhase(stranger);

            //Play this next to a non-character card. If either card is destroyed, destroy the other.
            DecisionSelectCard = mdp;
            PutIntoPlay("MarkOfDestruction");
            Card rune = GetCardInPlay("MarkOfDestruction");

            //destroy mdp, this should destroy the mark of destruction
            AssertIsInPlay(rune);
            DestroyCard(mdp, haka.CharacterCard);
            AssertInTrash(rune);
        }

        [Test()]
        public void TestMarkOfDestruction_ThisCardDestroyed()
        {
            SetupGameController("BaronBlade", "Haka", "Cauldron.TheStranger", "Ra", "Megalopolis");
            StartGame();
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            GoToPlayCardPhase(stranger);

            //Play this next to a non-character card. If either card is destroyed, destroy the other. 
            DecisionSelectCard = mdp;
            PutIntoPlay("MarkOfDestruction");
            Card rune = GetCardInPlay("MarkOfDestruction");

            //destroy mark of destruction, this should destroy mdp
            AssertIsInPlay(mdp);
            DestroyCard(rune, baron.CharacterCard);
            AssertInTrash(mdp);
        }

        [Test()]
        public void TestMarkOfDestruction_Redirect()
        {
            SetupGameController("BaronBlade", "Haka", "Cauldron.TheStranger", "Ra", "Megalopolis");
            StartGame();

            SetHitPoints(haka.CharacterCard, 15);
            SetHitPoints(ra.CharacterCard, 25);
            SetHitPoints(stranger.CharacterCard, 20);
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            GoToPlayCardPhase(stranger);

            DecisionSelectCard = mdp;
            PutIntoPlay("MarkOfDestruction");
            Card rune = GetCardInPlay("MarkOfDestruction");

            //Redirect damage dealt to this card by non-hero targets to the hero target with the highest HP.
            //ra has the highest hp of hero targests
            QuickHPStorage(ra);
            DealDamage(baron.CharacterCard, rune, 5, DamageType.Projectile);
            QuickHPCheck(-5);
        }

        [Test()]
        public void TestTheOldRoads_GlyphFromTrash()
        {
            SetupGameController("BaronBlade", "Haka", "Cauldron.TheStranger", "Ra", "Megalopolis");
            StartGame();

            //put a glyph in trash
            Card glyph = GetCard("GlyphOfDecay");
            PutInTrash(stranger, glyph);

            //Put a Glyph from your trash into your hand, or reveal cards from the top of your deck until you reveal a Glyph, put it into play, and shuffle the other revealed cards into your deck. 
            //taking glyph from trash
            DecisionSelectFunction = 0;
            AssertInTrash(glyph);
            PutIntoPlay("TheOldRoads");
            AssertInHand(glyph);
        }

        [Test()]
        public void TestTheOldRoads_GlyphFromDeck()
        {
            SetupGameController("BaronBlade", "Haka", "Cauldron.TheStranger", "Ra", "Megalopolis");
            StartGame();

            Card roads = GetCard("TheOldRoads");
            PutInHand(roads);
            //stack so glyph is second card in the deck
            Card glyph = GetCard("GlyphOfDecay");
            PutOnDeck(stranger, glyph);
            PutOnDeck("Corruption");

            //Put a Glyph from your trash into your hand, or reveal cards from the top of your deck until you reveal a Glyph, put it into play, and shuffle the other revealed cards into your deck. 

            //revealing cards from deck
            DecisionSelectFunction = 1;
            AssertInDeck(glyph);
            int numCardsInDeckBefore = GetNumberOfCardsInDeck(stranger);
            PlayCard(roads);

            AssertIsInPlay(glyph);
            //2 cards off deck, 1 for glyph, 1 for card draw
            AssertNumberOfCardsInDeck(stranger, numCardsInDeckBefore - 2);
        }

        [Test()]
        public void TestTheOldRoads_DrawCard()
        {
            SetupGameController("BaronBlade", "Haka", "Cauldron.TheStranger", "Ra", "Megalopolis");
            StartGame();

            // put a glyph in trash
            Card glyph = GetCard("GlyphOfDecay");
            PutInTrash(stranger, glyph);

            //put card on top of deck to draw
            Card web = GetCard("FlickeringWeb");
            PutOnDeck(stranger, web);

            //You may draw card
            DecisionSelectFunction = 0;
            //yes we want to draw a card
            DecisionYesNo = true;
            AssertInDeck(web);
            PutIntoPlay("TheOldRoads");
            //flickering web should have moved from deck to hand
            AssertInHand(web);
        }

        [Test()]
        public void TestUnweaveShuffle4_Gain4()
        {
            SetupGameController("BaronBlade", "Haka", "Cauldron.TheStranger", "Ra", "Megalopolis");
            StartGame();

            SetHitPoints(stranger.CharacterCard, 10);
            Card rune1 = GetCard("MarkOfBinding");
            Card rune2 = GetCard("MarkOfBreaking");
            Card rune3 = GetCard("MarkOfDestruction");
            Card rune4 = GetCard("MarkOfQuickening");
            PutInTrash(stranger, rune1);
            PutInTrash(stranger, rune2);
            PutInTrash(stranger, rune3);
            PutInTrash(stranger, rune4);

            //You may shuffle up to 4 Runes from your trash into your deck, or discard up to 4 cards.",
            //For each card shuffled or discarded this way, {TheStranger} may draw a card or regain 1HP."
            //shuffle, gain hp, gain hp, gain hp, gain hp
            DecisionSelectFunctions = new int?[] { 0, 1, 1, 1, 1 };
            DecisionSelectCards = new Card[] { rune1, rune2, rune3, rune4 };
            QuickHPStorage(stranger);
            PutIntoPlay("Unweave");
            QuickHPCheck(4);
            AssertInDeck(rune1);
            AssertInDeck(rune2);
            AssertInDeck(rune3);
            AssertInDeck(rune4);
        }

        [Test()]
        public void TestUnweaveShuffle4_Draw4()
        {
            SetupGameController("BaronBlade", "Haka", "Cauldron.TheStranger", "Ra", "Megalopolis");
            StartGame();

            SetHitPoints(stranger.CharacterCard, 10);
            Card rune1 = GetCard("MarkOfBinding");
            Card rune2 = GetCard("MarkOfBreaking");
            Card rune3 = GetCard("MarkOfDestruction");
            Card rune4 = GetCard("MarkOfQuickening");
            PutInTrash(stranger, rune1);
            PutInTrash(stranger, rune2);
            PutInTrash(stranger, rune3);
            PutInTrash(stranger, rune4);

            //You may shuffle up to 4 Runes from your trash into your deck, or discard up to 4 cards.",
            //For each card shuffled or discarded this way, {TheStranger} may draw a card or regain 1HP."
            //shuffle,draw, draw, draw, draw
            DecisionSelectFunctions = new int?[] { 0, 0, 0, 0, 0 };
            DecisionSelectCards = new Card[] { rune1, rune2, rune3, rune4 };
            QuickHandStorage(stranger);
            PutIntoPlay("Unweave");
            QuickHandCheck(4);
            AssertNotInTrash(rune1);
            AssertNotInTrash(rune2);
            AssertNotInTrash(rune3);
            AssertNotInTrash(rune4);
        }

        [Test()]
        public void TestUnweaveDiscard4_Draw4()
        {
            SetupGameController("BaronBlade", "Haka", "Cauldron.TheStranger", "Ra", "Megalopolis");
            StartGame();

            SetHitPoints(stranger.CharacterCard, 10);
            Card rune1 = GetCard("MarkOfBinding");
            Card rune2 = GetCard("MarkOfBreaking");
            Card rune3 = GetCard("MarkOfDestruction");
            Card rune4 = GetCard("MarkOfQuickening");
            PutInHand(stranger, rune1);
            PutInHand(stranger, rune2);
            PutInHand(stranger, rune3);
            PutInHand(stranger, rune4);

            //You may shuffle up to 4 Runes from your trash into your deck, or discard up to 4 cards.",
            //For each card shuffled or discarded this way, {TheStranger} may draw a card or regain 1HP."
            //discard, draw, draw, draw, draw
            DecisionSelectFunctions = new int?[] { 1, 0, 0, 0, 0 };
            DecisionSelectCards = new Card[] { rune1, rune2, rune3, rune4 };
            QuickHandStorage(stranger);
            PutIntoPlay("Unweave");
            QuickHandCheck(0);
            AssertInTrash(rune1);
            AssertInTrash(rune2);
            AssertInTrash(rune3);
            AssertInTrash(rune4);
        }

        [Test()]
        public void TestUnweaveDiscard4_Gain4()
        {
            SetupGameController("BaronBlade", "Haka", "Cauldron.TheStranger", "Ra", "Megalopolis");
            StartGame();

            SetHitPoints(stranger.CharacterCard, 10);
            Card rune1 = GetCard("MarkOfBinding");
            Card rune2 = GetCard("MarkOfBreaking");
            Card rune3 = GetCard("MarkOfDestruction");
            Card rune4 = GetCard("MarkOfQuickening");
            PutInHand(stranger, rune1);
            PutInHand(stranger, rune2);
            PutInHand(stranger, rune3);
            PutInHand(stranger, rune4);

            //You may shuffle up to 4 Runes from your trash into your deck, or discard up to 4 cards.",
            //For each card shuffled or discarded this way, {TheStranger} may draw a card or regain 1HP."
            //discard, gain hp, gain hp, gain hp, gain hp
            DecisionSelectFunctions = new int?[] { 1, 1, 1, 1, 1 };
            DecisionSelectCards = new Card[] { rune1, rune2, rune3, rune4 };
            QuickHPStorage(stranger);
            PutIntoPlay("Unweave");
            QuickHPCheck(4);
            AssertInTrash(rune1);
            AssertInTrash(rune2);
            AssertInTrash(rune3);
            AssertInTrash(rune4);
        }

        [Test()]
        public void TestWhisperedSigns_Draw2()
        {
            SetupGameController("BaronBlade", "Haka", "Cauldron.TheStranger", "Ra", "Megalopolis");
            StartGame();
            Card signs = GetCard("WhisperedSigns");
            PutInHand(signs);
            //You may draw 2 cards or put a Rune from your trash into your hand.
            DecisionSelectFunction = 0;
            QuickHandStorage(stranger);
            int numCardsInDeckBefore = GetNumberOfCardsInDeck(stranger);
            DecisionDoNotSelectCard = SelectionType.PlayCard;
            PlayCard(signs);
            //should be +1 card in hand, drew 2, played 1
            QuickHandCheck(1);
            AssertNumberOfCardsInDeck(stranger, numCardsInDeckBefore - 2);
        }

        [Test()]
        public void TestWhisperedSigns_RetrieveRune()
        {
            SetupGameController("BaronBlade", "Haka", "Cauldron.TheStranger", "Ra", "Megalopolis");
            StartGame();

            Card signs = GetCard("WhisperedSigns");
            PutInHand(signs);
            Card rune = GetCard("MarkOfBinding");
            PutInTrash(stranger, rune);

            //You may draw 2 cards or put a Rune from your trash into your hand.
            DecisionSelectFunction = 1;
            QuickHandStorage(stranger);
            int numCardsInTrashBefore = GetNumberOfCardsInTrash(stranger);
            AssertInTrash(rune);
            DecisionDoNotSelectCard = SelectionType.PlayCard;
            PlayCard(signs);
            //should be +0, played 1, moved 1 into hand
            QuickHandCheckZero();
            //should be +0, as whispered signs moved to trash, and we retrieved card from trash
            AssertNumberOfCardsInTrash(stranger, numCardsInTrashBefore);
            AssertInHand(rune);
        }

        [Test()]
        public void TestWhisperedSigns_PlayRune()
        {
            SetupGameController("BaronBlade", "Haka", "Cauldron.TheStranger", "Ra", "Megalopolis");
            StartGame();

            Card signs = GetCard("WhisperedSigns");
            PutInHand(signs);
            Card rune = GetCard("MarkOfBinding");
            PutInHand(rune);

            //You may play a Rune or Glyph now.

            DecisionSelectCards = new Card[] { rune, baron.CharacterCard };
            AssertInHand(rune);
            PlayCard(signs);
            AssertIsInPlay(rune);
        }

        [Test()]
        public void TestWhisperedSigns_PlayGlyph()
        {
            SetupGameController("BaronBlade", "Haka", "Cauldron.TheStranger", "Ra", "Megalopolis");
            StartGame();

            Card signs = GetCard("WhisperedSigns");
            PutInHand(signs);
            Card glyph = GetCard("GlyphOfDecay");
            PutInHand(glyph);

            //You may play a Rune or Glyph now.

            DecisionSelectCard = glyph;
            AssertInHand(glyph);
            PlayCard(signs);
            AssertIsInPlay(glyph);
        }

        [Test()]
        public void TestWhisperedSign_NoPlayIfNoRuneOrGlyph()
        {
            SetupGameController("BaronBlade", "Cauldron.TheStranger", "Haka", "Ra", "Megalopolis");
            StartGame();

            //discard hand
            DiscardAllCards(stranger);
            //put a non-rune in hand
            Card unweave = GetCard("Unweave", 0);
            PutInHand(unweave);

            Card signs = GetCard("WhisperedSigns");
            PutInHand(signs);
            PutOnDeck(stranger, GetCard("Unweave", 1));
            PutOnDeck(stranger, GetCard("Unweave", 2));
            //You may draw 2 cards or put a Rune from your trash into your hand.
            DecisionSelectFunction = 0;
            QuickHandStorage(stranger);
            DecisionDoNotSelectCard = SelectionType.PlayCard;
            PlayCard(signs);
            AssertNotInTrash(unweave);
        }
    }
}
