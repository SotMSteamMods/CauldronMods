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
    public class MalichaeVariantTests : BaseTest
    {
        #region MalichaeTestsHelperFunctions
        protected HeroTurnTakerController Malichae { get { return FindHero("Malichae"); } }
        private void SetupIncap(HeroTurnTakerController heroToIncap)
        {
            SetHitPoints(heroToIncap.CharacterCard, 1);
            DealDamage(GameController.FindVillainTurnTakerControllers(true).First(), heroToIncap, 2, DamageType.Melee);
            AssertIncapacitated(heroToIncap);
        }

        #endregion

        [Test()]
        public void TestShardmasterMalichaeLoads()
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae/ShardmasterMalichaeCharacter", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(Malichae);
            Assert.IsInstanceOf(typeof(ShardmasterMalichaeCharacterCardController), Malichae.CharacterCardController);

            Assert.AreEqual(26, Malichae.CharacterCard.HitPoints);
        }

        [Test()]
        public void TestShardmasterMalichaePower()
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae/ShardmasterMalichaeCharacter", "Ra", "Haka", "Megalopolis");
            StartGame();

            GoToUsePowerPhase(Malichae);

            QuickShuffleStorage(Malichae);
            var card = Malichae.TurnTaker.Deck.Cards.First(c => c.DoKeywordsContain("djinn"));
            DecisionSelectCard = card;

            UsePower(Malichae.CharacterCard);

            AssertInHand(card);
            QuickShuffleCheck(1);
        }


        [Test()]
        public void TestShardmasterMalichaeIncap1()
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae/ShardmasterMalichaeCharacter", "Ra", "Haka", "Megalopolis");
            StartGame();

            SetupIncap(Malichae);

            GoToUseIncapacitatedAbilityPhase(Malichae);

            var c1 = StackDeck(ra, "ImbuedFire");
            var c2 = StackDeck(ra, "FireBlast");
            AssertInDeck(c1);
            AssertInDeck(c2);

            DecisionSelectTurnTaker = ra.TurnTaker;
            DecisionSelectCard = c2;

            UseIncapacitatedAbility(Malichae, 0);
            AssertInTrash(c2);
            AssertInDeck(c1);
            AssertOnTopOfDeck(c1);

            AssertNumberOfCardsInRevealed(ra, 0);
        }

        [Test()]
        public void TestShardmasterMalichaeIncap2()
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae/ShardmasterMalichaeCharacter", "Ra", "Haka", "Megalopolis");
            StartGame();

            SetupIncap(Malichae);

            var c1 = PlayCard("LivingForceField");
            AssertIsInPlay(c1);

            GoToUseIncapacitatedAbilityPhase(Malichae);

            DecisionSelectCard = c1;

            UseIncapacitatedAbility(Malichae, 1);
            AssertInTrash(c1);
        }

        [Test()]
        public void TestShardmasterMalichaeIncap3()
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae/ShardmasterMalichaeCharacter", "Ra", "Haka", "Megalopolis");
            StartGame();

            SetupIncap(Malichae);

            GoToUseIncapacitatedAbilityPhase(Malichae);

            DecisionSelectTurnTaker = ra.TurnTaker;

            QuickHandStorage(ra, haka);
            UseIncapacitatedAbility(Malichae, 2);

            QuickHandCheck(1, 0);
        }
    }
}
