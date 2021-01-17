using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Gyrosaur
{
    public class RenegadeGyrosaurCharacterCardController : GyrosaurUtilityCharacterCardController
    {
        public RenegadeGyrosaurCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
        public override IEnumerator UsePower(int index = 0)
        {
            //"Discard a card, if it was a crash card, {Gyrosaur} regains 2 HP. Otherwise, she deals 1 target 2 melee damage. Draw a card."
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
                        //"Each hero with fewer than 2 non-character cards in play may use a power now.",
                        break;
                    }
                case 2:
                    {
                        //"Targets cannot regain HP until the start of your next turn."
                        break;
                    }
            }
            yield break;
        }
    }
}
