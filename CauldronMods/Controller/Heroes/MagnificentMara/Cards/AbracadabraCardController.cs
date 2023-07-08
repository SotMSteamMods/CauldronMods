using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
                                                                                dc.WasCardDestroyed;
                                                                                
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
                dc.AddAfterDestroyedAction(() => DestroyThisCardIfMovedResponse(dc), this);
            }

            yield break;
        }

        private IEnumerator DestroyThisCardIfMovedResponse(DestroyCardAction dc)
        {
            if (dc.CardToDestroy.Card.Location.IsHand)
            {
                var coroutine = DestroyThisCardResponse(dc);
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
            yield break;
        }
    }
}