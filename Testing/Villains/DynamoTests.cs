﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.UnitTest;

using Cauldron.Dynamo;

using NUnit.Framework;

namespace CauldronTests
{
    [TestFixture]
    public class DynamoTests : CauldronBaseTest
    {
        protected const string BankHeist = "BankHeist";
        protected const string CatharticDemolition = "CatharticDemolition";
        protected const string Copperhead = "Copperhead";
        protected const string CrimeSpree = "CrimeSpree";
        protected const string EnergyConversion = "EnergyConversion";
        protected const string HardenedCriminals = "HardenedCriminals";
        protected const string HelmetedCharge = "HelmetedCharge";
        protected const string HeresThePlan = "HeresThePlan";
        protected const string ImperviousAdvance = "ImperviousAdvance";
        protected const string KineticEnergyBeam = "KineticEnergyBeam";
        protected const string Python = "Python";
        protected const string SlipperyThief = "SlipperyThief";
        protected const string Stranglehold = "Stranglehold";
        protected const string TakeItOutside = "TakeItOutside";
        protected const string WantonDestruction = "WantonDestruction";

        [Test()]
        public void TestDynamo_Load()
        {
            SetupGameController("Cauldron.Dynamo", "Haka", "Bunker", "TheScholar", "Megalopolis");

            Assert.AreEqual(5, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(dynamo);
            Assert.IsInstanceOf(typeof(DynamoCharacterCardController), dynamo.CharacterCardController);

            foreach (Card card in dynamo.TurnTaker.GetAllCards())
            {
                CardController cc = GetCardController(card);
                Assert.IsTrue(cc.GetType() != typeof(CardController), $"{card.Identifier} is does not have a CardController");
            }

            Assert.AreEqual(60, dynamo.CharacterCard.HitPoints);
        }

        [Test()]
        public void TestDynamo_Decklist()
        {
            SetupGameController("Cauldron.Dynamo", "Haka", "Bunker", "TheScholar", "Megalopolis");

            AssertHasKeyword("one-shot", new string[] { HelmetedCharge, HeresThePlan, ImperviousAdvance, KineticEnergyBeam, SlipperyThief, Stranglehold, TakeItOutside });

            AssertHasKeyword("ongoing", new string[] { BankHeist, CatharticDemolition, CrimeSpree, EnergyConversion, HardenedCriminals, WantonDestruction });

            AssertHasKeyword("plot", new string[] { BankHeist, CrimeSpree, WantonDestruction });

            AssertHasKeyword("rogue", new string[] { Copperhead, Python });

            AssertMaximumHitPoints(GetCard(Copperhead), 18);
            AssertMaximumHitPoints(GetCard(Python), 10);
        }
    }
}
