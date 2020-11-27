using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.MagnificentMara
{
    public class AbracadabraCardController : CardController
    {
        public AbracadabraCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //"When a non-character card belonging to another hero is destroyed, you may return it to that player's hand. If you do, destroy this card.",
            //"When this card is destroyed, one player may play a card."
        }
    }
}