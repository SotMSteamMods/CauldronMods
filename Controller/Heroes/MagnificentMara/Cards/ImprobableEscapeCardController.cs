using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.MagnificentMara
{
    public class ImprobableEscapeCardController : CardController
    {
        public ImprobableEscapeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //"When a hero character is reduced to 0 or fewer HP, restore them to 2HP, then destroy this card.",
            //"When this card or any of your ongoing cards are destroyed you may draw a card."
        }
    }
}