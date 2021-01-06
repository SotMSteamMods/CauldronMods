using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.ScreaMachine
{
    public class TheSetListCardController : ScreaMachineUtilityCharacterCardController
    {
        public TheSetListCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsUnderCard(this.Card);
        }

        public IEnumerator RevealTopCardOfTheSetList()
        {
            List<Card> dummy = new List<Card>();
            var coroutine = GameController.RevealCards(TurnTakerController, this.Card.UnderLocation, 1, dummy, revealedCardDisplay: RevealedCardDisplay.ShowRevealedCards, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator TheSetListRevealProcess(RevealCardsAction rca)
        {
            foreach (var card in rca.RevealedCards)
            {
                var keywords = new HashSet<string>(GameController.GetAllKeywords(card), StringComparer.OrdinalIgnoreCase);
                var sharesAKeyword = FindCardsWhere(new LinqCardCriteria(c => c.IsInPlayAndNotUnderCard && GameController.GetAllKeywords(c).Any(k => keywords.Contains(k))), GetCardSource()).Any();
                var firstCC = FindCardController(card) as ScreaMachineBandCardController;
                Card cardToPlay = card;
                IEnumerator coroutine;
                if (!sharesAKeyword)
                {
                    coroutine = GameController.MoveCard(TurnTakerController, card, Card.UnderLocation, toBottom: true, playCardIfMovingToPlayArea: false, cardSource: GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                    cardToPlay = this.Card.UnderLocation.TopCard;
                }

                var secondCC = FindCardController(cardToPlay) as ScreaMachineBandCardController;
                coroutine = TheSetListFlavorMessage(firstCC, secondCC);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                coroutine = GameController.PlayCard(TurnTakerController, cardToPlay, reassignPlayIndex: true, cardSource: GetCardSource());
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

        private IEnumerator TheSetListFlavorMessage(ScreaMachineBandCardController firstCard, ScreaMachineBandCardController secondCard)
        {
            if (firstCard != null && secondCard != null)
            {
                Card card;
                string message;
                if (firstCard.Member == secondCard.Member)
                {
                    var bandMate = firstCard.GetBandmate();
                    if (firstCard.Card == secondCard.Card)
                    {
                        card = firstCard.Card;
                        message = $"{bandMate.Title} is ramping it up!";
                    }
                    else
                    {
                        card = secondCard.Card;
                        message = $"{bandMate.Title} is taking the solo!";
                    }
                }
                else
                {
                    card = secondCard.Card;
                    message = $"{firstCard.GetBandmate().Title} is keeping it mellow, while {secondCard.GetBandmate().Title} has the lime-light!";
                }

                var coroutine = GameController.SendMessageAction(message, Priority.Medium, GetCardSource(), new[] { card });
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

        public override void AddSideTriggers()
        {
            base.AddSideTriggers();

            if (!Card.IsFlipped)
            {
                AddSideTrigger(AddStartOfTurnTrigger(tt => tt == TurnTaker, pca => FlipThisCharacterCardResponse(pca), TriggerType.FlipCard));
            }
            else
            {
                AddSideTrigger(AddTrigger<RevealCardsAction>(rca => rca.SearchLocation == this.Card.UnderLocation, TheSetListRevealProcess, TriggerType.PlayCard, TriggerTiming.After));
                AddSideTrigger(AddStartOfTurnTrigger(tt => tt == TurnTaker, pca => RevealTopCardOfTheSetList(), TriggerType.RevealCard));

                if (Game.IsAdvanced)
                {
                    AddSideTrigger(AddTrigger<FlipCardAction>(fca => IsVillainTarget(fca.CardToFlip.Card), fca => FlipCardResponse(), TriggerType.DestroyCard, TriggerTiming.After));
                }
            }
        }

        private IEnumerator FlipCardResponse()
        {
            var coroutine = GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria(c => c.IsInPlayAndNotUnderCard && c.IsHero && (c.IsOngoing || IsEquipment(c)), "hero ongoing or equipment"), false, cardSource: GetCardSource());
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
