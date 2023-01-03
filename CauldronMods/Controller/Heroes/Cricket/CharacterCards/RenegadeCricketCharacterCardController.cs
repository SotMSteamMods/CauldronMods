using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Cricket
{
    public class RenegadeCricketCharacterCardController : CricketCharacterSubCardController
    {
        public RenegadeCricketCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator coroutine = null;
            switch (index)
            {
                case 0:
                    {
                        //Select a deck and put its top card into play.
                        List<SelectLocationDecision> storedResults = new List<SelectLocationDecision>();
                        //Select a deck
                        coroutine = base.GameController.SelectADeck(base.HeroTurnTakerController, SelectionType.RevealTopCardOfDeck, (Location deck) => true, storedResults, cardSource: base.GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }

                        if (DidSelectDeck(storedResults))
                        {
                            Location selectedDeck = GetSelectedLocation(storedResults);
                            //put its top card into play.
                            coroutine = base.GameController.PlayTopCardOfLocation(base.TurnTakerController, selectedDeck, isPutIntoPlay: true, cardSource: base.GetCardSource());
                            if (base.UseUnityCoroutines)
                            {
                                yield return base.GameController.StartCoroutine(coroutine);
                            }
                            else
                            {
                                base.GameController.ExhaustCoroutine(coroutine);
                            }
                        }
                        break;
                    }
                case 1:
                    {
                        //Discard the top 2 cards of the villain deck.
                        List<SelectLocationDecision> storedVillain = new List<SelectLocationDecision>();
                        IEnumerator coroutine4 = FindVillainDeck(DecisionMaker, SelectionType.DiscardFromDeck, storedVillain, (Location l) => true);
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine4);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine4);
                        }
                        Location selectedLocation = GetSelectedLocation(storedVillain);

                        if(selectedLocation != null)
                        {
                            coroutine = base.GameController.DiscardTopCards(base.HeroTurnTakerController, selectedLocation, 2, cardSource: base.GetCardSource());
                            if (base.UseUnityCoroutines)
                            {
                                yield return base.GameController.StartCoroutine(coroutine);
                            }
                            else
                            {
                                base.GameController.ExhaustCoroutine(coroutine);
                            }
                        }
                      
                        break;
                    }
                case 2:
                    {
                        //Shuffle 1 card from a trash back into its deck.
                        SelectCardDecision decision = new SelectCardDecision(base.GameController, base.HeroTurnTakerController, SelectionType.ShuffleCardFromTrashIntoDeck, base.GameController.FindCardsWhere(new LinqCardCriteria((Card c) => c.IsInTrash)), cardSource: base.GetCardSource());
                        coroutine = base.GameController.SelectCardAndDoAction(decision, (SelectCardDecision selectedCard) => base.GameController.MoveCard(base.TurnTakerController, selectedCard.SelectedCard, selectedCard.SelectedCard.NativeDeck, shuffledTrashIntoDeck: true, cardSource: base.GetCardSource()));
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        break;
                    }
            }
            yield break;
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //Reveral the top card of a hero deck. You may discard a card to put it into play, otherwise put it into that player's hand.
            List<SelectLocationDecision> storedResults = new List<SelectLocationDecision>();
            IEnumerator coroutine = base.GameController.SelectADeck(base.HeroTurnTakerController, SelectionType.RevealTopCardOfDeck, (Location deck) => deck.IsHero, storedResults, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (DidSelectDeck(storedResults))
            {
                Location selectedDeck = GetSelectedLocation(storedResults);
                List<RevealCardsAction> revealedCards = new List<RevealCardsAction>();
                //Reveal the top card of a hero deck.
                coroutine = base.GameController.RevealCards(base.TurnTakerController, selectedDeck, 1, null,  revealedCardDisplay: RevealedCardDisplay.ShowRevealedCards, storedResultsAction: revealedCards, cardSource: base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                if(!revealedCards.Any())
                {
                    yield break;
                }

                RevealCardsAction revealAction = revealedCards.First();

                if(!revealAction.RevealedCards.Any() && !revealAction.RemovedFromRevealedCards.Any())
                {
                    yield break;
                }

                Card revealedCard = revealAction.RevealedCards.Any() ? revealAction.RevealedCards.First() : revealAction.RemovedFromRevealedCards.First();

                List<DiscardCardAction> discardResults = new List<DiscardCardAction>();
                //You may discard a card...
                coroutine = base.GameController.SelectAndDiscardCard(base.HeroTurnTakerController, true, storedResults: discardResults, selectionType: SelectionType.DiscardCard, cardSource: base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                if (DidDiscardCards(discardResults))
                {
                    //...to put it into play...
                    coroutine = GameController.MoveCard(TurnTakerController, revealedCard, revealedCard.Owner.PlayArea, isPutIntoPlay: true, cardSource: GetCardSource());
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
                    //...otherwise put it into that player's hand.
                    coroutine = base.GameController.MoveCard(base.TurnTakerController, revealedCard, revealedCard.Owner.ToHero().Hand, cardSource: base.GetCardSource());
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