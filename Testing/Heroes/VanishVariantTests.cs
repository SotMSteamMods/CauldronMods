using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

using Cauldron.Vanish;

namespace CauldronTests
{
    [TestFixture()]
    public class VanishVariantsTests : CauldronBaseTest
    {
        #region HelperFunctions
        private void SetupIncap(TurnTakerController villain)
        {
            SetHitPoints(vanish.CharacterCard, 1);
            DealDamage(villain, vanish, 2, DamageType.Melee);
        }

        #endregion HelperFunctions

        [Test()]
        [Order(0)]
        public void FirstResponseVanishLoad()
        {
            SetupGameController("BaronBlade", "Cauldron.Vanish/FirstResponseVanishCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");

            Assert.AreEqual(6, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(vanish);
            Assert.IsInstanceOf(typeof(FirstResponseVanishCharacterCardController), vanish.CharacterCardController);

            foreach (var card in vanish.HeroTurnTaker.GetAllCards())
            {
                var cc = GetCardController(card);
                Assert.IsTrue(cc.GetType() != typeof(CardController), $"{card.Identifier} is does not have a CardController");
            }

            Assert.AreEqual(27, vanish.CharacterCard.HitPoints);
        }


        [Test]
        public void FirstResponseVanishInnatePower()
        {
            SetupGameController("BaronBlade", "Cauldron.Vanish/FirstResponseVanishCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            DestroyCard("MobileDefensePlatform");
            var minion = PlayCard("BladeBattalion");

            AssertNumberOfStatusEffectsInPlay(0);
            UsePower(vanish);
            AssertNumberOfStatusEffectsInPlay(1);

            QuickHPStorage(baron.CharacterCard, vanish.CharacterCard, haka.CharacterCard, bunker.CharacterCard, scholar.CharacterCard, minion);
            DealDamage(baron, haka, 2, DamageType.Cold);
            QuickHPCheck(0, 0, 0, 0, 0, 0);

            AssertNumberOfStatusEffectsInPlay(0);
        }

        [Test]
        public void FirstResponseVanishIncap1()
        {
            SetupGameController("BaronBlade", "Cauldron.Vanish/FirstResponseVanishCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            DestroyCard("MobileDefensePlatform");
            SetupIncap(baron);
            AssertIncapacitated(vanish);

            GoToUseIncapacitatedAbilityPhase(vanish);
            DecisionSelectTurnTaker = haka.TurnTaker;
            DecisionSelectTarget = baron.CharacterCard;
            QuickHPStorage(baron.CharacterCard, haka.CharacterCard, bunker.CharacterCard, scholar.CharacterCard);
            UseIncapacitatedAbility(vanish, 0);
            QuickHPCheck(-2, 0, 0, 0);

        }

        [Test]
        public void FirstResponseVanishIncap2()
        {
            //This is a bad test, but the power kinda doesn't do anything either.

            SetupGameController("BaronBlade", "Cauldron.Vanish/FirstResponseVanishCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            DestroyCard("MobileDefensePlatform");
            SetupIncap(baron);
            AssertIncapacitated(vanish);

            GoToUseIncapacitatedAbilityPhase(vanish);
            DecisionSelectLocations = new[] { new LocationChoice(baron.TurnTaker.Deck), new LocationChoice(haka.TurnTaker.Deck) };

            UseIncapacitatedAbility(vanish, 1);

            AssertNumberOfCardsInRevealed(baron, 0);
            AssertNumberOfCardsInRevealed(haka, 0);
        }

        [Test]
        public void FirstResponseVanishIncap3()
        {
            SetupGameController("BaronBlade", "Cauldron.Vanish/FirstResponseVanishCharacter", "Ra", "TheWraith", "Megalopolis");
            StartGame();

            DestroyCard("MobileDefensePlatform");

            SetupIncap(baron);
            AssertIncapacitated(vanish);

            GoToUseIncapacitatedAbilityPhase(vanish);

            AssertNumberOfStatusEffectsInPlay(0);
            UseIncapacitatedAbility(vanish, 2);
            AssertNumberOfStatusEffectsInPlay(1);

            QuickHPStorage(baron.CharacterCard, ra.CharacterCard, wraith.CharacterCard);
            DecisionSelectTarget = wraith.CharacterCard;
            DealDamage(baron, ra, 2, DamageType.Infernal);
            QuickHPCheck(0, 0, -2);
            AssertNumberOfStatusEffectsInPlay(0);
        }


        [Test()]
        [Order(0)]
        public void PastVanishLoad()
        {
            SetupGameController("BaronBlade", "Cauldron.Vanish/PastVanishCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");

            Assert.AreEqual(6, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(vanish);
            Assert.IsInstanceOf(typeof(PastVanishCharacterCardController), vanish.CharacterCardController);

            foreach (var card in vanish.HeroTurnTaker.GetAllCards())
            {
                var cc = GetCardController(card);
                Assert.IsTrue(cc.GetType() != typeof(CardController), $"{card.Identifier} is does not have a CardController");
            }

            Assert.AreEqual(27, vanish.CharacterCard.HitPoints);
        }


        [Test]
        public void PastVanishInnatePower()
        {
            SetupGameController("BaronBlade", "Cauldron.Vanish/PastVanishCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            DestroyCard("MobileDefensePlatform");
            var minion = PlayCard("BladeBattalion");

            GoToUsePowerPhase(vanish);

            QuickHandStorage(vanish, haka, bunker);
            AssertNumberOfStatusEffectsInPlay(0);
            UsePower(vanish);
            AssertNumberOfStatusEffectsInPlay(1);
            QuickHandCheck(1, 0, 0);

            QuickHPStorage(baron.CharacterCard, vanish.CharacterCard, haka.CharacterCard, bunker.CharacterCard, scholar.CharacterCard, minion);
            DealDamage(vanish, minion, 1, DamageType.Cold);
            QuickHPCheck(0, 0, 0, 0, 0, -2);

            QuickHPStorage(baron.CharacterCard, vanish.CharacterCard, haka.CharacterCard, bunker.CharacterCard, scholar.CharacterCard, minion);
            DealDamage(haka, minion, 1, DamageType.Cold);
            QuickHPCheck(0, 0, 0, 0, 0, -2);

            GoToStartOfTurn(haka);

            AssertNumberOfStatusEffectsInPlay(0);
        }

        [Test]
        public void PastVanishIncap1()
        {
            SetupGameController("BaronBlade", "Cauldron.Vanish/PastVanishCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            DestroyCard("MobileDefensePlatform");
            SetupIncap(baron);
            AssertIncapacitated(vanish);

            var card = PutInHand("ElbowSmash");

            GoToUseIncapacitatedAbilityPhase(vanish);
            DecisionSelectTurnTaker = haka.TurnTaker;
            DecisionSelectCard = card;
            DecisionSelectTarget = baron.CharacterCard;
            QuickHPStorage(baron.CharacterCard, haka.CharacterCard, bunker.CharacterCard, scholar.CharacterCard);
            UseIncapacitatedAbility(vanish, 0);
            QuickHPCheck(-3, 0, 0, 0);
        }

        [Test]
        public void PastVanishIncap2()
        {
            SetupGameController("BaronBlade", "Cauldron.Vanish/PastVanishCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            DestroyCard("MobileDefensePlatform");
            SetupIncap(baron);
            AssertIncapacitated(vanish);

            GoToUseIncapacitatedAbilityPhase(vanish);
            UseIncapacitatedAbility(vanish, 1);
            AssertNumberOfStatusEffectsInPlay(1);

            var minion = PlayCard("BladeBattalion");
            AssertHitPoints(minion, 4);

            AssertNumberOfStatusEffectsInPlay(0);
        }

        [Test]
        public void PastVanishIncap2Stacks()
        {
            SetupGameController("BaronBlade", "Cauldron.Vanish/PastVanishCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            DestroyCard("MobileDefensePlatform");
            SetupIncap(baron);
            AssertIncapacitated(vanish);

            GoToUseIncapacitatedAbilityPhase(vanish);
            UseIncapacitatedAbility(vanish, 1);
            AssertNumberOfStatusEffectsInPlay(1);
            //test stacks
            UseIncapacitatedAbility(vanish, 1);
            AssertNumberOfStatusEffectsInPlay(2);

            var minion = PlayCard("BladeBattalion");
            AssertHitPoints(minion, 3);

            AssertNumberOfStatusEffectsInPlay(0);
        }

        [Test]
        public void PastVanishIncap3()
        {
            SetupGameController("BaronBlade", "Cauldron.Vanish/PastVanishCharacter", "Ra", "TheWraith", "Megalopolis");
            StartGame();

            DestroyCard("MobileDefensePlatform");

            SetupIncap(baron);
            AssertIncapacitated(vanish);

            var c1 = PlayCard("CrampedQuartersCombat");
            var c2 = PlayCard("HostageSituation");

            GoToUseIncapacitatedAbilityPhase(vanish);

            QuickShuffleStorage(env);
            DecisionSelectCards = new[] { c1, c2 };
            UseIncapacitatedAbility(vanish, 2);

            QuickShuffleCheck(1);

            AssertInDeck(c1);
            AssertInDeck(c2);
        }



        [Test()]
        [Order(0)]
        public void TombOfThievesVanishLoad()
        {
            SetupGameController("BaronBlade", "Cauldron.Vanish/TombOfThievesVanishCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");

            Assert.AreEqual(6, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(vanish);
            Assert.IsInstanceOf(typeof(TombOfThievesVanishCharacterCardController), vanish.CharacterCardController);

            foreach (var card in vanish.HeroTurnTaker.GetAllCards())
            {
                var cc = GetCardController(card);
                Assert.IsTrue(cc.GetType() != typeof(CardController), $"{card.Identifier} is does not have a CardController");
            }

            Assert.AreEqual(26, vanish.CharacterCard.HitPoints);
        }


        [Test]
        public void TombOfThievesVanishInnatePowerStacks()
        {
            SetupGameController("BaronBlade", "Cauldron.Vanish/TombOfThievesVanishCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            DestroyCard("MobileDefensePlatform");
            var minion = PlayCard("BladeBattalion");

            GoToUsePowerPhase(vanish);

            QuickHandStorage(vanish, haka, bunker);
            AssertNumberOfStatusEffectsInPlay(0);
            UsePower(vanish);
            AssertNumberOfStatusEffectsInPlay(1);
            UsePower(vanish);
            AssertNumberOfStatusEffectsInPlay(1);

            DecisionSelectFunction = 1;
                        
            DealDamage(minion, vanish, 1, DamageType.Cold);
            AssertNumberOfStatusEffectsInPlay(0);
            QuickHandCheck(2, 0, 0);
        }


        [Test]
        public void TombOfThievesVanishInnatePowerDraw()
        {
            SetupGameController("BaronBlade", "Cauldron.Vanish/TombOfThievesVanishCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            DestroyCard("MobileDefensePlatform");
            var minion = PlayCard("BladeBattalion");

            GoToUsePowerPhase(vanish);

            QuickHandStorage(vanish, haka, bunker);
            AssertNumberOfStatusEffectsInPlay(0);
            UsePower(vanish);
            AssertNumberOfStatusEffectsInPlay(1);

            DecisionSelectFunction = 1;

            DealDamage(minion, vanish, 1, DamageType.Cold);
            AssertNumberOfStatusEffectsInPlay(0);
            QuickHandCheck(1, 0, 0);
        }

        [Test]
        public void TombOfThievesVanishInnatePowerPlay()
        {
            SetupGameController("BaronBlade", "Cauldron.Vanish/TombOfThievesVanishCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            DestroyCard("MobileDefensePlatform");
            var minion = PlayCard("BladeBattalion");
            var c = PutInHand("JauntingReflex");


            GoToUsePowerPhase(vanish);

            QuickHandStorage(vanish, haka, bunker);
            AssertNumberOfStatusEffectsInPlay(0);
            UsePower(vanish);
            AssertNumberOfStatusEffectsInPlay(1);

            DecisionSelectFunction = 0;
            DecisionSelectCard = c;
            DealDamage(minion, vanish, 1, DamageType.Cold);
            QuickHandCheck(-1, 0, 0);
        }

        [Test]
        public void TombOfThievesVanishInnatePower_DamageZero()
        {
            SetupGameController("BaronBlade", "Cauldron.Vanish/TombOfThievesVanishCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            DestroyCard("MobileDefensePlatform");
            var minion = PlayCard("BladeBattalion");
            var c = PutInHand("JauntingReflex");


            GoToUsePowerPhase(vanish);

            QuickHandStorage(vanish, haka, bunker);
            AssertNumberOfStatusEffectsInPlay(0);
            UsePower(vanish);
            AssertNumberOfStatusEffectsInPlay(1);

            AddReduceDamageTrigger(vanish, true, false, 1);

            DecisionSelectFunction = 0;
            DecisionSelectCard = c;
            DealDamage(minion, vanish, 1, DamageType.Cold);
            QuickHandCheck(0, 0, 0);

            AssertNumberOfStatusEffectsInPlay(1);

            DealDamage(minion, vanish, 2, DamageType.Cold);
            QuickHandCheck(-1, 0, 0);
        }


        [Test]
        public void TombOfThievesVanishIncap1()
        {
            SetupGameController("BaronBlade", "Cauldron.Vanish/TombOfThievesVanishCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            DestroyCard("MobileDefensePlatform");
            SetupIncap(baron);
            AssertIncapacitated(vanish);

            GoToUseIncapacitatedAbilityPhase(vanish);
            DecisionSelectTurnTaker = haka.TurnTaker;
            DecisionSelectTarget = baron.CharacterCard;
            QuickHPStorage(baron.CharacterCard, haka.CharacterCard, bunker.CharacterCard, scholar.CharacterCard);
            UseIncapacitatedAbility(vanish, 0);
            QuickHPCheck(-2, 0, 0, 0);
        }

        [Test]
        public void TombOfThievesVanishIncap2()
        {
            SetupGameController("BaronBlade", "Cauldron.Vanish/TombOfThievesVanishCharacter", "Haka", "Bunker", "TheScholar", "Megalopolis");
            StartGame();

            DestroyCard("MobileDefensePlatform");
            SetupIncap(baron);
            AssertIncapacitated(vanish);

            var c = PlayCard("Dominion");

            GoToUseIncapacitatedAbilityPhase(vanish);
            DecisionSelectCard = c;
            UseIncapacitatedAbility(vanish, 1);
            AssertInTrash(c);
        }

        [Test]
        public void TombOfThievesVanishIncap3()
        {
            SetupGameController("BaronBlade", "Cauldron.Vanish/TombOfThievesVanishCharacter", "Ra", "TheWraith", "Megalopolis");
            StartGame();

            DestroyCard("MobileDefensePlatform");

            SetupIncap(baron);
            AssertIncapacitated(vanish);

            var c1 = PlayCard("PlummetingMonorail");

            GoToUseIncapacitatedAbilityPhase(vanish);
            AssertNumberOfStatusEffectsInPlay(0);
            DecisionSelectCard = c1;
            UseIncapacitatedAbility(vanish, 2);
            AssertNumberOfStatusEffectsInPlay(2);

            QuickHPStorage(c1);
            DealDamage(baron, c1, 99, DamageType.Cold);
            AssertInPlayArea(env, c1);
            QuickHPCheckZero();

            GoToEndOfTurn(baron);
            AssertInPlayArea(env, c1);

            GoToStartOfTurn(vanish);
            AssertInTrash(env, c1);
            AssertNumberOfStatusEffectsInPlay(0);








        }
    }
}
