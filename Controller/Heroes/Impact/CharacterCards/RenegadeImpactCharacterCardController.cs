using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Impact
{
    public class RenegadeImpactCharacterCardController : HeroCharacterCardController
    {
        public RenegadeImpactCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //"Destroy 1 of your ongoing cards. If you do, play a card and draw a card."
            yield break;
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator coroutine;
            switch (index)
            {
                case 0:
                    {
                        //"Select a target. Damage dealt by that target is irreducible during its next turn.",
                        yield break;
                    }
                case 1:
                    {
                        //"One hero may use a power now.",
                        break;
                    }
                case 2:
                    {
                        //"The environment deals 1 target 2 projectile damage."
                        break;
                    }
            }
            yield break;
        }
    }
}