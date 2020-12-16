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

            var cs = StackDeck(ra, new[] { "ImbuedFire", "FireBlast" }).ToArray();
            AssertInDeck(cs[0]);
            AssertInDeck(cs[1]);

            DecisionSelectTurnTaker = ra.TurnTaker;
            DecisionSelectCards = new[] { cs[0], cs[1] };

            UseIncapacitatedAbility(Malichae, 0);
            AssertInTrash(cs[0]);
            AssertInDeck(cs[1]);
            AssertOnTopOfDeck(cs[1]);

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
        public void TestMinistryOfStrategicScienceMalichaePower()
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae/MinistryOfStrategicScienceMalichaeCharacter", "Ra", "Haka", "Megalopolis");
            StartGame();

            GoToUsePowerPhase(Malichae);

            var top = GetTopCardOfDeck(Malichae);
            var card = PutInHand("Reshiel");
            DecisionSelectCard = card;

            QuickHandStorage(Malichae);
            UsePower(Malichae.CharacterCard);
            QuickHandCheck(0);
            AssertInTrash(card);
            AssertInHand(top);
        }


        [Test]
        public void Djinn_HighReshiel_UsePower()
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae/MinistryOfStrategicScienceMalichaeCharacter", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            GoToUsePowerPhase(Malichae);

            var card = PutInHand("HighReshiel");

            DecisionSelectCards = new Card[] { card, ra.CharacterCard, fanatic.CharacterCard, Malichae.CharacterCard };
            QuickHPStorage(baron.CharacterCard, Malichae.CharacterCard, ra.CharacterCard, fanatic.CharacterCard);
            UsePower(Malichae.CharacterCard);

            QuickHPCheck(0, -2, -2, -2); //2
            AssertInTrash(Malichae, card);
        }

        public void Djinn_GrandBathiel_UsePower()
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae/MinistryOfStrategicScienceMalichaeCharacter", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            GoToUsePowerPhase(Malichae);

            var card = PutInHand("GrandReshiel");

            DecisionSelectCards = new Card[] { card, ra.CharacterCard };
            QuickHPStorage(baron.CharacterCard, Malichae.CharacterCard, ra.CharacterCard, fanatic.CharacterCard, card);
            UsePower(Malichae.CharacterCard);

            QuickHPCheck(0, 0, -6, 0, 0); //6
            AssertInTrash(Malichae, card);
        }


        [Test]
        public void Djinn_GrandEzael_UsePower()
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae/MinistryOfStrategicScienceMalichaeCharacter", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            SetHitPoints(ra.CharacterCard, 20);
            SetHitPoints(baron.CharacterCard, 20);
            SetHitPoints(fanatic.CharacterCard, 20);
            SetHitPoints(Malichae.CharacterCard, 20);

            GoToUsePowerPhase(Malichae);
            var card = PutInHand("GrandEzael");
            DecisionSelectCards = new Card[] { card };

            QuickHPStorage(baron.CharacterCard, Malichae.CharacterCard, ra.CharacterCard, fanatic.CharacterCard);
            UsePower(Malichae.CharacterCard);

            QuickHPCheck(0, 3, 3, 3);
            AssertInTrash(Malichae, card);
        }

        [Test]
        public void Djinn_GrandReshiel_UsePower()
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae/MinistryOfStrategicScienceMalichaeCharacter", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            var mdp = GetCardInPlay("MobileDefensePlatform");
            var blade = PlayCard("BladeBattalion");

            GoToUsePowerPhase(Malichae);
            var card = PutInHand("GrandReshiel");

            DecisionSelectCards = new Card[] { card };
            DecisionAutoDecideIfAble = true;
            QuickHPStorage(baron.CharacterCard, Malichae.CharacterCard, ra.CharacterCard, fanatic.CharacterCard, blade, mdp);
            UsePower(Malichae.CharacterCard);

            QuickHPCheck(0, 0, 0, 0, -2, -2); //2 + 1
            AssertInTrash(Malichae, card);
        }

        [Test]
        public void Djinn_GrandSomael_UsePower()
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae/MinistryOfStrategicScienceMalichaeCharacter", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            var mdp = GetCardInPlay("MobileDefensePlatform");
            var blade = PlayCard("BladeBattalion");

            GoToUsePowerPhase(Malichae);
            var card = PutInHand("GrandSomael");

            DecisionSelectCards = new Card[] { card };

            DecisionAutoDecideIfAble = true;
            UsePower(Malichae.CharacterCard);

            QuickHPStorage(baron.CharacterCard, Malichae.CharacterCard, ra.CharacterCard, fanatic.CharacterCard, mdp, blade);
            DealDamage(baron.CharacterCard, Malichae.CharacterCard, 2, DamageType.Cold);
            DealDamage(baron.CharacterCard, ra.CharacterCard, 2, DamageType.Cold);
            DealDamage(baron.CharacterCard, fanatic.CharacterCard, 2, DamageType.Cold);
            DealDamage(baron.CharacterCard, mdp, 2, DamageType.Cold);
            DealDamage(baron.CharacterCard, blade, 2, DamageType.Cold);
            QuickHPCheck(0, 0, 0, 0, -2, -2); //damage reduced to zero except for villain
            SetHitPoints(blade, 5);

            QuickHPStorage(baron.CharacterCard, Malichae.CharacterCard, ra.CharacterCard, fanatic.CharacterCard, mdp, blade);
            DealDamage(baron.CharacterCard, Malichae.CharacterCard, 2, DamageType.Cold, true);
            DealDamage(baron.CharacterCard, ra.CharacterCard, 2, DamageType.Cold, true);
            DealDamage(baron.CharacterCard, fanatic.CharacterCard, 2, DamageType.Cold, true);
            DealDamage(baron.CharacterCard, mdp, 2, DamageType.Cold, true);
            DealDamage(baron.CharacterCard, blade, 2, DamageType.Cold, true);
            QuickHPCheck(0, -2, -2, -2, -2, -2); //damage not reduced to zero
            SetHitPoints(blade, 5);

            DecisionSelectCard = ra.CharacterCard;

            AssertInTrash(Malichae, card);
        }

        [Test]
        public void Djinn_HighBathiel_UsePower()
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae/MinistryOfStrategicScienceMalichaeCharacter", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            GoToUsePowerPhase(Malichae);
            var card = PutInHand("HighBathiel");

            DecisionSelectCards = new Card[] { card, ra.CharacterCard };
            DecisionAutoDecideIfAble = true;
            QuickHPStorage(baron.CharacterCard, Malichae.CharacterCard, ra.CharacterCard, fanatic.CharacterCard);
            UsePower(Malichae.CharacterCard);

            QuickHPCheck(0, 0, -4, 0); //4
            AssertInTrash(Malichae, card);
        }


        [Test]
        public void Djinn_HighSomael_UsePower()
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae/MinistryOfStrategicScienceMalichaeCharacter", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            var blade = PlayCard("BladeBattalion");

            GoToUsePowerPhase(Malichae);

            var card = PutInHand("HighSomael");

            DecisionSelectCards = new Card[] { card };
            DecisionAutoDecideIfAble = true;

            UsePower(Malichae.CharacterCard);

            QuickHPStorage(baron.CharacterCard, Malichae.CharacterCard, ra.CharacterCard, fanatic.CharacterCard, blade);
            DealDamage(baron.CharacterCard, Malichae.CharacterCard, 1, DamageType.Cold);
            DealDamage(baron.CharacterCard, ra.CharacterCard, 1, DamageType.Cold);
            DealDamage(baron.CharacterCard, fanatic.CharacterCard, 1, DamageType.Cold);
            DealDamage(baron.CharacterCard, card, 1, DamageType.Cold);
            DealDamage(baron.CharacterCard, blade, 1, DamageType.Cold);
            QuickHPCheck(0, 0, 0, 0, -1); //damage reduced to zero except for villain

            QuickHPStorage(baron.CharacterCard, Malichae.CharacterCard, ra.CharacterCard, fanatic.CharacterCard, blade);
            DealDamage(baron.CharacterCard, Malichae.CharacterCard, 1, DamageType.Cold, true);
            DealDamage(baron.CharacterCard, ra.CharacterCard, 1, DamageType.Cold, true);
            DealDamage(baron.CharacterCard, fanatic.CharacterCard, 1, DamageType.Cold, true);
            DealDamage(baron.CharacterCard, card, 1, DamageType.Cold, true);
            DealDamage(baron.CharacterCard, blade, 1, DamageType.Cold, true);
            QuickHPCheck(0, -1, -1, -1, -1); //damage not reduced to zero

            AssertInTrash(Malichae, card);
        }

        [Test]
        public void Djinn_HighEzael_UsePower()
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae/MinistryOfStrategicScienceMalichaeCharacter", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            GoToUsePowerPhase(Malichae);

            SetHitPoints(ra.CharacterCard, 20);
            SetHitPoints(baron.CharacterCard, 20);
            SetHitPoints(fanatic.CharacterCard, 20);
            SetHitPoints(Malichae.CharacterCard, 20);

            var card = PutInHand("HighEzael");

            DecisionSelectCards = new Card[] { card };
            DecisionAutoDecideIfAble = true;
            QuickHPStorage(baron.CharacterCard, Malichae.CharacterCard, ra.CharacterCard, fanatic.CharacterCard);
            UsePower(Malichae.CharacterCard);
            QuickHPCheck(0, 1, 1, 1);

            AssertInTrash(Malichae, card);
        }


        [Test]
        public void SummoningCrystal()
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae/MinistryOfStrategicScienceMalichaeCharacter", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            GoToUsePowerPhase(Malichae);

            var card = PutInHand("SummoningCrystal");
            var target = PutInHand("Bathiel");

            DecisionSelectCards = new Card[] { card, target };
            UsePower(Malichae.CharacterCard);

            AssertInPlayArea(Malichae, target);
            AssertInTrash(Malichae, card);
        }


        [Test]
        public void ZephaerensCompass()
        {
            SetupGameController("BaronBlade", "Cauldron.Malichae/MinistryOfStrategicScienceMalichaeCharacter", "Ra", "Fanatic", "Megalopolis");
            StartGame();

            var ongoing = PlayCard("BacklashField");
            var envCard = PlayCard("PlummetingMonorail");

            GoToUsePowerPhase(Malichae);

            var card = PutInHand("ZephaerensCompass");

            string djinn = "Reshiel";
            var target = PlayCard(djinn);
            DecisionMoveCard = target;
            var high = GetCard("High" + djinn);
            PlayCard(high);
            AssertNextToCard(high, target);

            DecisionSelectCards = new Card[] { card, ongoing, envCard };
            QuickHandStorage(Malichae, ra, fanatic);

            UsePower(Malichae.CharacterCard);
            
            QuickHandCheck(1, 0, 0);
            AssertInTrash(card);
            AssertInTrash(ongoing);
            AssertInTrash(envCard);
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
