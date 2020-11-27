using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.MagnificentMara
{
    class MagnificentMaraCharacterCardController : HeroCharacterCardController
    {
        public MagnificentMaraCharacterCardController(Card card, TurnTakerController ttc) : base(card, ttc)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //"{MagnificentMara} deals 1 target 1 psychic damage. That target deals another target 1 melee damage."
            yield break;
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            switch(index)
            {
                case (0):
                    {
                        //"One hero may put a card from their trash on top of their deck.",

                        break;
                    }
                case (1):
                    {
                        //"Destroy 1 ongoing card.",

                        break;
                    }
                case (2):
                    {
                        //"Select a target. Until the start of your next turn that target is immune to damage from environment cards."

                        break;
                    }
            }
            yield break;
        }
    }
}
