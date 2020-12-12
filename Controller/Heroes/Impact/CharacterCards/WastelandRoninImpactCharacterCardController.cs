using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Impact
{
    public class WastelandRoninImpactCharacterCardController : HeroCharacterCardController
    {
        public WastelandRoninImpactCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //"{Impact} regains 1 HP, or the next time one of your ongoing cards is destroyed, play it."
            yield break;
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator coroutine;
            switch (index)
            {
                case 0:
                    {
                        //"One hero may use a power now.",
                        yield break;
                    }
                case 1:
                    {
                        //"Environment cards cannot be played during the next environment turn.",
                        break;
                    }
                case 2:
                    {
                        //"Select a target. The next damage it deals is irreducible."
                        break;
                    }
            }
            yield break;
        }
    }
}