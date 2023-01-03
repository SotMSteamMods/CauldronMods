using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Impact
{
    public class ImpactTurnTakerController : HeroTurnTakerController
    {
        public ImpactTurnTakerController(TurnTaker turnTaker, GameController gameController) : base(turnTaker, gameController)
        {

        }

        public string[] availablePromos = new string[] { "WastelandRoninImpact" };

        public bool ArePromosSetup { get; set; } = false;

    }
}