
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace Cauldron.Vector
{
    public class QuarantineProtocolsCardController : CardController
    {
        //==============================================================
        //
        //==============================================================

        public static string Identifier = "QuarantineProtocols";

        public QuarantineProtocolsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

    }
}