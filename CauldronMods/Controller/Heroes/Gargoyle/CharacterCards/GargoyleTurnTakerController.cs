using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Gargoyle
{
    public class GargoyleTurnTakerController : HeroTurnTakerController
    {
        public GargoyleTurnTakerController(TurnTaker turnTaker, GameController gameController) : base(turnTaker, gameController)
        {

        }

        public string[] availablePromos = new string[] { "WastelandRoninGargoyle" };

        public bool ArePromosSetup { get; set; } = false;

    }
}