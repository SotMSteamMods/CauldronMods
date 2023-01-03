using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Cypher
{
    public class CypherTurnTakerController : HeroTurnTakerController
    {
        public CypherTurnTakerController(TurnTaker turnTaker, GameController gameController) : base(turnTaker, gameController)
        {

        }

        public string[] availablePromos = new string[] { "FirstResponseCypher", "SwarmingProtocolCypher" };

        public bool ArePromosSetup { get; set; } = false;

    }
}