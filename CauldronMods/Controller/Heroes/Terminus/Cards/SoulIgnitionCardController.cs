using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Terminus
{
    public class SoulIgnitionCardController : TerminusBaseCardController
    {
        /* 
         * Add 3 tokens to your Wrath pool. 
         * You may use a power now. 
         * You may discard a card. If you do, add 3 tokens to your Wrath pool or use a power now.
         */
        public SoulIgnitionCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowTokenPool(base.WrathPool);
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;
            List<DiscardCardAction> storedResults = new List<DiscardCardAction>();
            List<Function> functionList;
            SelectFunctionDecision selectFunction;

            // Add 3 tokens to your Wrath pool.
            coroutine = base.AddWrathTokens(3);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            // You may use a power now. 
            coroutine = base.SelectAndUsePower(base.CharacterCardController);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            // You may discard a card. If you do, add 3 tokens to your Wrath pool or use a power now.
            coroutine = base.SelectAndDiscardCards(DecisionMaker, 1, true, storedResults: storedResults);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            if (DidDiscardCards(storedResults))
            {
                // If you do, add 3 tokens to your Wrath pool or use a power now.
                functionList = new List<Function>();

                functionList.Add(new Function(DecisionMaker, $"Add 3 tokens to {WrathPool.Name}", SelectionType.AddTokens, () => base.AddWrathTokens(3)));
                functionList.Add(new Function(DecisionMaker, $"Use a power now", SelectionType.RemoveTokens, () => base.SelectAndUsePower(base.CharacterCardController)));
                selectFunction = new SelectFunctionDecision(GameController, DecisionMaker, functionList, false, null, null, null, base.GetCardSource());

                coroutine = base.GameController.SelectAndPerformFunction(selectFunction);
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
    }
}
