using Cauldron.Necro;
using Handelabra.Sentinels.Engine.Model;
using NUnit.Framework;

namespace MyModTest
{
    [SetUpFixture]
    public class Setup
    {
        [OneTimeSetUp]
        public void DoSetup()
        {
            ModHelper.AddAssembly("Cauldron", typeof(NecroCharacterCardController).Assembly);
        }
    }
}
