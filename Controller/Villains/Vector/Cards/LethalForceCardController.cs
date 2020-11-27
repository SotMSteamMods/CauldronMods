
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace Cauldron.Vector
{
    public class LethalForceCardController : CardController
    {
        //==============================================================
        //
        //==============================================================

        public static string Identifier = "LethalForce";

        public LethalForceCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

    }
}