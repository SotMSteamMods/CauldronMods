using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;

namespace Cauldron.Malichae
{
    public abstract class DjinnTargetCardController : MalichaeCardController
    {
        private readonly LinqCardCriteria _djinnOngoingCriteria;

        protected DjinnTargetCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            _djinnOngoingCriteria = new LinqCardCriteria(c => c.IsInPlay && IsOngoing(c) && IsDjinn(c) && c.Location == this.Card.NextToLocation, "djinn ongoings");

            SpecialStringMaker.ShowNumberOfCards(_djinnOngoingCriteria);
        }

        public override bool CanBeDestroyed => false;

        public override IEnumerator DestroyAttempted(DestroyCardAction destroyCard)
        {
            var coroutine = DjinnDestroyInsteadReponse(destroyCard);
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

        private IEnumerator DjinnDestroyInsteadReponse(GameAction action)
        {
            var storedResults = new List<DestroyCardAction>();
            var cards = base.GameController.FindCardsWhere(c => _djinnOngoingCriteria.Criteria(c) && !this.GameController.IsCardIndestructible(c), visibleToCard: GetCardSource());
            IEnumerator coroutine;

            if (!cards.Any())
            {
                string message = $"There are no {_djinnOngoingCriteria.Description()} in play for {Card.Title} to destroy. {Card.Title} will be returned to {TurnTaker.Name}'s hand.";
                coroutine = base.GameController.SendMessageAction(message, Priority.Medium, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            else
            {
                coroutine = base.GameController.SelectAndDestroyCard(this.DecisionMaker, _djinnOngoingCriteria, false, storedResults, action.CardSource.Card, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }

            if (DidDestroyCard(storedResults))
            {
                coroutine = base.GameController.SetHP(this.Card, this.Card.MaximumHitPoints.Value, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                var card = GetDestroyedCards(storedResults).First();

                string message = $"{card.Title} was destroyed, and {Card.Title} was restored to {Card.MaximumHitPoints}.";
                coroutine = base.GameController.SendMessageAction(message, Priority.Medium, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            else
            {
                coroutine = base.GameController.MoveCard(this.DecisionMaker, this.Card, this.HeroTurnTaker.Hand,
                                cardSource: GetCardSource());
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
    }
}
