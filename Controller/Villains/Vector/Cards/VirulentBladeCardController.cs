
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace Cauldron.Vector
{
    public class VirulentBladeCardController : CardController
    {
        //==============================================================
        //
        //==============================================================

        public static readonly string Identifier = "VirulentBlade";

        public VirulentBladeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

    }
}