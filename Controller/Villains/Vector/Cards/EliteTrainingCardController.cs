
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace Cauldron.Vector
{
    public class EliteTrainingCardController : CardController
    {
        //==============================================================
        //
        //==============================================================

        public static string Identifier = "EliteTraining";

        public EliteTrainingCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

    }
}