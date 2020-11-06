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




    }
}
