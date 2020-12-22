using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;

namespace Cauldron.Echelon
{
    public class RemoteObservationCardController : TacticBaseCardController
    {
        //==============================================================
        //"At the start of your turn, you may discard a card. If you do not, draw a card and destroy this card.",
        //"During their draw Phase, heroes draw 1 additional card, then discard 1 card."
        //==============================================================

        public static string Identifier = "RemoteObservation";

        public RemoteObservationCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        protected override void AddTacticEffectTrigger()
        {
            //"During their draw Phase, heroes draw 1 additional card, then discard 1 card."
        }

    }
}