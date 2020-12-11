using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.TheKnight
{
    public class FairTheKnightCharacterCardController : TheKnightUtilityCharacterCardController
    {
        public FairTheKnightCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //"{TheKnight} and all her targets regain 2 HP."
            yield break;
        }
        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator coroutine;
            switch (index)
            {
                case 0:
                    {
                        //"One player may play a card now.",
                        yield break;
                    }
                case 1:
                    {
                        //"One player may draw a card now.",
                        break;
                    }
                case 2:
                    {
                        //"Until the start of your next turn, reduce damage dealt by environment cards to hero targets by 2."
                        break;
                    }
            }
            yield break;
        }
    }
}