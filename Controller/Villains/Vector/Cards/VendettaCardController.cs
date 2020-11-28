
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace Cauldron.Vector
{
    public class VendettaCardController : CardController
    {
        //==============================================================
        //
        //==============================================================

        public static readonly string Identifier = "Vendetta";

        public VendettaCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

    }
}