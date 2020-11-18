using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Dendron
{
    public class DendronBaseCardController : CardController
    {

        protected int TattooCardsInDeck = 15;

        public DendronBaseCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        protected bool IsTattoo(Card card)
        {
            return card != null && base.GameController.DoesCardContainKeyword(card, "tattoo");
        }
    }
}