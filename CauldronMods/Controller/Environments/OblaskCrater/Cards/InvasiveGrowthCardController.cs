using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.OblaskCrater
{
    public class InvasiveGrowthCardController : OblaskCraterUtilityCardController
    {
        /* 
         * When this card enters play, discard the top 2 cards of the enviroment deck, then put a random target 
         * from the environment trash into play.
         * When an environment target is destroyed, all other environment cards become indestructible until the 
         * start of the next turn.
         */
        public InvasiveGrowthCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            base.AddTrigger<DestroyCardAction>(DestroyCardActionCriteria, DestroyCardActionResponse, TriggerType.DestroyCard, TriggerTiming.After);
        }

        private bool DestroyCardActionCriteria(DestroyCardAction destroyCardAction)
        {
            return destroyCardAction.CardToDestroy != null && destroyCardAction.WasCardDestroyed && destroyCardAction.CardToDestroy.Card.IsEnvironmentTarget;
        }

        private IEnumerator DestroyCardActionResponse(DestroyCardAction destroyCardAction)
        {
            /* 
             * When an environment target is destroyed, all other environment cards become indestructible until the 
             * start of the next turn.
             */
            IEnumerator coroutine;
            MakeIndestructibleStatusEffect makeIndestructibleStatusEffect;

            makeIndestructibleStatusEffect = new MakeIndestructibleStatusEffect();
            makeIndestructibleStatusEffect.CardsToMakeIndestructible.IsEnvironment = true;
            makeIndestructibleStatusEffect.UntilStartOfNextTurn(base.FindNextTurnTaker());

            coroutine = base.AddStatusEffect(makeIndestructibleStatusEffect);
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

        public override IEnumerator Play()
        {
            /* 
             * When this card enters play, discard the top 2 cards of the enviroment deck, then put a random target 
             * from the environment trash into play.
             */
            IEnumerator coroutine;
            List<RevealCardsAction> storedRevealCardResults = new List<RevealCardsAction>();
            Location environmentTrash = FindLocationsWhere(location => location.IsRealTrash && location.IsEnvironment && GameController.IsLocationVisibleToSource(location, GetCardSource())).First();
            TurnTakerController environmentTurnTakerController = FindEnvironment();
            List<Card> otherCards;
            Card matchedCard;

            coroutine = base.DiscardCardsFromTopOfDeck(environmentTurnTakerController, 2);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = base.ShuffleDeck(DecisionMaker, environmentTrash);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = base.GameController.RevealCards(base.TurnTakerController,environmentTrash, (card) => card.IsTarget, 1, storedRevealCardResults);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            matchedCard = GetRevealedCards(storedRevealCardResults).FirstOrDefault(c => c.IsTarget);
            otherCards = GetRevealedCards(storedRevealCardResults).Where(c => !c.IsTarget).ToList();
            if (otherCards.Any())
            {
                // Put non matching revealed cards back in the trash
                coroutine = GameController.MoveCards(environmentTurnTakerController, otherCards, environmentTrash, cardSource: GetCardSource());
                if (this.UseUnityCoroutines)
                {
                    yield return this.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    this.GameController.ExhaustCoroutine(coroutine);
                }
            }

            if (matchedCard != null)
            {
                coroutine = base.GameController.MoveCard(environmentTurnTakerController, matchedCard, environmentTurnTakerController.TurnTaker.PlayArea, cardSource: base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }

            yield break;
        }
    }
}
