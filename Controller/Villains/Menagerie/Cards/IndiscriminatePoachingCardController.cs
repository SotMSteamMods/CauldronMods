using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Menagerie
{
    public class IndiscriminatePoachingCardController : MenagerieCardController
    {
        public IndiscriminatePoachingCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //Reveal cards from the top of the villain deck until an Enclosure is revealed. Put it into play. Shuffle the other revealed cards back into the villain deck.
            IEnumerator coroutine = base.RevealCards_MoveMatching_ReturnNonMatchingCards(base.TurnTakerController, base.TurnTaker.Deck, false, true, false, new LinqCardCriteria((Card c) => base.IsEnclosure(c), "enclosure"), 1, revealedCardDisplay: RevealedCardDisplay.ShowMatchingCards);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            List<Card> highestEnclosures = new List<Card>();
            base.GameController.FindTargetWithHighestHitPoints(1, (Card c) => base.IsEnclosure(c) && c.IsInPlayAndHasGameText, highestEnclosures, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //...the Enclosure with the highest HP...
            Card highestEnclosure;
            if (highestEnclosures.Any())
            {
                if (highestEnclosures.Count() == 1)
                {
                    highestEnclosure = highestEnclosures.FirstOrDefault();
                }
                else
                {
                    List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
                    coroutine = base.GameController.SelectCardAndStoreResults(base.DecisionMaker, SelectionType.HighestHP, new LinqCardCriteria((Card c) => highestEnclosures.Contains(c), "enclosure"), storedResults, false, cardSource: base.GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                    highestEnclosure = storedResults.FirstOrDefault().SelectedCard;
                }

                //Put the top card of the environment deck beneath the Enclosure with the highest HP 
                coroutine = base.GameController.MoveCard(base.TurnTakerController, base.FindEnvironment().TurnTaker.Deck.TopCard, base.Card.UnderLocation, flipFaceDown: true, cardSource: base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }

            //...and destroy {H - 2} hero ongoing and/or equipment cards.
            coroutine = base.GameController.SelectAndDestroyCards(this.DecisionMaker, new LinqCardCriteria((Card c) => c.IsHero && (c.IsOngoing || base.IsEquipment(c))), 2, cardSource: base.GetCardSource());
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