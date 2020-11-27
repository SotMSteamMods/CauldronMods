using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.MagnificentMara
{
    public class WandOfBanishmentCardController : CardController
    {
        public WandOfBanishmentCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //"When a non-character card from another deck would be destroyed, you may put it on the top or bottom of its deck instead. If you do, destroy this card.",

        }
    }
}