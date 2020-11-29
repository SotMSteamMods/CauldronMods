using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.SwarmEater
{
    public abstract class AugCardController : CardController
    {
        public AugCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public Card CardThatAbsorbedThis()
        {
            Card nextTo = this.Card.Location.OwnerCard;
            if (nextTo != null && nextTo.Identifier == "AbsorbedNanites")
            {
                nextTo = base.CharacterCard;
            }
            return nextTo;
        }
    }
}