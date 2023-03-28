using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.ScreaMachine
{
    public class PoundingRhythmCardController : ScreaMachineBandCardController
    {
        public PoundingRhythmCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, ScreaMachineBandmate.Value.RickyG)
        {
            SpecialStringMaker.ShowHeroTargetWithHighestHP();
        }

        protected override IEnumerator ActivateBandAbility()
        {
            var coroutine = DealDamageToHighestHP(GetBandmate(), 1, c => IsHeroTarget(c) && c.IsInPlayAndNotUnderCard, c => 1, DamageType.Melee, true,
                                addStatusEffect: MakeStatusEffect);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator MakeStatusEffect(DealDamageAction action)
        {
            IEnumerator coroutine;
            if (action != null && action.DidDealDamage && action.Target.IsInPlayAndNotUnderCard && !action.DidDestroyTarget)
            {
                var effect = new ReduceDamageStatusEffect(2);
                effect.SourceCriteria.IsSpecificCard = action.Target;
                effect.CardSource = Card;
                effect.NumberOfUses = 1;
                effect.UntilTargetLeavesPlay(action.Target);

                coroutine = AddStatusEffect(effect);
            }
            else
            {
                coroutine =  DoNothing();
            }
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
