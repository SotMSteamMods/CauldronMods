using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;

namespace Cauldron.Titan
{
    public class UnbreakableCardController : CardController
    {
        public UnbreakableCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        //Skip any effects which would act at the end of the villain turn.
        //You may not use Powers. You may not draw cards.
        //At the start of your turn, destroy this card.
    }
}