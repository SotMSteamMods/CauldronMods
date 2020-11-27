
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace Cauldron.Vector
{
    public class HostageShieldCardController : CardController
    {
        //==============================================================
        //
        //==============================================================

        public static string Identifier = "HostageShield";

        public HostageShieldCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

    }
}