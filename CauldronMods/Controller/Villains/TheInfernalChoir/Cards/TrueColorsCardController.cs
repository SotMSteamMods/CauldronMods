using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheInfernalChoir
{
    public class TrueColorsCardController : TheInfernalChoirUtilityCardController
    {
        public TrueColorsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroTargetWithHighestHP();
            SpecialStringMaker.ShowIfSpecificCardIsInPlay(VagrantHeartSoulRevealedIdentifier);
        }

        public override IEnumerator Play()
        {
            var coroutine = base.DealDamageToHighestHP(CharacterCard, 1, c => IsHero(c), c => 6, DamageType.Infernal, numberOfTargets: () => 1);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            if (IsVagrantHeartSoulRevealedInPlay())
            {
                coroutine = base.DealDamageToHighestHP(CharacterCard, 1, c => IsHero(c), c => 6, DamageType.Cold, numberOfTargets: () => 1);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }
        }
    }
}
