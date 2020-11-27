using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.MagnificentMara
{
    public class PostHypnoticCueCardController : CardController
    {
        public PostHypnoticCueCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //"Play this card next to a hero. When this card is destroyed, that hero may use a power.",
            //"At the start of your turn you may destroy this card."
        }
    }
}