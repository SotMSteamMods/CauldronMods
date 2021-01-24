using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Pyre
{
    public class ExpeditionOblaskPyreCharacterCardController : PyreUtilityCharacterCardController
    {
        public ExpeditionOblaskPyreCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
        public override IEnumerator UsePower(int index = 0)
        {
            //"Select 3 Non-{PyreIrradiate} cards among all players' hands and {PyreIrradiate} them until they leave those players' hands."
            yield break;
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator coroutine;
            switch (index)
            {
                case 0:
                    {
                        //"One player may discard a {PyreIrradiate} card. If they do, each player may play a card now.",
                        break;
                    }
                case 1:
                    {
                        //"Destroy 2 environment cards.",
                        break;
                    }
                case 2:
                    {
                        //"One player may draw a card now."
                        break;
                    }
            }
            yield break;
        }
    }
}
