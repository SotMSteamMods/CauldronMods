using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Vanish
{
    public class VanishTurnTakerController : HeroTurnTakerController
    {
        public VanishTurnTakerController(TurnTaker turnTaker, GameController gameController) : base(turnTaker, gameController)
        {

        }

        public string[] availablePromos = new string[] { "FirstResponseVanish" };

        public bool ArePromosSetup { get; set; } = false;

    }
}