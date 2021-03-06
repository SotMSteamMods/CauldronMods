﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Terminus
{
    public class GuiltyVerdictCardController : TerminusBaseCardController
    {
        /* 
         * Whenever a hero target is dealt 3 or more damage, add 1 token or remove 3 tokens from your Wrath pool. 
         * If you removed 3 tokens this way, increase damage dealt by that hero target by 1 until the start of 
         * the next environment turn.
         */
        public GuiltyVerdictCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowTokenPool(base.WrathPool);
        }

        private bool _wasTargetHeroBeforeDamage = false;

        public override void AddTriggers()
        {
            base.AddTrigger<DealDamageAction>((dda) => dda.Target.IsHero, StoreIsHero, TriggerType.HiddenLast, TriggerTiming.Before);
            base.AddTrigger<DealDamageAction>((dda) => _wasTargetHeroBeforeDamage && dda.Amount >= 3, DealDamageActionResponse, TriggerType.IncreaseDamage, TriggerTiming.After);
            base.AddTriggers();
        }

        private IEnumerator StoreIsHero(DealDamageAction dda)
        {
            _wasTargetHeroBeforeDamage = true;

            yield return null;
            yield break;
        }

        private IEnumerator DealDamageActionResponse(DealDamageAction dealDamageAction)
        {
            IEnumerator coroutine;
            if(!dealDamageAction.IsPretend)
            {
                _wasTargetHeroBeforeDamage = false;
            }
            coroutine = base.AddOrRemoveWrathTokens<GameAction, DealDamageAction>(1, 3, removeTokenResponse: RemoveTokensFromPoolResponse, removeTokenGameAction: dealDamageAction, insufficientTokenMessage: "nothing happens.", removeEffectDescription: "increase the target's damage", triggerAction: dealDamageAction);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }

        private IEnumerator RemoveTokensFromPoolResponse(DealDamageAction dealDamageAction, List<RemoveTokensFromPoolAction> storedResults)
        {
            IEnumerator coroutine;
            IncreaseDamageStatusEffect increaseDamageStatusEffect;

            if (DidRemoveTokens(storedResults, 3))
            {
                increaseDamageStatusEffect = new IncreaseDamageStatusEffect(1);
                increaseDamageStatusEffect.SourceCriteria.IsSpecificCard = dealDamageAction.Target;
                increaseDamageStatusEffect.UntilStartOfNextTurn(base.FindEnvironment().TurnTaker);
                coroutine = base.AddStatusEffect(increaseDamageStatusEffect);
            }
            else
            {
                coroutine = DoNothing();
            }
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }
    }
}
