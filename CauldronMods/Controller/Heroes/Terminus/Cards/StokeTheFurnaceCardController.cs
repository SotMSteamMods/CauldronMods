using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Terminus
{
    public class StokeTheFurnaceCardController : TerminusBaseCardController
    {
        /* 
         * Draw 2 cards. 
         * Increase damage dealt by {Terminus} by 1 until the start of your next turn. 
         * Add or remove 3 tokens from your Wrath pool. If you removed 3 tokens this way, you may play a card.
         */
        public StokeTheFurnaceCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowTokenPool(base.WrathPool);
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;
            IncreaseDamageStatusEffect increaseDamageStatusEffect;

            // Draw 2 cards. 
            coroutine = base.DrawCards(DecisionMaker, 2);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            // Increase damage dealt by {Terminus} by 1 until the start of your next turn. 
            increaseDamageStatusEffect = new IncreaseDamageStatusEffect(1);
            increaseDamageStatusEffect.SourceCriteria.IsSpecificCard = base.CharacterCard;
            increaseDamageStatusEffect.UntilStartOfNextTurn(base.TurnTaker);

            coroutine = base.AddStatusEffect(increaseDamageStatusEffect);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            // Add or remove 3 tokens from your Wrath pool. If you removed 3 tokens this way, you may play a card.
            coroutine = base.AddOrRemoveWrathTokens<GameAction, GameAction>(3, 3, removeTokenResponse: RemoveTokensResponse, insufficientTokenMessage: "you may not play a card.");
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

        public IEnumerator RemoveTokensResponse(GameAction gameAction, List<RemoveTokensFromPoolAction> storedResults)
        {
            IEnumerator coroutine;

            if (DidRemoveTokens(storedResults, 3))
            {
                // If you removed 3 tokens this way, you may play a card.
                coroutine = base.SelectAndPlayCardFromHand(DecisionMaker);
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
