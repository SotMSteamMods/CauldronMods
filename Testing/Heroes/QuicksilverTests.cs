using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cauldron.Quicksilver;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;

namespace CauldronTests
{
    class QuicksilverTests : BaseTest
    {
        [Test()]
        public void TestComboChainPreventDamage()
        {
            SetupGameController("BaronBlade", "Cualdron.Quicksilver", "Legacy", "Ra", "Magmaria");
            StartGame();
            PlayCard("ComboChain");
            PlayCard("AlloyStorm");
        }
    }
}
