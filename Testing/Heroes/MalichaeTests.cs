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

    }
}
