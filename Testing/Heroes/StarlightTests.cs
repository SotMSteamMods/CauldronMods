using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using Cauldron.Starlight;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CauldronTests
{
    [TestFixture()]
    public class StarlightTests : BaseTest
    {
        #region StarlightHelperFunctions
        protected HeroTurnTakerController starlight { get { return FindHero("Starlight"); } }
        private void SetupIncap(TurnTakerController villain)
        {
            SetHitPoints(starlight.CharacterCard, 1);
            DealDamage(villain, starlight, 2, DamageType.Melee);
        }
        #endregion

        [Test()]
        public void TestStarlightLoads()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(starlight);
            Assert.IsInstanceOf(typeof(StarlightCharacterCardController), starlight.CharacterCardController);

            Assert.AreEqual(31, starlight.CharacterCard.HitPoints);
        }
        [Test()]
        public void TestStarlightPowerMayDrawCards()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Megalopolis");
            StartGame();

            PutInTrash("AncientConstellationA");

            DecisionSelectFunction = 0;

            QuickHandStorage(starlight);
            UsePower(starlight);
            QuickHandCheck(1);
            AssertNumberOfCardsInTrash(starlight, 1);
        }

        [Test()]
        public void TestStarlightPowerMayPlayTrashConstellation()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Megalopolis");
            StartGame();

            PutInTrash("AncientConstellationA");

            DecisionSelectFunction = 1;

            QuickHandStorage(starlight);
            UsePower(starlight);
            QuickHandCheck(0);
            AssertNumberOfCardsInTrash(starlight, 0);
        }
        [Test()]
        public void TestStarlightPowerPlaysOnlyOneConstellation()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Megalopolis");
            StartGame();

            PutInTrash("AncientConstellationA", "AncientConstellationB", "NightloreArmor");

            DecisionSelectFunction = 1;

            QuickHandStorage(starlight);
            AssertNumberOfCardsInTrash(starlight, 3);
            UsePower(starlight);
            QuickHandCheck(0);
            AssertNumberOfCardsInTrash(starlight, 2);
        }

        [Test()]
        public void TestStarlightPowerMustDrawWithNoConstellationsInTrash()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Megalopolis");
            StartGame();

            PutInTrash("NightloreArmor");

            //if we get a decision, we will try to play from trash
            DecisionSelectFunction = 1;

            QuickHandStorage(starlight);
            UsePower(starlight);
            QuickHandCheck(1);
        }
        [Test()]
        public void TestStarlightPowerMustDrawWhenNotAbleToPlayCards()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Megalopolis");
            StartGame();

            PutInTrash("NightloreArmor", "AncientConstellationA");
            PutIntoPlay("HostageSituation");

            //if we get a decision, we will try to play from trash
            DecisionSelectFunction = 1;

            QuickHandStorage(starlight);
            UsePower(starlight);
            QuickHandCheck(1);
        }

        [Test()]
        public void TestStarlightPowerMustPlayTrashConstellationWhenNotAbleToDraw()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Megalopolis");
            StartGame();

            PutInTrash("AncientConstellationB");
            PutIntoPlay("TrafficPileup");

            //if we get a decision we will try to draw
            DecisionSelectFunction = 0;

            AssertNumberOfCardsInTrash(starlight, 1);
            UsePower(starlight);
            AssertNumberOfCardsInTrash(starlight, 0);
            AssertNumberOfCardsInPlay(starlight, 2);
        }
        [Test()]
        public void TestStarlightIncap1()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Haka", "Ra", "Megalopolis");
            StartGame();

            SetupIncap(baron);
            AssertIncapacitated(starlight);
            Card mdp = GetCardInPlay("MobileDefensePlatform");

            GoToUseIncapacitatedAbilityPhase(starlight);
            UseIncapacitatedAbility(starlight, 0);

            //as lowest HP target, Mobile Defense Platform should be immune to damage
            QuickHPStorage(mdp);
            DealDamage(haka, mdp, 2, DamageType.Melee);
            QuickHPCheck(0);

            //But it should not be able to deal damage either.
            QuickHPStorage(haka);
            DealDamage(mdp, haka, 2, DamageType.Melee);
            QuickHPCheck(0);

            //Other targets should be unaffected
            DealDamage(ra, haka, 2, DamageType.Fire);
            QuickHPCheck(-2);

            //Should change which target is affected when lowest HP changes
            PutIntoPlay("BladeBattalion");
            Card battalion = GetCardInPlay("BladeBattalion");
            QuickHPStorage(mdp, battalion);
            DealDamage(haka, mdp, 2, DamageType.Melee);
            DealDamage(haka, battalion, 2, DamageType.Melee);
            QuickHPCheck(-2, 0);

            //Should wear off at start of Starlight's turn
            GoToStartOfTurn(starlight);
            QuickHPStorage(battalion);
            DealDamage(haka, battalion, 2, DamageType.Melee);
            QuickHPCheck(-2);
        }
        [Test()]
        public void TestStarlightIncap2()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Haka", "Ra", "Megalopolis");
            StartGame();

            SetupIncap(baron);
            AssertIncapacitated(starlight);
            Card mdp = GetCardInPlay("MobileDefensePlatform");

            //one hero may use a power - have Haka punch the MDP

            GoToUseIncapacitatedAbilityPhase(starlight);
            QuickHPStorage(mdp);
            DecisionSelectTurnTaker = haka.TurnTaker;
            DecisionSelectTarget = mdp;
            AssertIncapLetsHeroUsePower(starlight, 1, haka);
            QuickHPCheck(-2);

        }
        [Test()]
        public void TestStarlightIncap3()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Haka", "Ra", "TheVisionary", "Megalopolis");
            StartGame();

            SetupIncap(baron);
            AssertIncapacitated(starlight);
            PutIntoPlay("DecoyProjection");
            Card mdp = GetCardInPlay("MobileDefensePlatform");
            Card decoy = GetCardInPlay("DecoyProjection");

            //give some room for healing
            SetHitPoints(ra, 10);
            SetHitPoints(haka, 10);
            SetHitPoints(mdp, 5);
            SetHitPoints(decoy, 1);

            //should be able to heal both hero characters and non-characters
            DecisionSelectCards = new Card[] { ra.CharacterCard, decoy };
            QuickHPStorage(ra.CharacterCard, decoy);
            UseIncapacitatedAbility(starlight, 2);
            UseIncapacitatedAbility(starlight, 2);
            QuickHPCheck(2, 2);

            //should not be able to heal villain targets - only options are Ra, Haka, Decoy, and Visionary
            DecisionSelectCards = null;
            AssertNumberOfChoicesInNextDecision(4);
            UseIncapacitatedAbility(starlight, 2);
        }


        [Test()]
        public void TestAncientConstellationPlaysNextToTarget()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Haka", "Ra", "TheVisionary", "Megalopolis");
            StartGame();
            Card mdp = GetCardInPlay("MobileDefensePlatform");

            Card consta = GetCard("AncientConstellationA");
            Card constb = GetCard("AncientConstellationB");
            Card constc = GetCard("AncientConstellationC");

            DecisionSelectCards = new Card[] { baron.CharacterCard, haka.CharacterCard, mdp };
            PlayCards(new Card[] { consta, constb, constc });

            //should be able to play constellations next to targets whether hero or villain, character or not
            AssertNextToCard(consta, baron.CharacterCard);
            AssertNextToCard(constb, haka.CharacterCard);
            AssertNextToCard(constc, mdp);
        }

        [Test()]
        public void TestAncientConstellationDestroySelfTrigger()
        {
            SetupGameController("BaronBlade", "Cauldron.Starlight", "Haka", "Ra", "TheVisionary", "Megalopolis");
            StartGame();
            Card mdp = GetCardInPlay("MobileDefensePlatform");

            Card consta = GetCard("AncientConstellationA");
            Card constb = GetCard("AncientConstellationB");
            Card constc = GetCard("AncientConstellationC");

            Card decoy = GetCard("DecoyProjection");
            PlayCard(decoy);

            DecisionSelectCards = new Card[] { mdp, haka.CharacterCard, decoy };
            PlayCards(new Card[] { consta, constb, constc });

            //constellations should self-destroy when target by them leaves play
            //whether through destruction-into-trash
            DealDamage(haka, mdp, 12, DamageType.Melee);
            AssertInTrash(consta);
            //character card flipping
            DealDamage(baron, haka, 50, DamageType.Melee);
            AssertInTrash(constb);
            //or simply going somewhere else
            PutInHand(decoy);
            AssertInTrash(constc);
        }
    }
}