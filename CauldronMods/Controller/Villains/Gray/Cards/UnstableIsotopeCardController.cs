using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Gray
{
    public class UnstableIsotopeCardController : CardController
    {
        public UnstableIsotopeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsAtLocation(base.TurnTaker.Deck, new LinqCardCriteria((Card c) => c.DoKeywordsContain("radiation"), "radiation"));
        }

        public override IEnumerator Play()
        {
            //Reveal cards from the top of the villain deck until 2 Radiation cards are revealed. 
            List<RevealCardsAction> storedResults = new List<RevealCardsAction>();
            IEnumerator coroutine = base.GameController.RevealCards(base.TurnTakerController, base.TurnTaker.Deck, (Card c) => c.DoKeywordsContain("radiation"), 2, storedResults, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            RevealCardsAction revealedCards = storedResults.FirstOrDefault<RevealCardsAction>();
            if (revealedCards != null)
            {
                if (revealedCards.RevealedCards != null && revealedCards.MatchingCards != null && revealedCards.MatchingCards.Count<Card>() > 0)
                {
                    //Put those cards into play...
                    foreach (Card radiation in revealedCards.MatchingCards)
                    {
                        if (radiation != null && radiation.DoKeywordsContain("radiation"))
                        {
                            coroutine = base.GameController.PlayCard(base.TurnTakerController, radiation, cardSource: base.GetCardSource());
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
                //...and discard the rest.
                foreach (Card cardToMove in revealedCards.NonMatchingCards)
                {
                    coroutine = base.GameController.MoveCard(base.TurnTakerController, cardToMove, base.TurnTaker.Trash, isDiscard: true, cardSource: base.GetCardSource());
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
            yield break;
        }
    }
}
