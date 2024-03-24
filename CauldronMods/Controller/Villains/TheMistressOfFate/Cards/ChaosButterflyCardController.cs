using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Handelabra;

namespace Cauldron.TheMistressOfFate
{
    public class ChaosButterflyCardController : TheMistressOfFateUtilityCardController
    {
        public ChaosButterflyCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            _isStoredCard = false;
        }

        public override void AddTriggers()
        {
            //"At the end of the villain turn, this card deals each hero target 3 projectile damage and 3 cold damage.",
            AddEndOfTurnTrigger(tt => tt == TurnTaker, DealDamageResponse, TriggerType.DealDamage);
            //"When this card is destroyed, the players may swap the position of 2 face up Day cards. Cards under them move as well."
            AddWhenDestroyedTrigger(SwapDayCardsResponse, TriggerType.MoveCard);
        }

        private IEnumerator DealDamageResponse(GameAction ga)
        {
            var damages = new List<DealDamageAction>
            {
                new DealDamageAction(GetCardSource(), new DamageSource(GameController, this.Card), null, 3, DamageType.Projectile),
                new DealDamageAction(GetCardSource(), new DamageSource(GameController, this.Card), null, 3, DamageType.Cold)
            };
            IEnumerator coroutine = DealMultipleInstancesOfDamage(damages, (Card c) => IsHeroTarget(c));
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

        private IEnumerator SwapDayCardsResponse(DestroyCardAction dc)
        {
            IEnumerator coroutine;
            var faceUpDayCards = TurnTaker.GetCardsWhere((Card c) => IsDay(c) && c.IsInPlayAndHasGameText).OrderBy(c => c.PlayIndex);
            if (faceUpDayCards.Count() < 2)
            {
                coroutine = GameController.SendMessageAction("There are not enough face-up day cards to swap their positions.", Priority.High, GetCardSource());
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

            var storedYesNo = new List<YesNoCardDecision>();
            coroutine = GameController.MakeYesNoCardDecision(DecisionMaker, SelectionType.Custom, this.Card, storedResults: storedYesNo, associatedCards: faceUpDayCards, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            if(!DidPlayerAnswerYes(storedYesNo))
            {
                yield break;
            }

            var storedSelection = new List<SelectCardDecision>();
            coroutine = GameController.SelectCardAndStoreResults(DecisionMaker, SelectionType.MoveCard, faceUpDayCards, storedSelection, selectionTypeOrdinal: 1, maintainCardOrder: true, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            var firstDay = GetSelectedCard(storedSelection);
            if(firstDay == null)
            {
                yield break;
            }

            storedSelection.Clear();
            coroutine = GameController.SelectCardAndStoreResults(DecisionMaker, SelectionType.MoveCard, faceUpDayCards, storedSelection, additionalCriteria: new LinqCardCriteria((Card c) => c != firstDay), selectionTypeOrdinal: 2, maintainCardOrder: true, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            var secondDay = GetSelectedCard(storedSelection);
            if(secondDay != null)
            {
                coroutine = SwapDayPositions(firstDay, secondDay);
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

        private IEnumerator SwapDayPositions(Card firstDay, Card secondDay)
        {
            int? firstDayStartingIndex = firstDay.PlayIndex;
            int? secondDayStartingIndex = secondDay.PlayIndex;
            //Log.Debug($"Switching {firstDay.Title} (index {firstDay.PlayIndex}) with {secondDay.Title} (index {secondDay.PlayIndex})");
            IEnumerator coroutine = GameController.SwitchCards(firstDay, secondDay, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            secondDay.PlayIndex = firstDayStartingIndex;
            firstDay.PlayIndex = secondDayStartingIndex;
            //Log.Debug($"Ended up with: {secondDay.Title} (index {secondDay.PlayIndex}), {firstDay.Title} (index {firstDay.PlayIndex})");

            coroutine = GameController.SwitchCardsAtLocations(TurnTakerController, firstDay.UnderLocation, secondDay.UnderLocation, cardSource: GetCardSource());
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

        public override CustomDecisionText GetCustomDecisionText(IDecision decision)
        {

            return new CustomDecisionText("Do you want to swap the position of 2 face up Day cards?", "Should they swap the position of 2 face up Day cards?", "Vote for if they should swap the position of 2 face up Day cards?", "swap position of 2 face up day cards");

        }
    }
}
