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
    }
}