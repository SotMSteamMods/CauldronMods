using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Terminus
{
    public class RailwaySpikeCardController : TerminusMementoCardController
    {
        /* 
         * This card is indestructible. If another Memento would enter play, instead remove it from the game and draw 4 cards. 
         * Whenever {Terminus} is dealt damage, add 1 token or remove 3 tokens from your Wrath pool. If you removed 3 tokens 
         * this way, {Terminus} deals the source of that damage 3 cold damage. 
         */
        public RailwaySpikeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowTokenPool(base.WrathPool);
        }

        public override void AddTriggers()
        {
            base.AddTrigger<DealDamageAction>((dda) => dda.Target == base.CharacterCard && dda.DidDealDamage, DealDamageActionResponse, new TriggerType[] { TriggerType.ModifyTokens, TriggerType.AddTokensToPool, TriggerType.DealDamage }, TriggerTiming.After);
            base.AddTriggers();
        }

        private IEnumerator DealDamageActionResponse(DealDamageAction dealDamageAction)
        {
            IEnumerator coroutine;
            List<Function> list = new List<Function>();
            // SelectFunctionDecision selectFunction;

            coroutine = base.AddOrRemoveWrathTokens<GameAction, DealDamageAction>(1, 3, removeTokenResponse: RemoveTokenResponse, removeTokenGameAction: dealDamageAction, insufficientTokenMessage: "no damage was dealt.", removeEffectDescription: "deal counter damage", triggerAction: dealDamageAction);
            //list.Add(new Function(DecisionMaker, $"Add 1 token to {WrathPool.Name}", SelectionType.AddTokens, () => base.GameController.AddTokensToPool(WrathPool, 1, base.GetCardSource())));
            //list.Add(new Function(DecisionMaker, $"Remove 3 tokens from  {WrathPool.Name}", SelectionType.RemoveTokens, () => RemoveTokenResponse(dealDamageAction)));
            //selectFunction = new SelectFunctionDecision(GameController, DecisionMaker, list, false, null, null, null, base.GetCardSource());
            //coroutine = GameController.SelectAndPerformFunction(selectFunction);
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

        private IEnumerator RemoveTokenResponse(DealDamageAction dealDamageAction, List<RemoveTokensFromPoolAction> removeTokensFromPoolActions)
        {
            IEnumerator coroutine;
            //List<RemoveTokensFromPoolAction> storedResults = new List<RemoveTokensFromPoolAction>();

            //coroutine = base.GameController.RemoveTokensFromPool(WrathPool, 3, storedResults, optional: false, null, base.GetCardSource());
            //if (UseUnityCoroutines)
            //{
            //    yield return GameController.StartCoroutine(coroutine);
            //}
            //else
            //{
            //    GameController.ExhaustCoroutine(coroutine);
            //}

            if (base.GetNumberOfTokensRemoved(removeTokensFromPoolActions) >= 3)
            {
                coroutine = base.GameController.DealDamageToTarget(new DamageSource(base.GameController, base.CharacterCard), dealDamageAction.DamageSource.Card, 3, DamageType.Cold, cardSource: base.GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }

            yield break;
        }

        protected override IEnumerator OnOtherMementoRemoved()
        {
            // and draw 4 cards. 
            return base.DrawCards(DecisionMaker, 4);
        }
    }
}
