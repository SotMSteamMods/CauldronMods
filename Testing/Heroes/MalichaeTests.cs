using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using Cauldron.Malichae;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CauldronTests
{
    [TestFixture()]
    public class MalichaeTests : BaseTest
    {
        #region MalichaeTestsHelperFunctions
        protected HeroTurnTakerController Malichae { get { return FindHero("Malichae"); } }
        private void SetupIncap(TurnTakerController villain)
        {
            SetHitPoints(Malichae.CharacterCard, 1);
            DealDamage(villain, Malichae, 2, DamageType.Melee);
            AssertIncapacitated(Malichae);
        }

        #endregion

        [Test()]
        public void TestMalichaeLoads()
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(Malichae);
            Assert.IsInstanceOf(typeof(MalichaeCharacterCardController), Malichae.CharacterCardController);

            Assert.AreEqual(27, Malichae.CharacterCard.HitPoints);
        }

        [Test]
        public void InnatePower()
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            GoToUsePowerPhase(Malichae);

            QuickHandStorage(Malichae, ra, fanatic);
            var discard = GetCardFromHand(Malichae);
            DecisionDiscardCard = discard;
            UsePower(Malichae);

            QuickHandCheck(1, 0, 0);
            AssertInTrash(Malichae, discard);
        }

        [Test]
        public void Incap1_DiscardDraw()
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            SetupIncap(baron);
            GoToUseIncapacitatedAbilityPhase(Malichae);

            QuickHandStorage(Malichae, ra, fanatic);

            var discard = GetCardFromHand(ra);

            DecisionSelectTurnTaker = ra.TurnTaker;
            DecisionDiscardCard = discard;

            UseIncapacitatedAbility(Malichae, 0);
            QuickHandCheck(0, 2, 0);
            AssertInTrash(ra, discard);
        }

        [Test]
        public void Incap1_NoDiscardNoDraw()
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            SetupIncap(baron);
            GoToUseIncapacitatedAbilityPhase(Malichae);

            QuickHandStorage(Malichae, ra, fanatic);

            DecisionSelectTurnTaker = ra.TurnTaker;
            DecisionSelectCards = new Card[1];

            UseIncapacitatedAbility(Malichae, 0);
            QuickHandCheck(0, 0, 0);
            AssertNumberOfCardsInTrash(Malichae, 0);
        }

        [Test]
        public void Incap2()
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            SetupIncap(baron);
            GoToUseIncapacitatedAbilityPhase(Malichae);
            var card = PlayCard(env, "TrafficPileup");
            AssertInPlayArea(env, card);

            DecisionSelectCard = card;

            UseIncapacitatedAbility(Malichae, 1);

            AssertNumberOfCardsInTrash(env, 1);
        }

        [Test]
        public void Incap3_DamageTypeChanged()
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            SetupIncap(baron);
            GoToUseIncapacitatedAbilityPhase(Malichae);
            var card = PlayCard(ra, "FleshOfTheSunGod");
            AssertInPlayArea(ra, card);

            DecisionSelectDamageType = DamageType.Fire;

            UseIncapacitatedAbility(Malichae, 2);

            QuickHPStorage(baron, ra, fanatic);
            //If the damage type is changed, the damage will be prevented
            DealDamage(fanatic, ra, 2, DamageType.Melee);
            QuickHPCheck(0, 0, 0);

            QuickHPStorage(baron, ra, fanatic);
            //If the damage type is changed, the damage will be prevented
            DealDamage(ra, ra, 2, DamageType.Melee);
            QuickHPCheck(0, 0, 0);

            QuickHPStorage(baron, ra, fanatic);
            //villian damage is uneffected
            DealDamage(baron, ra, 2, DamageType.Melee);
            QuickHPCheck(0, -2, 0);
        }

        [Test]
        public void Incap3_EffectExpires()
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            SetupIncap(baron);
            GoToUseIncapacitatedAbilityPhase(Malichae);
            var card = PlayCard(ra, "FleshOfTheSunGod");
            AssertInPlayArea(ra, card);

            DecisionSelectDamageType = DamageType.Fire;

            UseIncapacitatedAbility(Malichae, 2);

            GoToEndOfTurn(Malichae);

            GoToStartOfTurn(Malichae);

            //effect is now expired

            QuickHPStorage(baron, ra, fanatic);
            //If the damage type is changed, the damage will be prevented
            DealDamage(fanatic, ra, 2, DamageType.Melee);
            QuickHPCheck(0, -2, 0);

            QuickHPStorage(baron, ra, fanatic);
            //If the damage type is changed, the damage will be prevented
            DealDamage(ra, ra, 2, DamageType.Melee);
            QuickHPCheck(0, -2, 0);

            QuickHPStorage(baron, ra, fanatic);
            //villian damage is uneffected
            DealDamage(baron, ra, 2, DamageType.Melee);
            QuickHPCheck(0, -2, 0);
        }

        [Test]
        public void BaitAndSwitch_Discard_PlayDjinn()
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            //2nd discard for the UsePower innate
            var discard1 = GetCard("GrandBathiel");
            var discard2 = GetCardFromHand(Malichae);
            PutInHand(Malichae, discard1);

            var card = PutInHand(Malichae, "BaitAndSwitch");
            var trash = PutInTrash("Bathiel");
            
            DiscardTopCards(Malichae, 10); //load other cards in the trash

            var trashCount = GetNumberOfCardsInTrash(Malichae);

            GoToPlayCardPhase(Malichae);

            QuickHandStorage(Malichae, ra, fanatic);

            DecisionSelectCards = new Card[] { discard1, trash, discard2 };
            DecisionSelectFunction = 0; //put djinn into play
            DecisionSelectPower = Malichae.CharacterCard;

            PlayCard(card);
            AssertInTrash(Malichae, card);
            AssertInTrash(Malichae, discard1);
            AssertInPlayArea(Malichae, trash);
            AssertInTrash(Malichae, discard2);
            AssertNumberOfCardsInTrash(Malichae, trashCount + 2);
            //played a card, discarded a card, draw 2, discard 1
            //-1 + -1 + 2 + -1 = -1
            QuickHandCheck(-1, 0, 0);
        }

        [Test]
        public void BaitAndSwitch_Discard_DrawCards()
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            //2nd discard for the UsePower innate
            var discard1 = GetCard("GrandBathiel");
            var discard2 = GetCardFromHand(Malichae);
            PutInHand(Malichae, discard1);

            var card = PutInHand(Malichae, "BaitAndSwitch");
            var trash = PutInTrash("Bathiel");

            DiscardTopCards(Malichae, 10); //load other cards in the trash

            var trashCount = GetNumberOfCardsInTrash(Malichae);

            GoToPlayCardPhase(Malichae);

            QuickHandStorage(Malichae, ra, fanatic);

            DecisionSelectCards = new Card[] { discard1, discard2 };
            DecisionSelectFunction = 1; //draw cards
            DecisionSelectPower = Malichae.CharacterCard;

            PlayCard(card);
            AssertInTrash(Malichae, card);
            AssertInTrash(Malichae, discard1);
            AssertInTrash(Malichae, trash);
            AssertInTrash(Malichae, discard2);
            AssertNumberOfCardsInTrash(Malichae, trashCount + 3);
            //played a card, discarded a card, draw 2, draw 2, discard 1
            //-1 + -1 + 2 + 2 + -1 = 1
            QuickHandCheck(1, 0, 0);
        }


        [Test]
        public void BaitAndSwitch_NoDiscard()
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            //2nd discard for the UsePower innate
            var discard1 = GetCard("GrandBathiel");
            var discard2 = GetCardFromHand(Malichae);
            PutInHand(Malichae, discard1);

            var card = PutInHand(Malichae, "BaitAndSwitch");
            var trash = PutInTrash("Bathiel");

            DiscardTopCards(Malichae, 10); //load other cards in the trash

            var trashCount = GetNumberOfCardsInTrash(Malichae);

            GoToPlayCardPhase(Malichae);

            QuickHandStorage(Malichae, ra, fanatic);

            DecisionDoNotSelectCard = SelectionType.DiscardCard;
            
            DecisionSelectCards = new Card[] { null, discard2 };
            DecisionSelectPower = Malichae.CharacterCard;

            PlayCard(card);
            AssertInTrash(Malichae, card);
            AssertInHand(Malichae, discard1);
            AssertInTrash(Malichae, trash);
            AssertInTrash(Malichae, discard2);
            AssertNumberOfCardsInTrash(Malichae, trashCount + 2);
            //played a card, draw 2, discard 1
            //-1 + 2 + -1 = 0
            QuickHandCheck(0, 0, 0);
        }

        [Test]
        public void DjinnTarget_IsTarget()
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(Malichae);

            string djinn = "Bathiel";

            var card = PlayCard(djinn);
            AssertInPlayArea(Malichae, card);
            AssertIsTarget(card, 5);
            AssertCardHasKeyword(card, "djinn", false);
        }

        [Test]
        public void DjinnTarget_IsImmuneToDamageType()
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(Malichae);

            string djinn = "Bathiel";
            DamageType type = DamageType.Energy;

            var card = PlayCard(djinn);
            AssertInPlayArea(Malichae, card);

            QuickHPStorage(baron.CharacterCard, Malichae.CharacterCard, ra.CharacterCard, fanatic.CharacterCard, card);
            DealDamage(baron.CharacterCard, card, 1, type, true); //damage is irreducible to ensure it's only the immunity that blocks the damage
            QuickHPCheck(0, 0, 0, 0, 0); //no damage

            QuickHPStorage(baron.CharacterCard, Malichae.CharacterCard, ra.CharacterCard, fanatic.CharacterCard, card);
            DealDamage(baron.CharacterCard, card, 1, DamageType.Melee, true); //damage is irreducible to ensure it's only the immunity that blocks the damage
            QuickHPCheck(0, 0, 0, 0, -1); //no damage
        }

        [Test]
        public void DjinnTarget_ReturnsToHandIfDestroyed()
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            GoToPlayCardPhase(Malichae);

            string djinn = "Bathiel";

            PrintSeparator("Destroy Action");
            var card = PlayCard(djinn);
            AssertInPlayArea(Malichae, card);

            DestroyCard(card, baron.CharacterCard);
            AssertInHand(Malichae, card);

            PrintSeparator("Damage Action");
            PlayCard(card);
            AssertInPlayArea(Malichae, card);

            DealDamage(baron.CharacterCard, card, 99, DamageType.Melee, true);
            AssertInHand(Malichae, card);
        }

    }
}
