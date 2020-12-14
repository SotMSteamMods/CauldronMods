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



        [Test()]
        public void TestMinistryOfStrategicScienceMalichaeLoads()
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae/MinistryOfStrategicScienceMalichaeCharacter", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(Malichae);
            Assert.IsInstanceOf(typeof(MinistryOfStrategicScienceMalichaeCharacterCardController), Malichae.CharacterCardController);

            Assert.AreEqual(24, Malichae.CharacterCard.HitPoints);
        }

        [Test()]
        [Ignore("Not Implemented")]
        public void TestMinistryOfStrategicScienceMalichaePower()
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae/MinistryOfStrategicScienceMalichaeCharacter", "Ra", "Haka", "Megalopolis");
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
        public void TestMinistryOfStrategicScienceMalichaeIncap1()
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae/MinistryOfStrategicScienceMalichaeCharacter", "Ra", "Haka", "Megalopolis");
            StartGame();
            var mdp = GetMobileDefensePlatform().Card;

            SetupIncap(Malichae);

            GoToUseIncapacitatedAbilityPhase(Malichae);

            DecisionSelectTurnTaker = ra.TurnTaker;
            DecisionSelectCards = new Card[] { mdp };

            QuickHPStorage(mdp);
            UseIncapacitatedAbility(Malichae, 0);

            QuickHPCheck(-2);
        }

        [Test()]
        public void TestMinistryOfStrategicScienceMalichaeIncap2()
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae/MinistryOfStrategicScienceMalichaeCharacter", "Ra", "Haka", "Megalopolis");
            StartGame();

            SetupIncap(Malichae);

            SetHitPoints(ra.CharacterCard, 20);
            SetHitPoints(haka.CharacterCard, 20);
            SetHitPoints(baron.CharacterCard, 20);

            GoToUseIncapacitatedAbilityPhase(Malichae);

            QuickHPStorage(baron, ra, haka);
            DecisionSelectCards = new[] { ra.CharacterCard, haka.CharacterCard };
            UseIncapacitatedAbility(Malichae, 1);

            QuickHPCheck(0, 1, 1);
        }

        [Test()]
        public void TestMinistryOfStrategicScienceMalichaeIncap3()
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae/MinistryOfStrategicScienceMalichaeCharacter", "Ra", "Haka", "Megalopolis");
            StartGame();

            SetupIncap(Malichae);

            GoToUseIncapacitatedAbilityPhase(Malichae);

            AssertNumberOfStatusEffectsInPlay(0);
            UseIncapacitatedAbility(Malichae, 2);
            AssertNumberOfStatusEffectsInPlay(1);

            QuickHPStorage(ra.CharacterCard, haka.CharacterCard);
            DealDamage(baron, ra, 1, DamageType.Cold);
            QuickHPCheck(0, 0);

            QuickHPStorage(ra.CharacterCard, haka.CharacterCard);
            DealDamage(baron, haka, 2, DamageType.Cold);
            QuickHPCheck(0, -2);
        }

    }
}
