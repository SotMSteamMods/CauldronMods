using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Echelon
{
    public class EchelonTurnTakerController : HeroTurnTakerController
    {
        public EchelonTurnTakerController(TurnTaker turnTaker, GameController gameController) : base(turnTaker, gameController)
        {

        }

        public string[] availablePromos = new string[] { "FirstResponseEchelon" };

        public bool ArePromosSetup { get; set; } = false;

    }
}