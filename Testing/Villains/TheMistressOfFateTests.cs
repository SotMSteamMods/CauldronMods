using NUnit.Framework;
using System;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.Engine.Controller;
using System.Linq;
using System.Collections;
using Handelabra.Sentinels.UnitTest;
using System.Reflection;
using Handelabra;
using System.Collections.Generic;
using Cauldron.TheMistressOfFate;

namespace CauldronTests
{
    class TheMistressOfFateTests : BaseTest
    {
        #region FateHelperFunctions
        protected TurnTakerController fate { get { return FindVillain("TheMistressOfFate"); } }
        private void AssertHasKeyword(string keyword, IEnumerable<string> identifiers)
        {
            foreach (var id in identifiers)
            {
                var card = GetCard(id);
                AssertCardHasKeyword(card, keyword, false);
            }
        }
        #endregion
        [Test]
        public void TestMistressOfFateLoads()
        {
            SetupGameController("Cauldron.TheMistressOfFate", "Legacy", "Megalopolis");

            Assert.AreEqual(3, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(fate);
            Assert.IsInstanceOf(typeof(TheMistressOfFateCharacterCardController), fate.CharacterCardController);

            Assert.AreEqual(88, fate.CharacterCard.HitPoints);
        }
    }
}
