using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cauldron.Echelon;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;

namespace CauldronTests
{
    [TestFixture]
    public class EchelonVariantTests : BaseTest
    {
        #region echelonhelperfunctions
        protected HeroTurnTakerController echelon => FindHero("Echelon");

        private const string DeckNamespace = "Cauldron.Echelon";

        private readonly DamageType DTM = DamageType.Melee;
        private Card MDP => GetCardInPlay("MobileDefensePlatform");

        private void AssertHasKeyword(string keyword, IEnumerable<string> identifiers)
        {
            foreach (var id in identifiers)
            {
                AssertCardHasKeyword(GetCard(id), keyword, false);
            }
        }
        private void AddImmuneToNextDamageEffect(TurnTakerController ttc, bool villains, bool heroes)
        {
            ImmuneToDamageStatusEffect effect = new ImmuneToDamageStatusEffect();
            effect.TargetCriteria.IsVillain = villains;
            effect.TargetCriteria.IsHero = heroes;
            effect.NumberOfUses = 1;
            RunCoroutine(GameController.AddStatusEffect(effect, true, ttc.CharacterCardController.GetCardSource()));
        }

        #endregion

        [Test]
        public void TestFirstResponseEchelonLoads()
        {
            // Arrange & Act
            SetupGameController("BaronBlade", "Cauldron.Echelon/FirstResponseEchelonCharacter", "Megalopolis");

            // Assert
            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());
            Assert.IsNotNull(echelon);
            Assert.IsInstanceOf(typeof(FirstResponseEchelonCharacterCardController), echelon.CharacterCardController);

            Assert.AreEqual(28, echelon.CharacterCard.HitPoints);
        }
        [Test]
        public void TestFutureEchelonLoads()
        {
            // Arrange & Act
            SetupGameController("BaronBlade", "Cauldron.Echelon/FutureEchelonCharacter", "Megalopolis");

            // Assert
            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());
            Assert.IsNotNull(echelon);
            Assert.IsInstanceOf(typeof(FutureEchelonCharacterCardController), echelon.CharacterCardController);

            Assert.AreEqual(26, echelon.CharacterCard.HitPoints);
        }
    }
}