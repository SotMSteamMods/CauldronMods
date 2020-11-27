
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace Cauldron.Vector
{
    public class DelayedSymptomsCardController : CardController
    {
        //==============================================================
        //
        //==============================================================

        public static string Identifier = "DelayedSymptoms";

        public DelayedSymptomsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

    }
}