using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.TheKnight
{
    public class PastTheKnightCharacterCardController : TheKnightUtilityCharacterCardController
    {
        public PastTheKnightCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //"Play an equipment target next to a hero. Treat {TheKnight}'s name on that equipment as the name of the hero it is next to."

            yield break;
        }
        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator coroutine;
            switch (index)
            {
                case 0:
                    {
                        
                        //"One player may draw a card now.",
				
				
                        yield break;
                    }
                case 1:
                    {
                        //"Reveal the top card of the villain deck, then replace it.",
                        break;
                    }
                case 2:
                    {
                        //"Select a target, increase the next damage dealt to and by that target by 2."
                        break;
                    }
            }
            yield break;
        }
    }
}