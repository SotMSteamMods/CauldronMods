using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.SwarmEater
{
    public abstract class AugCardController : CardController
    {
        protected AugCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
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