using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.TheKnight
{
    public class BerzerkerTheKnightCharacterCardController : TheKnightUtilityCharacterCardController
    {
        public BerzerkerTheKnightCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //"Destroy an equipment target. {TheKnight} deals 1 target X psychic damage, where X is the HP of the equipment before it was destroyed by 1."
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
                        //"Increase all damage by 1 until the start of your next turn.",
                        break;
                    }
                case 2:
                    {
                        //"One target deals itself 1 psychic damage."
                        break;
                    }
            }
            yield break;
        }
    }
}