
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace Cauldron.Vector
{
    public class UndiagnosedSubjectCardController : CardController
    {
        //==============================================================
        //
        //==============================================================

        public static string Identifier = "UndiagnosedSubject";

        public UndiagnosedSubjectCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

    }
}