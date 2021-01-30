using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Cauldron.Pyre;

namespace CauldronTests
{
    [TestFixture()]
    public class PyreVariantTests : CauldronBaseTest
    {
        #region PyreHelperFunctions
        private void SetupIncap(TurnTakerController villain)
        {
            SetHitPoints(pyre.CharacterCard, 1);
            DealDamage(villain, pyre, 2, DamageType.Melee);
        }

        protected DamageType DTM => DamageType.Melee;

        protected Card MDP { get { return FindCardInPlay("MobileDefensePlatform"); } }

        protected bool IsIrradiated(Card card)
        {
            var result = card != null && card.NextToLocation.Cards.Any((Card c) => c.Identifier == "IrradiatedMarker");
            if (result && !card.Location.IsHand)
            {
                Assert.Fail($"{card.Title} is irradiated, but is not in a hand!");
            }
            return result;
        }
        protected void AssertIrradiated(Card card)
        {
            Assert.IsTrue(IsIrradiated(card), $"{card.Title} should have been irradiated, but it was not.");
        }
        protected void AssertNotIrradiated(Card card)
        {
            Assert.IsFalse(IsIrradiated(card), $"{card.Title} was irradiated, but it should not be.");
        }
        protected void RemoveCascadeFromGame()
        {
            MoveCards(pyre, new string[] { "RogueFissionCascade", "RogueFissionCascade" }, pyre.TurnTaker.OutOfGame);
        }
        #endregion PyreHelperFunctions
        [Test]
        public void TestExpeditionOblaskPyreLoads()
        {
            SetupGameController("BaronBlade", "Cauldron.Pyre/ExpeditionOblaskPyreCharacter", "Megalopolis");

            // Assert
            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());
            Assert.IsNotNull(pyre);
            Assert.IsInstanceOf(typeof(ExpeditionOblaskPyreCharacterCardController), pyre.CharacterCardController);

            Assert.AreEqual(30, pyre.CharacterCard.HitPoints);
        }
        [Test]
        public void TestUnstablePyreLoads()
        {
            SetupGameController("BaronBlade", "Cauldron.Pyre/UnstablePyreCharacter", "Megalopolis");

            // Assert
            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());
            Assert.IsNotNull(pyre);
            Assert.IsInstanceOf(typeof(UnstablePyreCharacterCardController), pyre.CharacterCardController);

            Assert.AreEqual(33, pyre.CharacterCard.HitPoints);
        }
        [Test]
        public void TestWastelandRoninPyreLoads()
        {
            SetupGameController("BaronBlade", "Cauldron.Pyre/WastelandRoninPyreCharacter", "Megalopolis");

            // Assert
            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());
            Assert.IsNotNull(pyre);
            Assert.IsInstanceOf(typeof(WastelandRoninPyreCharacterCardController), pyre.CharacterCardController);

            Assert.AreEqual(28, pyre.CharacterCard.HitPoints);
        }
    }
}
