using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.MagnificentMara
{
    public class MesmerPendantCardController : CardController
    {
        public MesmerPendantCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //"Play this card next to a non-character target. Treat the words 'Hero Target', 'Hero Ongoing', and 'Non-Villain target' on that card as 'Villain Target', 'Villain Ongoing' and 'Non-Hero target' instead.",
            //"At the start of your turn destroy this card."
        }
    }
}