using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Pyre
{
    public class WastelandRoninPyreCharacterCardController : PyreUtilityCharacterCardController
    {
        public WastelandRoninPyreCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
        public override IEnumerator UsePower(int index = 0)
        {
            //"The next time {Pyre} would deal damage to a hero target, you may redirect it to another target. Draw 2 cards. Discard a card."
            yield break;
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator coroutine;
            switch (index)
            {
                case 0:
                    {
                        //"One player may discard a {PyreIrradiate} card. If they do, they deal 3 targets 3 energy damage each.",
                        break;
                    }
                case 1:
                    {
                        //"Each target regains 1 HP.",
                        break;
                    }
                case 2:
                    {
                        //"One hero may deal themselves 2 energy damage and play 2 cards."
                        break;
                    }
            }
            yield break;
        }
    }
}
