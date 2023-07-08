using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Drift
{
    public class ThroughTheBreachDriftCharacterCardController : ThroughTheBreachSubCharacterCardController
    {
        public ThroughTheBreachDriftCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowIfElseSpecialString(() => this.GetBreachedCard(1) != null, () => "The card at position 1 is " + this.GetBreachedCard(1).Title, () => "There is no card at position 1");
            base.SpecialStringMaker.ShowIfElseSpecialString(() => this.GetBreachedCard(2) != null, () => "The card at position 2 is " + this.GetBreachedCard(2).Title, () => "There is no card at position 2");
            base.SpecialStringMaker.ShowIfElseSpecialString(() => this.GetBreachedCard(3) != null, () => "The card at position 3 is " + this.GetBreachedCard(3).Title, () => "There is no card at position 3");
            base.SpecialStringMaker.ShowIfElseSpecialString(() => this.GetBreachedCard(4) != null, () => "The card at position 4 is " + this.GetBreachedCard(4).Title, () => "There is no card at position 4");
        }

        protected const string BreachPosition = "BreachPosition";

        public override IEnumerator UsePower(int index = 0)
        {

            int cardNumeral = base.GetPowerNumeral(0, 2);

            //Add the top 2 cards of your deck to your shift track, or discard the card from your current shift track space.
            IEnumerator coroutine = base.SelectAndPerformFunction(base.HeroTurnTakerController, new Function[] {
                    new Function(base.HeroTurnTakerController, "Add the top 2 cards of your deck to your shift track", SelectionType.MoveCard, () => this.AddCardsResponse(cardNumeral)),
                    new Function(base.HeroTurnTakerController, "discard the card from your current shift track space", SelectionType.DiscardCard, () => this.DiscardCardResponse(), onlyDisplayIfTrue: this.GetBreachedCard(base.CurrentShiftPosition()) != null)
            });
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

        private IEnumerator AddCardsResponse(int cardNumeral)
        {
            //Cards added to your shift track are placed face up next to 1 of its 4 spaces. Each space may only have 1 card next to it. They are not considered in play.
            //Add the top 2 cards of your deck to your shift track...
            for (int i = 0; i < cardNumeral; i++)
            {
                //Reveal card
                List<Card> revealedCards = new List<Card>();
                IEnumerator coroutine = base.GameController.RevealCards(base.TurnTakerController, base.TurnTaker.Deck, 1, revealedCards, revealedCardDisplay: RevealedCardDisplay.ShowRevealedCards, cardSource: base.GetCardSource());
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
                    //Pick position
                    List<SelectNumberDecision> numberDecisions = new List<SelectNumberDecision>();
                    coroutine = PickPosition(numberDecisions);
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                    if (numberDecisions.Any())
                    {
                        //Move card

                        while(numberDecisions.FirstOrDefault().SelectedNumber.Value == 0)
                        {
                            numberDecisions.Clear();
                            coroutine = GameController.SendMessageAction("Please pick a valid position!", Priority.High, GetCardSource());
                            if (base.UseUnityCoroutines)
                            {
                                yield return base.GameController.StartCoroutine(coroutine);
                            }
                            else
                            {
                                base.GameController.ExhaustCoroutine(coroutine);
                            }
                            coroutine = PickPosition(numberDecisions);
                            if (base.UseUnityCoroutines)
                            {
                                yield return base.GameController.StartCoroutine(coroutine);
                            }
                            else
                            {
                                base.GameController.ExhaustCoroutine(coroutine);
                            }
                        }

                        Card cardToMove = revealedCards.FirstOrDefault();
                        int cardPosition = numberDecisions.FirstOrDefault().SelectedNumber.Value;
                        coroutine = base.GameController.MoveCard(base.TurnTakerController, cardToMove, base.GetPositionalShiftTrack(cardPosition).UnderLocation, cardSource: base.GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }

                        //Mark card position
                        base.SetCardPropertyToTrueIfRealAction(BreachPosition + cardPosition, cardToMove);
                    }
                    else
                    {
                        coroutine = base.GameController.SendMessageAction("There were no open positions", Priority.Medium, base.GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }

                        coroutine = base.CleanupRevealedCards(base.TurnTaker.Revealed, base.TurnTaker.Trash);
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
                else
                {
                    coroutine = base.GameController.SendMessageAction("There were no cards in the deck", Priority.Medium, base.GetCardSource());
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

        private IEnumerator PickPosition(List<SelectNumberDecision> numberDecisions)
        {
            int available = 4;
            for(int i=1; i<=4; i++)
            {
                if(this.GetBreachedCard(i) != null)
                {
                    available--;
                }
            }
            if(available == 0)
            {
                yield break;
            }
            IEnumerator coroutine = base.GameController.SelectNumber(base.HeroTurnTakerController, SelectionType.MoveCard, 1, 4, additionalCriteria: (int position) => this.GetBreachedCard(position) == null, storedResults: numberDecisions, cardSource: base.GetCardSource());
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

        private IEnumerator DiscardCardResponse()
        {
            //...discard the card from your current shift track space.
            Card cardToDiscard = this.GetBreachedCard(base.CurrentShiftPosition());
            IEnumerator coroutine = base.GameController.MoveCard(base.TurnTakerController, cardToDiscard, base.TurnTaker.Trash, isDiscard: true, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            base.FindCardController(cardToDiscard).SetCardProperty(BreachPosition + this.CurrentShiftPosition(), false);

            //When you discard a card from the track, you may play it or {Drift} may deal 1 target 3 radiant damage.
            coroutine = base.SelectAndPerformFunction(base.HeroTurnTakerController, new Function[] {
                    new Function(base.HeroTurnTakerController, "you may play the discarded card", SelectionType.AddTokens, () => base.GameController.PlayCard(base.TurnTakerController, cardToDiscard, optional: true, cardSource: base.GetCardSource())),
                    new Function(base.HeroTurnTakerController, "Drift may deal 1 target 3 radiant damage", SelectionType.AddTokens, () => base.GameController.SelectTargetsAndDealDamage(base.HeroTurnTakerController, new DamageSource(base.GameController, base.CharacterCard), 3, DamageType.Radiant, 1, false, 0, cardSource:base.GetCardSource()))
            }, associatedCards: cardToDiscard.ToEnumerable());
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

        private Card GetBreachedCard(int position)
        {
            IEnumerable<CardPropertiesJournalEntry> entries = base.Journal.CardPropertiesEntries((CardPropertiesJournalEntry entry) => entry.Key == BreachPosition + position && entry.BoolValue.Value);
            foreach (CardPropertiesJournalEntry entry in entries)
            {
                if (base.GameController.GetCardPropertyJournalEntryBoolean(entry.Card, BreachPosition + position) ?? false)
                {
                    return entry.Card;
                }
            }
            return null;
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator coroutine;
            switch (index)
            {
                case 0:
                    {
                        //One hero may draw a card now.
                        coroutine = base.GameController.SelectHeroToDrawCard(base.HeroTurnTakerController, additionalCriteria: new LinqTurnTakerCriteria((TurnTaker tt) => IsHero(tt) && !tt.IsIncapacitatedOrOutOfGame && !tt.IsIncapacitated), cardSource: base.GetCardSource());
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
                        //Two heroes may use a power now. If those powers deal damage, reduce that damage by 1.
                        List<SelectCardDecision> selectedHero = new List<SelectCardDecision>();
                        coroutine = base.GameController.SelectHeroCharacterCard(base.HeroTurnTakerController, SelectionType.UsePower, selectedHero, cardSource: base.GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }

                        if(!DidSelectCard(selectedHero))
                        {
                            yield break;
                        }

                        //First Hero
                        coroutine = base.SelectHeroToUsePowerAndModifyIfDealsDamage(base.HeroTurnTakerController, (Func<DealDamageAction, bool> c) => base.AddReduceDamageTrigger((Card card) => true, 1), -1, additionalCriteria: new LinqTurnTakerCriteria((TurnTaker tt) => tt == GetSelectedCard(selectedHero).Owner));
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }

                        //Second Hero
                        coroutine = base.SelectHeroToUsePowerAndModifyIfDealsDamage(base.HeroTurnTakerController, (Func<DealDamageAction, bool> c) => base.AddReduceDamageTrigger((Card card) => true, 1), -1, additionalCriteria: new LinqTurnTakerCriteria((TurnTaker tt) => tt != GetSelectedCard(selectedHero).Owner));
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
                case 2:
                    {
                        //Move 1 environment card from play to the of its deck.
                        List<SelectCardDecision> cardDecision = new List<SelectCardDecision>();
                        coroutine = base.GameController.SelectCardAndStoreResults(base.HeroTurnTakerController, SelectionType.ReturnToDeck, new LinqCardCriteria((Card c) => c.IsEnvironment && c.IsInPlayAndHasGameText, "environment"), cardDecision, false, cardSource: base.GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }

                        if(!DidSelectCard(cardDecision))
                        {
                            yield break;
                        }

                        Card selectedCard = GetSelectedCard(cardDecision);
                        coroutine = base.GameController.MoveCard(base.TurnTakerController, selectedCard, selectedCard.NativeDeck, cardSource: base.GetCardSource());
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
        }
    }
}
