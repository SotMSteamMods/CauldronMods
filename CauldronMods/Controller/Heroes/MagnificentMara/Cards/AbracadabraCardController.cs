using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Cauldron.MagnificentMara
{
    public class AbracadabraCardController : CardController
    {
        public AbracadabraCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //"When a non-character card belonging to another hero is destroyed, you may return it to that player's hand. If you do, destroy this card.",
            Func<DestroyCardAction, bool> validCardDestroyed = (DestroyCardAction dc) => !this.IsBeingDestroyed &&
                                                                                dc.CardToDestroy != null &&
                                                                                dc.CardToDestroy.Card != null &&
                                                                                IsHero(dc.CardToDestroy.Card) &&
                                                                                !dc.CardToDestroy.Card.IsCharacter &&
                                                                                dc.CardToDestroy.Card.Owner != TurnTaker &&
                                                                                dc.WasCardDestroyed &&
                                                                                dc.PostDestroyDestinationCanBeChanged;
                                                                                
            AddTrigger(validCardDestroyed, MayReturnDestroyedResponse, new TriggerType[] { TriggerType.MoveCard, TriggerType.DestroySelf }, TriggerTiming.After);
            
            //"When this card is destroyed, one player may play a card."

            AddWhenDestroyedTrigger(OnePlayerMayPlayCardResponse, TriggerType.PlayCard);
        }

        private IEnumerator MayReturnDestroyedResponse(DestroyCardAction dc)
        {
            Card toReturn = dc.CardToDestroy.Card;

            var decisionResult = new List<YesNoCardDecision> { };
            IEnumerator coroutine = GameController.MakeYesNoCardDecision(DecisionMaker, SelectionType.MoveCardToHand, toReturn, storedResults: decisionResult, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (DidPlayerAnswerYes(decisionResult))
            {
                dc.SetPostDestroyDestination(dc.CardToDestroy.Card.Owner.ToHero().Hand);

                var card = Card;
                dc.AddAfterDestroyedAction(() => DestroyThisCardIfMovedResponse(dc, card), this);
            }

            yield break;
        }

        private IEnumerator DestroyThisCardIfMovedResponse(DestroyCardAction dc, Card savingCard)
        {
            CardController savingController = FindCardController(savingCard);
            if (dc.CardToDestroy.Card.Location.IsHand)
            {
                var coroutine = GameController.DestroyCard(savingController.DecisionMaker, savingCard, optional: false, null, cardSource: savingController.GetCardSource());
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

        private IEnumerator OnePlayerMayPlayCardResponse(DestroyCardAction dc)
        {
            IEnumerator coroutine = GameController.SelectHeroToPlayCard(DecisionMaker, cardSource: GetCardSource());
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
