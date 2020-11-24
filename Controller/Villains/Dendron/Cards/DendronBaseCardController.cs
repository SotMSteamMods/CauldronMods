using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Dendron
{
    public abstract class DendronBaseCardController : CardController
    {
        protected DendronBaseCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        protected bool IsTattoo(Card card)
        {
            return card != null && base.GameController.DoesCardContainKeyword(card, "tattoo");
        }
    }
}