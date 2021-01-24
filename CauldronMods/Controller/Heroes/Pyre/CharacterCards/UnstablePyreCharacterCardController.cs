using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Pyre
{
    public class UnstablePyreCharacterCardController : PyreUtilityCharacterCardController
    {
        public UnstablePyreCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
        public override IEnumerator UsePower(int index = 0)
        {
            //"{PyreIrradiate} 1 non-{PyreIrradiate} card in a player's hand until it leaves their hand. Play an equipment card. Shuffle a cascade card from your trash into your deck."
            yield break;
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator coroutine;
            switch (index)
            {
                case 0:
                    {
                        //"One player may discard a {PyreIrradiate} card. If they do, 1 target regains 4 HP.",
                        break;
                    }
                case 1:
                    {
                        //"Select a target. That target is immune to damage during the next environment turn.",
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
