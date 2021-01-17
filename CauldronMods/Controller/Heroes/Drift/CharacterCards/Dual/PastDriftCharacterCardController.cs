using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Drift
{
    public class PastDriftCharacterCardController : DualDriftSubCharacterCardController
    {
        public PastDriftCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator UsePower(int index = 0)
        {
            int cardNumeral = base.GetPowerNumeral(0, 2);
            int deckNumeral = base.GetPowerNumeral(1, 1);

            IEnumerator coroutine;
            //Reveal the top 2 cards of 1 hero deck. Replace or discard each of them in any order.
            for (int i = 0; i < deckNumeral; i++)
            {
                //...1 hero deck.
                List<SelectLocationDecision> storedResults = new List<SelectLocationDecision>();
                coroutine = base.GameController.SelectADeck(base.HeroTurnTakerController, SelectionType.RevealTopCardOfDeck, (Location deck) => deck.IsDeck && deck.IsHero, storedResults, cardSource: base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                if (storedResults.Any())
                {
                    //Reveal the top 2 cards...
                    List<Card> revealedCards = new List<Card>();
                    Location selectedDeck = storedResults.FirstOrDefault().SelectedLocation.Location;
                    coroutine = base.GameController.RevealCards(base.TurnTakerController, selectedDeck, cardNumeral, revealedCards, revealedCardDisplay: RevealedCardDisplay.ShowRevealedCards, cardSource: base.GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }

                    if (revealedCards.Any())
                    {
                        Card selectedCard = null;
                        //Replace or discard each of them in any order.
                        if (revealedCards.Count() == 1)
                        {
                            coroutine = base.GameController.SendMessageAction("There was only 1 card to reveal.", Priority.High, base.GetCardSource(), revealedCards);
                            selectedCard = revealedCards.FirstOrDefault();
                        }
                        else
                        {
                            List<SelectCardDecision> storedResultsCards = new List<SelectCardDecision>();
                            coroutine = base.GameController.SelectCardAndStoreResults(base.HeroTurnTakerController, SelectionType.ReturnToDeck, revealedCards, storedResultsCards, false, cardSource: base.GetCardSource());
                            if (base.UseUnityCoroutines)
                            {
                                yield return base.GameController.StartCoroutine(coroutine);
                            }
                            else
                            {
                                base.GameController.ExhaustCoroutine(coroutine);
                            }
                            selectedCard = base.GetSelectedCard(storedResultsCards);
                        }

                        while (selectedCard != null)
                        {
                            //Replace or discard...
                            MoveCardDestination deck = new MoveCardDestination(selectedCard.Owner.Deck);
                            MoveCardDestination trash = new MoveCardDestination(selectedCard.Owner.Trash);
                            IEnumerable<MoveCardDestination> destinations = new List<MoveCardDestination>() { deck, trash };
                            coroutine = base.GameController.SelectLocationAndMoveCard(base.HeroTurnTakerController, selectedCard, destinations, cardSource: base.GetCardSource());
                            if (base.UseUnityCoroutines)
                            {
                                yield return base.GameController.StartCoroutine(coroutine);
                            }
                            else
                            {
                                base.GameController.ExhaustCoroutine(coroutine);
                            }

                            //Pick a new card
                            List<SelectCardDecision> storedResultsCards = new List<SelectCardDecision>();
                            coroutine = base.GameController.SelectCardAndStoreResults(base.HeroTurnTakerController, SelectionType.ReturnToDeck, revealedCards, storedResultsCards, false, cardSource: base.GetCardSource());
                            if (base.UseUnityCoroutines)
                            {
                                yield return base.GameController.StartCoroutine(coroutine);
                            }
                            else
                            {
                                base.GameController.ExhaustCoroutine(coroutine);
                            }
                            selectedCard = base.GetSelectedCard(storedResultsCards);
                        }
                    }
                }
            }

            //Shift {DriftLL}.
            coroutine = base.ShiftLL();
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

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator coroutine;
            switch (index)
            {
                case 0:
                    {
                        //One player may draw a card now.
                        coroutine = base.GameController.SelectHeroToDrawCard(base.HeroTurnTakerController, cardSource: base.GetCardSource());
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
                case 1:
                    {
                        //Select a card in a hero trash with a power on it. That hero uses that power, then shuffles that card into their deck.
                        IEnumerable<Card> powerCards = base.FindCardsWhere(new LinqCardCriteria((Card c) => c.Location.IsHero && c.Location.IsTrash && c.HasPowers));
                        if (powerCards.Any())
                        {
                            SelectCardDecision powerCardDecision = new SelectCardDecision(base.GameController, base.HeroTurnTakerController, SelectionType.UsePowerOnCard, powerCards, cardSource: base.GetCardSource());
                            coroutine = base.GameController.SelectCardAndDoAction(powerCardDecision, this.UsePowerResponse);
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
                        //Destroy an environment card.
                        coroutine = base.GameController.SelectAndDestroyCard(base.HeroTurnTakerController, new LinqCardCriteria((Card c) => c.IsEnvironment && c.IsInPlayAndHasGameText, "environment"), false, cardSource: base.GetCardSource());
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

        private IEnumerator UsePowerResponse(SelectCardDecision decision)
        {
            //That hero uses that power...
            IEnumerator coroutine;
            int powerIndex = 0;
            Card selectedCard = decision.SelectedCard;
            if (selectedCard.NumberOfPowers > 1)
            {
                coroutine = base.GameController.SelectAndUsePower(base.HeroTurnTakerController, false, (Power p) => p.CardSource.Card == selectedCard, allowOutOfPlayPower: true, cardSource: base.GetCardSource());
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
                coroutine = base.UsePowerOnOtherCard(decision.SelectedCard, powerIndex);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }

            //...then shuffles that card into their deck.
            coroutine = base.GameController.MoveCard(base.TurnTakerController, selectedCard, selectedCard.Owner.Deck, shuffledTrashIntoDeck: true, cardSource: base.GetCardSource());
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
