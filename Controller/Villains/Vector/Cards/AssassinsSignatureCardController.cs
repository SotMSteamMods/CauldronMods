
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace Cauldron.Vector
{
    public class AssassinsSignatureCardController : CardController
    {
        //==============================================================
        //
        //==============================================================

        public static string Identifier = "AssassinsSignature";

        public AssassinsSignatureCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

    }
}