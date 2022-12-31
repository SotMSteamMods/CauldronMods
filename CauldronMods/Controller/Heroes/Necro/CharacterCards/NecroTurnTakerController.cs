using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Necro
{
    public class NecroTurnTakerController : HeroTurnTakerController
    {
        public NecroTurnTakerController(TurnTaker turnTaker, GameController gameController) : base(turnTaker, gameController)
        {

        }

        public string[] availablePromos = new string[] { "LastOfTheForgottenOrderNecro" };

        public bool ArePromosSetup { get; set; } = false;

    }
}