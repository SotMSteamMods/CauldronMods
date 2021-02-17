using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.DungeonsOfTerror
{
    public class OneInAMillionCardController : DungeonsOfTerrorUtilityCardController
    {
        public OneInAMillionCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //When this card enters play, discard and check the top 2 cards of the environment deck.
            List<MoveCardAction> storedDiscard = new List<MoveCardAction>();
            IEnumerator coroutine = DiscardCardsFromTopOfDeck(TurnTakerController, 2, storedResults: storedDiscard, showMessage: true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            IEnumerable<Card> cardsToCheck;
            if (DidMoveCard(storedDiscard))
            {
                cardsToCheck = storedDiscard.Select(mca => mca.CardToMove);
            }
            else
            {
                cardsToCheck = TurnTaker.Deck.GetTopCards(2);
            }
            List<int> storedResults = new List<int>();
            coroutine = CheckForNumberOfFates(cardsToCheck, storedResults, TurnTaker.Deck);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            IEnumerator message = DoNothing();
            IEnumerator effect = DoNothing();
            if (storedResults.Any() && storedResults.First() == 2)
            {
                //If at least 1 is a fate card, each player draws a card.
                //If both are fate cards, skip the next villain turn.
                message = GameController.SendMessageAction($"What are the odds! Both of the top cards of the environment deck were fate cards!", Priority.High, GetCardSource(), associatedCards: cardsToCheck, showCardSource: true);
                effect = TwoMatchingFates();
            }
            else if (storedResults.Any() && storedResults.First() == 1)
            {
                //If at least 1 is a fate card, each player draws a card.
                message = GameController.SendMessageAction($"One of the top cards of the environment deck was a fate card!", Priority.High, GetCardSource(), associatedCards: cardsToCheck, showCardSource: true);
                effect = OneMatchingFate();
            }
            else if (storedResults.Any() && storedResults.First() == 0)
            {
                message = GameController.SendMessageAction($"None of the top cards of the environment deck were fate cards!", Priority.High, GetCardSource(), associatedCards: cardsToCheck, showCardSource: true);
            }
            else
            {
                message = GameController.SendMessageAction("There were no cards in the environment deck!", Priority.High, GetCardSource(), showCardSource: true);
            }
            //Then, destroy this card.
            coroutine = DestroyThisCardResponse(null);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(message);
                yield return base.GameController.StartCoroutine(effect);
                yield return base.GameController.StartCoroutine(coroutine);

            }
            else
            {
                base.GameController.ExhaustCoroutine(message);
                base.GameController.ExhaustCoroutine(effect);
                base.GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }

        private IEnumerator OneMatchingFate()
        {
            //If at least 1 is a fate card, each player draws a card.
            IEnumerator coroutine = EachPlayerDrawsACard();
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

        private IEnumerator TwoMatchingFates()
        {
            //If at least 1 is a fate card, each player draws a card.
            IEnumerator coroutine = OneMatchingFate();
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            //If both are fate cards, skip the next villain turn.
            OnPhaseChangeStatusEffect effect = new OnPhaseChangeStatusEffect(Card, "SkipVillainTurn", "Skip the next villain turn.", new TriggerType[] { TriggerType.SkipTurn }, Card);
            effect.NumberOfUses = 1;
            effect.TurnTakerCriteria.IsVillain = true;
            effect.TurnPhaseCriteria.Phase = Phase.Start;
            effect.BeforeOrAfter = BeforeOrAfter.After;
            coroutine = AddStatusEffect(effect);
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

        public IEnumerator SkipVillainTurn(PhaseChangeAction p, StatusEffect effect)
        {
            if (p.ToPhase == null || p.ToPhase.TurnTaker == null || !p.ToPhase.TurnTaker.IsVillain)
            {
                yield break;
            }

            IEnumerator coroutine = GameController.SkipToNextTurn(GetCardSource());
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
