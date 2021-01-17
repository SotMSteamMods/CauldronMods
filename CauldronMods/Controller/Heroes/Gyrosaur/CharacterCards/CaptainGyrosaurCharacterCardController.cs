using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Gyrosaur
{
    public class CaptainGyrosaurCharacterCardController : GyrosaurUtilityCharacterCardController
    {
        public CaptainGyrosaurCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
        public override IEnumerator UsePower(int index = 0)
        {
            //"Put the top card of your deck beneath {Gyrosaur}, face up. When she deals damage, play or draw it."
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
                        //"One hero may use a power now.",
                        break;
                    }
                case 2:
                    {
                        //"Select a target. Increase the next damage dealt to it by 2."
                        break;
                    }
            }
            yield break;
        }
    }
}
