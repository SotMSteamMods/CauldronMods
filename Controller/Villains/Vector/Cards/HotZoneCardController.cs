
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace Cauldron.Vector
{
    public class HotZoneCardController : CardController
    {
        //==============================================================
        //
        //==============================================================

        public static string Identifier = "HotZone";

        public HotZoneCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

    }
}