using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.UnitTest;
using NUnit.Framework;

namespace CauldronTests
{
    [TestFixture()]
    class SwarmEaterTests : BaseTest
    {
        protected TurnTakerController swarm { get { return FindVillain("SwarmEater"); } }

    }
}
