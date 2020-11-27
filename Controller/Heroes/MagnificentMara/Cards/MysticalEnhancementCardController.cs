using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.MagnificentMara
{
    public class MysticalEnhancementCardController : CardController
    {
        public MysticalEnhancementCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //"Play this card next to a card with a power on it.",
            //"Increase damage dealt by that power by 1.",
            //"If that card would be destroyed, destroy this card instead."
        }
    }
}