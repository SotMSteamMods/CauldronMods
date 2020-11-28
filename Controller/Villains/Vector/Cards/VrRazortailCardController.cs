
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace Cauldron.Vector
{
    public class VrRazortailCardController : CardController
    {
        //==============================================================
        //
        //==============================================================

        public static readonly string Identifier = "VrRazortail";

        public VrRazortailCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

    }
}