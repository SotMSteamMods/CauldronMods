
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace Cauldron.Vector
{
    public class SupervirusCardController : CardController
    {
        //==============================================================
        //
        //==============================================================

        public static readonly string Identifier = "Supervirus";

        public SupervirusCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

    }
}