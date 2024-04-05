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
        private const string SavedCard = "AbracadabraSavedCard";
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
                Journal.RecordCardProperties(Card, SavedCard, dc.CardToDestroy.Card);
                dc.SetPostDestroyDestination(dc.CardToDestroy.Card.Owner.ToHero().Hand);
                dc.AddAfterDestroyedAction(() => DestroyThisCardIfMovedResponse(dc), this);
            }

            yield break;
        }

        private IEnumerator DestroyThisCardIfMovedResponse(DestroyCardAction dc)
        {
            Card savingCard = Journal.CardPropertiesEntriesThisTurn((prop) => prop.Key == SavedCard && prop.OtherCard == dc.CardToDestroy.Card).FirstOrDefault().Card;
            CardController savingController = FindCardController(savingCard);
            if (dc.CardToDestroy.Card.Location.IsHand)
            {
                GameController gameController = savingController.GameController;
                HeroTurnTakerController decisionMaker = savingController.DecisionMaker;
                Card card = savingController.Card;
                CardSource cardSource = savingController.GetCardSource();
                var coroutine = GameController.DestroyCard(decisionMaker, card, optional: false, null, null, null, null, null, null, null, null, cardSource);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }

            // If the card destroyed itself, this flag has no more purpose
            // If the destruction was prevented somehow, it should also be reset
            Journal.RemoveCardProperties(savingCard, SavedCard);
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