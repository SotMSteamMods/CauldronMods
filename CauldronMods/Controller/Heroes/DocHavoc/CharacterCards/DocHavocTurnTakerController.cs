using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.DocHavoc
{
    public class DocHavocTurnTakerController : HeroTurnTakerController
    {
        public DocHavocTurnTakerController(TurnTaker turnTaker, GameController gameController) : base(turnTaker, gameController)
        {

        }

        public string[] availablePromos = new string[] { "FirstResponseDocHavoc" };

        public bool ArePromosSetup { get; set; } = false;

    }
}