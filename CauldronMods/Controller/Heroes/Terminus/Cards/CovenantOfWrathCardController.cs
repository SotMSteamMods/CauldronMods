using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Terminus
{
    public class CovenantOfWrathCardController : TerminusUtilityCardController
    {
        /* 
         * When this card would be destroyed, you may remove 3 tokens from your Wrath pool. If you removed 3 tokens this way, prevent that destruction.
         * Powers 
         * Terminus deals 1 target 6 cold damage. Destroy this card.
         */
        private int TargetCount => GetPowerNumeral(0, 1);
        private int ColdDamage => GetPowerNumeral(1, 6);
        private const int TokensToRemove = 3;

        public CovenantOfWrathCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowTokenPool(base.WrathPool);
        }

        public override void AddTriggers()
        {
            base.AddTrigger<DestroyCardAction>((dca) => dca.CardToDestroy.Card == base.Card, DestroyCardActionResponse, new TriggerType[] { TriggerType.ModifyTokens, TriggerType.DestroySelf }, TriggerTiming.Before, null, isConditional: false, requireActionSuccess: true, isActionOptional: true);
        }

        private IEnumerator DestroyCardActionResponse(DestroyCardAction destroyCardAction)
        {
            IEnumerator coroutine;
            List<RemoveTokensFromPoolAction> storedResults;

            if (destroyCardAction.CanBeCancelled)
            {
                // When this card would be destroyed, you may remove 3 tokens from your Wrath pool.
                storedResults = new List<RemoveTokensFromPoolAction>();
                coroutine = base.GameController.RemoveTokensFromPool(base.WrathPool, TokensToRemove, storedResults, optional: true, destroyCardAction, GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                if (DidRemoveTokens(storedResults, TokensToRemove))
                {
                    // If you removed 3 tokens this way, prevent that destruction.
                    coroutine = base.GameController.CancelAction(destroyCardAction, cardSource: GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                }
            }

            yield break;
        }

        public override IEnumerator UsePower(int index = 0)
        {
            IEnumerator coroutine;

            // Terminus deals 1 target 6 cold damage.
            coroutine = base.GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(base.GameController, base.Card), ColdDamage, DamageType.Cold, TargetCount, false, TargetCount, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            // Destroy this card.
            coroutine = base.GameController.DestroyCard(DecisionMaker, base.Card, cardSource: base.GetCardSource());
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
