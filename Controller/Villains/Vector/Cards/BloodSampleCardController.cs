
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace Cauldron.Vector
{
    public class BloodSampleCardController : CardController
    {
        //==============================================================
        //
        //==============================================================

        public static string Identifier = "BloodSample";

        public BloodSampleCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

    }
}