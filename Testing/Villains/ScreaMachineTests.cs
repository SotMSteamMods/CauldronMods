using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using Handelabra.Sentinels.UnitTest;
using System.Reflection;
using Cauldron.ScreaMachine;
using Handelabra;
using System.Collections.Generic;

namespace CauldronTests
{
    [TestFixture()]
    public class ScreaMachineTestsTests : BaseTest
    {
        #region ScreaMachineTestsHelperFunctions

        protected TurnTakerController scream { get { return FindVillain("ScreaMachine"); } }

        protected Card slice { get { return FindCardInPlay("SliceCharacter"); } }
        protected Card valentine { get { return FindCardInPlay("ValentineCharacter"); } }
        protected Card bloodlace { get { return FindCardInPlay("BloodlaceCharacter"); } }
        protected Card rickyg { get { return FindCardInPlay("RickyGCharacter"); } }
        protected Card setlist { get { return FindCardInPlay("TheSetList"); } }

        protected void AddImmuneToDamageTrigger(TurnTakerController ttc, bool heroesImmune, bool villainsImmune)
        {
            ImmuneToDamageStatusEffect immuneToDamageStatusEffect = new ImmuneToDamageStatusEffect();
            immuneToDamageStatusEffect.TargetCriteria.IsHero = new bool?(heroesImmune);
            immuneToDamageStatusEffect.TargetCriteria.IsVillain = new bool?(villainsImmune);
            immuneToDamageStatusEffect.UntilStartOfNextTurn(ttc.TurnTaker);
            this.RunCoroutine(this.GameController.AddStatusEffect(immuneToDamageStatusEffect, true, new CardSource(ttc.CharacterCardController)));
        }

        #endregion

        [Test()]
        public void TestScreaMachineLoadedProperly()
        {
            SetupGameController("Cauldron.ScreaMachine", "Legacy", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(scream);
            Assert.IsInstanceOf(typeof(ScreaMachineTurnTakerController), scream);

            Assert.IsNotNull(slice);
            Assert.IsInstanceOf(typeof(SliceCharacterCardController), slice);
            Assert.AreEqual(28, slice.HitPoints);

            Assert.IsNotNull(valentine);
            Assert.IsInstanceOf(typeof(ValentineCharacterCardController), valentine);
            Assert.AreEqual(31, valentine.HitPoints);

            Assert.IsNotNull(bloodlace);
            Assert.IsInstanceOf(typeof(BloodlaceCharacterCardController), bloodlace);
            Assert.AreEqual(26, bloodlace.HitPoints);

            Assert.IsNotNull(rickyg);
            Assert.IsInstanceOf(typeof(RickyGCharacterCardController), rickyg);
            Assert.AreEqual(35, rickyg.HitPoints);

            Assert.IsNotNull(setlist);
            Assert.IsInstanceOf(typeof(TheSetListCardController), setlist);

        }


    }
}
