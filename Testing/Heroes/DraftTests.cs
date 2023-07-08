using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

using Cauldron.Draft;

namespace CauldronTests
{
    [TestFixture()]
    public class DraftTests : CauldronBaseTest
    {
        

        [Test()]
        public void TestDraftLoad()
        {
            SetupGameController("BaronBlade", "Cauldron.Draft", "Haka", "Bunker", "TheScholar", "Megalopolis");

            Assert.AreEqual(6, this.GameController.TurnTakerControllers.Count());

            Assert.IsNotNull(draft);
            Assert.IsInstanceOf(typeof(DraftCharacterCardController), draft.CharacterCardController);


            Assert.AreEqual(26, draft.CharacterCard.HitPoints);
        }
    }
}
