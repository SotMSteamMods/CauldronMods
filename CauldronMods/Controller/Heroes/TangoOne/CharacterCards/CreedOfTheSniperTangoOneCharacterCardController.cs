using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TangoOne
{
    public class CreedOfTheSniperTangoOneCharacterCardController : HeroCharacterCardController
    {
        private const int PlayersToPlay = 1;

        public CreedOfTheSniperTangoOneCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //==============================================================
            // Draw a card. If it is a critical card, discard it and 1 player plays a card.
            //==============================================================
            
            int players = GetPowerNumeral(0, PlayersToPlay);

            List<DrawCardAction> storedResults = new List<DrawCardAction>();
            // Draw a cards
            var coroutine = base.DrawCards(DecisionMaker, 1, storedResults: storedResults);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (DidDrawCards(storedResults, 1))
            {
                var card = storedResults.First().DrawnCard;
                if (GameController.DoesCardContainKeyword(card, "critical"))
                {
                    coroutine = GameController.SendMessageAction($"{card.Title} is a critical card, so it is discarded!", Priority.Medium, GetCardSource(), associatedCards: card.ToEnumerable());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }

                    coroutine = GameController.DiscardCard(DecisionMaker, card, null, cardSource: GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }

                    List<TurnTaker> playedCards = new List<TurnTaker>();
                    while (--players >= 0)
                    {
                        List<SelectTurnTakerDecision> storedTurnTaker = new List<SelectTurnTakerDecision>();

                        coroutine = SelectHeroToPlayCard(DecisionMaker, optionalPlayCard: false, storedResultsTurnTaker: storedTurnTaker,
                                        heroCriteria: new LinqTurnTakerCriteria(tt => !playedCards.Contains(tt)));
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }

                        if (DidSelectTurnTaker(storedTurnTaker))
                        {
                            playedCards.Add(GetSelectedTurnTaker(storedTurnTaker));
                        }
                    }
                }
            }
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            switch (index)
            {
                case 0:
                    {
                        //==============================================================
                        // "One player may draw a card now.",
                        //==============================================================

                        IEnumerator drawCardRoutine = this.GameController.SelectHeroToDrawCard(DecisionMaker,
                            cardSource: base.GetCardSource());

                        if (this.UseUnityCoroutines)
                        {
                            yield return this.GameController.StartCoroutine(drawCardRoutine);
                        }
                        else
                        {
                            this.GameController.ExhaustCoroutine(drawCardRoutine);
                        }
                    }
                    break;

                case 1:
                    {
                        //==============================================================
                        // "One player may use a power now.",
                        //==============================================================

                        IEnumerator usePowerRoutine = this.GameController.SelectHeroToUsePower(DecisionMaker,
                            cardSource: base.GetCardSource());

                        if (this.UseUnityCoroutines)
                        {
                            yield return this.GameController.StartCoroutine(usePowerRoutine);
                        }
                        else
                        {
                            this.GameController.ExhaustCoroutine(usePowerRoutine);
                        }
                    }
                    break;

                case 2:
                    {
                        //==============================================================
                        // "Discard the top 2 cards of a deck. If they share a keyword, put them into play in any order."
                        //==============================================================
                        var coroutine = DiscardAndPlayMatchingKeywords();
                        if (this.UseUnityCoroutines)
                        {
                            yield return this.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            this.GameController.ExhaustCoroutine(coroutine);
                        }
                    }
                    break;
            }
        }

        private IEnumerator DiscardAndPlayMatchingKeywords()
        {
            //stolen verbatium from Super Scientific Tachyon, changing bottom to top.

            List<SelectLocationDecision> storedResults = new List<SelectLocationDecision>();
            IEnumerator coroutine = base.GameController.SelectADeck(base.HeroTurnTakerController, SelectionType.RevealCardsFromDeck, (Location l) => true, storedResults, false, null, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if (!DidSelectDeck(storedResults))
            {
                yield break;
            }
            Location deck = storedResults.First().SelectedLocation.Location;
            List<Card> revealedCards = new List<Card>();
            if (deck.Cards.Count() == 1)
            {
                CardController revealedCard = base.GameController.FindCardController(deck.TopCard);
                coroutine = base.GameController.SendMessageAction(revealedCard.Card.Title + " was the only card in " + deck.GetFriendlyName() + ". It must be discarded.", Priority.High, GetCardSource(), new Card[1]
                {
                    revealedCard.Card
                });
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                coroutine = base.GameController.MoveCard(base.TurnTakerController, revealedCard.Card, deck.OwnerTurnTaker.Trash, false, false, true, null, false, null, base.TurnTaker, null, false, false, null, true, false, false, false, GetCardSource());
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
                //fromBottom changed to false here
                IEnumerator coroutine2 = base.GameController.RevealCards(base.TurnTakerController, deck, 2, revealedCards, false, RevealedCardDisplay.None, null, GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine2);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine2);
                }
                if (revealedCards.Count == 2)
                {
                    Card card = revealedCards.First();
                    IEnumerable<string> allKeywords = base.GameController.GetAllKeywords(card);
                    Card card2 = revealedCards.Last();
                    if (allKeywords.Intersect(base.GameController.GetAllKeywords(card2)).Any())
                    {
                        coroutine = base.GameController.PlayCards(base.HeroTurnTakerController, (Card c) => revealedCards.Contains(c), false, true, null, null, false, null, null, null, base.TurnTaker, GetCardSource());
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
                        List<SelectCardDecision> selectedCards = new List<SelectCardDecision>();
                        IEnumerator coroutine3 = base.GameController.SelectCardAndStoreResults(base.HeroTurnTakerController, SelectionType.DiscardCard, revealedCards, selectedCards, false, true);
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine3);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine3);
                        }
                        Card firstToDiscard = selectedCards.Where((SelectCardDecision d) => d.Completed && d.SelectedCard != null).Select((SelectCardDecision d) => d.SelectedCard).FirstOrDefault();
                        if (firstToDiscard != null)
                        {
                            IEnumerator coroutine4 = base.GameController.MoveCard(base.TurnTakerController, firstToDiscard, deck.OwnerTurnTaker.Trash, false, false, true, null, false, null, base.TurnTaker, null, false, false, null, true, false, false, false, GetCardSource());
                            Card cardToMove = revealedCards.First((Card c) => c != firstToDiscard);
                            IEnumerator move2 = base.GameController.MoveCard(base.TurnTakerController, cardToMove, deck.OwnerTurnTaker.Trash, false, false, true, null, false, null, base.TurnTaker, null, false, false, null, true, false, false, false, GetCardSource());
                            if (base.UseUnityCoroutines)
                            {
                                yield return base.GameController.StartCoroutine(coroutine4);
                                yield return base.GameController.StartCoroutine(move2);
                            }
                            else
                            {
                                base.GameController.ExhaustCoroutine(coroutine4);
                                base.GameController.ExhaustCoroutine(move2);
                            }
                        }
                    }
                }
            }
            List<Location> list = new List<Location>();
            list.Add(deck.OwnerTurnTaker.Revealed);
            IEnumerator coroutine5 = CleanupCardsAtLocations(list, deck, true, true, false, false, false, true, revealedCards);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine5);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine5);
            }
        }
    }
}
