using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Terminus
{
    public class ImmortalCoilsCardController : TerminusBaseCardController
    {
        /*
         * Power
         * {Terminus} deals 1 target 3 cold damage. Add 1 or remove 3 tokens from your Wrath pool. If you removed 3 tokens this way, 
         * reduce damage dealt by that target by 1 until the start of your next turn.
         */
        private int ColdDamageTargetCount => GetPowerNumeral(0, 1);
        private int ColdDamageAmount => GetPowerNumeral(1, 3);
        private int AddTokenAmount => GetPowerNumeral(2, 1);
        private int RemoveTokenAmount => GetPowerNumeral(3, 3);
        private int NumTokensToRemoveAmount => GetPowerNumeral(4, 3);
        private int ReduceDamageAmount => GetPowerNumeral(5, 1);

        public ImmortalCoilsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowTokenPool(base.WrathPool);
        }

        public override IEnumerator UsePower(int index = 0)
        {
            IEnumerator coroutine;
            List<DealDamageAction> storedResults = new List<DealDamageAction>();
            DealDamageAction dealDamageAction = null;

            coroutine = base.GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), ColdDamageAmount, DamageType.Cold, ColdDamageTargetCount, false, ColdDamageTargetCount, storedResultsDamage: storedResults, cardSource: base.GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            if (storedResults != null && storedResults.Count() > 0)
            {
                dealDamageAction = storedResults.FirstOrDefault();
            }

            coroutine = base.AddOrRemoveWrathTokens<GameAction, DealDamageAction>(AddTokenAmount, RemoveTokenAmount, removeTokenResponse: RemoveTokensFromPoolResponse, removeTokenGameAction: dealDamageAction, insufficientTokenMessage: "nothing happens.", removeEffectDescription: "reduce the target's damage", triggerAction: dealDamageAction);
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
            ReduceDamageStatusEffect reduceDamageStatusEffect;

            if (DidRemoveTokens(storedResults, NumTokensToRemoveAmount) && dealDamageAction != null)
            {
                reduceDamageStatusEffect = new ReduceDamageStatusEffect(ReduceDamageAmount);
                reduceDamageStatusEffect.SourceCriteria.IsSpecificCard = dealDamageAction.Target;
                reduceDamageStatusEffect.UntilStartOfNextTurn(base.TurnTaker);
                coroutine = base.AddStatusEffect(reduceDamageStatusEffect);
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
