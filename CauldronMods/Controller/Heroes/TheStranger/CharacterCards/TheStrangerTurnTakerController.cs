using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheStranger
{
    public class TheStrangerTurnTakerController : HeroTurnTakerController
    {
        public TheStrangerTurnTakerController(TurnTaker turnTaker, GameController gameController) : base(turnTaker, gameController)
        {

        }

        public string[] availablePromos = new string[] { "WastelandRoninTheStranger" };

        public bool ArePromosSetup { get; set; } = false;

    }
}