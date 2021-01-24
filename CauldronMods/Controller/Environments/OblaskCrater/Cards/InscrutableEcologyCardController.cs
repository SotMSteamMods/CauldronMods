using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.OblaskCrater
{
    public class InscrutableEcologyCardController : OblaskCraterUtilityCardController
    {
        /* 
         * When this card enters play, reveal cards from the top of the environmnet deck until {H - 1} targets are 
         * revealed, put them into play, and discard the other revealed cards.
         * Reduce damage dealt by environment targets by 1. At the start of the environment turn, destroy this card.
         */
        public InscrutableEcologyCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            base.AddStartOfTurnTrigger(tt => tt == base.TurnTaker, base.DestroyThisCardResponse, TriggerType.DestroySelf);
        }

        public override IEnumerator Play()
        {
            Location environmentDeck = FindLocationsWhere(location => location.IsRealDeck && location.IsEnvironment && GameController.IsLocationVisibleToSource(location, GetCardSource())).First();
            Location environmentTrash = FindLocationsWhere(location => location.IsRealTrash && location.IsEnvironment && GameController.IsLocationVisibleToSource(location, GetCardSource())).First();
            List<RevealCardsAction> revealedCards = new List<RevealCardsAction>();
            IEnumerator revealCardsRoutine;
            IEnumerator returnCardsRoutine;
            IEnumerator coroutine;
            List<Card> matchedCards;
            List<Card> otherCards;
            ReduceDamageStatusEffect reduceDamageStatusEffect;

            // reveal cards from the top of the environmnet deck until {H - 1} targets are  revealed
            revealCardsRoutine = base.GameController.RevealCards(TurnTakerController, environmentDeck, card => card.IsTarget, base.H - 1, revealedCards);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(revealCardsRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(revealCardsRoutine);
            }

            matchedCards = GetRevealedCards(revealedCards).Where(c => c.IsTarget).ToList();
            otherCards = GetRevealedCards(revealedCards).Where(c => !c.IsTarget).ToList();
            if (otherCards.Any())
            {
                // Put non matching revealed cards back in the trash
                returnCardsRoutine = GameController.MoveCards(DecisionMaker, otherCards, environmentTrash, cardSource: GetCardSource());
                if (this.UseUnityCoroutines)
                {
                    yield return this.GameController.StartCoroutine(returnCardsRoutine);
                }
                else
                {
                    this.GameController.ExhaustCoroutine(returnCardsRoutine);
                }
            }

            if (matchedCards != null && matchedCards.Count() > 0)
            {
                coroutine = base.GameController.PlayCards(base.DecisionMaker, (card) => matchedCards.Contains(card), false, true, cardSource: base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }

            // Reduce damage dealt by environment targets by 1.            
            reduceDamageStatusEffect = new ReduceDamageStatusEffect(1);
            reduceDamageStatusEffect.SourceCriteria.IsEnvironment = true;
            reduceDamageStatusEffect.SourceCriteria.IsTarget = true;
            coroutine = base.AddStatusEffect(reduceDamageStatusEffect);
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
