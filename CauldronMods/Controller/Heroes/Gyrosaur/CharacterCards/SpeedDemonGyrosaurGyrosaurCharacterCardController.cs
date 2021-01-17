using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Gyrosaur
{
    public class SpeedDemonGyrosaurCharacterCardController : GyrosaurUtilityCharacterCardController
    {
        public SpeedDemonGyrosaurCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
        public override IEnumerator UsePower(int index = 0)
        {
            //"If at least half of the cards in your hand are crash cards, draw a card. If not, play a card."
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
                        break;
                    }
                case 1:
                    {
                        //"One player with fewer than 4 cards in their hand may play 2 cards now.",
                        break;
                    }
                case 2:
                    {
                        //"Reduce the next damage dealt to a hero target by 2."
                        break;
                    }
            }
            yield break;
        }
    }
}
