using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Dynamo
{
    public class KineticEnergyBeamCardController : DynamoUtilityCardController
    {
        public KineticEnergyBeamCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroTargetWithHighestHP(ranking: 2);
        }

        public override IEnumerator Play()
        {
            //{Dynamo} deals the hero target with the second highest HP {H} energy damage.
            IEnumerator coroutine = base.DealDamageToHighestHP(base.CharacterCard, 2, (Card c) => IsHeroTarget(c), (Card c) => base.Game.H, DamageType.Energy, addStatusEffect: (DealDamageAction action) => this.IncreaseDamageTakenResponse(action));
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }

        private IEnumerator IncreaseDamageTakenResponse(DealDamageAction action)
        {
            //Increase damage dealt to that target by environment cards by 1 until the start of the next villain turn.
            IncreaseDamageStatusEffect statusEffect = new IncreaseDamageStatusEffect(1);
            statusEffect.TargetCriteria.IsSpecificCard = action.Target;
            statusEffect.SourceCriteria.IsEnvironment = true;
            statusEffect.UntilStartOfNextTurn(base.TurnTaker);
            statusEffect.UntilCardLeavesPlay(action.Target);

            IEnumerator coroutine = base.AddStatusEffect(statusEffect);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }
    }
}
