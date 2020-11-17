using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Vanish
{
    public class VanishCharacterCardController : HeroCharacterCardController
    {
        public VanishCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
        public override IEnumerator UsePower(int index = 0)
        {
            List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
            var coroutine = base.GameController.SelectCardAndStoreResults(this.DecisionMaker, SelectionType.SelectTarget, new LinqCardCriteria((Card c) => c.IsInPlayAndHasGameText && c.IsTarget, "target to deal damage", false), storedResults, false, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if (DidSelectCard(storedResults))
            {
                Card selectedCard = base.GetSelectedCard(storedResults);
                coroutine = GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(GameController, selectedCard), 1, DamageType.Projectile, 1, false, 1, cardSource: GetCardSource());
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
        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            switch (index)
            {
                case 0:
                    {
                        //One player may draw a card now.
                        IEnumerator coroutine = base.GameController.SelectHeroToDrawCard(this.DecisionMaker, cardSource: base.GetCardSource());
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
                        //Reveal the bottom card of a deck, then replace it or move it to the top.
                        List<SelectLocationDecision> storedResults = new List<SelectLocationDecision>();
                        IEnumerator coroutine = base.GameController.SelectADeck(this.DecisionMaker, SelectionType.RevealBottomCardOfDeck, (Location l) => l.IsDeck && !l.OwnerTurnTaker.IsIncapacitatedOrOutOfGame, storedResults, cardSource: base.GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }

                        if (DidSelectLocation(storedResults))
                        {
                            var location = GetSelectedLocation(storedResults);
                            List<Card> revealedCards = new List<Card>(); //we already know this is location.BottomCard, but the function demands
                            coroutine = GameController.RevealCards(this.DecisionMaker, location, 1, revealedCards,
                                fromBottom: true,
                                revealedCardDisplay: RevealedCardDisplay.ShowRevealedCards,
                                cardSource: GetCardSource());
                            if (base.UseUnityCoroutines)
                            {
                                yield return base.GameController.StartCoroutine(coroutine);
                            }
                            else
                            {
                                base.GameController.ExhaustCoroutine(coroutine);
                            }

                            var card = revealedCards.First();
                            var locations = new[]
                            {
                                new MoveCardDestination(card.Owner.Deck, true, false),
                                new MoveCardDestination(card.Owner.Deck, false, true)
                            };

                            coroutine = GameController.SelectLocationAndMoveCard(this.DecisionMaker, card, locations, cardSource: GetCardSource());
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
                        //Select a hero target. Reduce damage dealt to that target by 1 till the start of your next turn"
                        List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
                        var coroutine = base.GameController.SelectCardAndStoreResults(this.DecisionMaker, SelectionType.SelectTargetFriendly, new LinqCardCriteria(c => c.IsInPlayAndHasGameText && c.IsTarget && c.IsHero, "hero target", false), storedResults, false, cardSource: base.GetCardSource());
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }
                        if (DidSelectCard(storedResults))
                        {
                            Card selectedCard = GetSelectedCard(storedResults);
                            ReduceDamageStatusEffect reduceDamageStatusEffect = new ReduceDamageStatusEffect(1);
                            reduceDamageStatusEffect.TargetCriteria.IsSpecificCard = selectedCard;
                            reduceDamageStatusEffect.UntilStartOfNextTurn(base.TurnTaker);
                            coroutine = base.AddStatusEffect(reduceDamageStatusEffect, true);
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
            }
            yield break;
        }

        private bool IsRune(Card card)
        {
            return card != null && base.GameController.DoesCardContainKeyword(card, "rune", false, false);
        }

        protected IEnumerator RevealCard_PutItBackOrDiscardIt(TurnTakerController revealingTurnTaker, Location deck, LinqCardCriteria autoPlayCriteria = null, List<MoveCardAction> storedResults = null, bool showRevealedCards = true, TurnTaker responsibleTurnTaker = null, bool isDiscard = true)
        {
            RevealedCardDisplay revealedCardDisplay = RevealedCardDisplay.None;
            if (showRevealedCards)
            {
                revealedCardDisplay = RevealedCardDisplay.Message;
            }
            List<Card> revealedCards = new List<Card>();
            IEnumerator coroutine = this.GameController.RevealCards(revealingTurnTaker, deck, 1, revealedCards, false, revealedCardDisplay, null, this.GetCardSource(null));
            if (this.UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(coroutine);
            }
            else
            {
                this.GameController.ExhaustCoroutine(coroutine);
            }
            if (responsibleTurnTaker == null)
            {
                responsibleTurnTaker = this.TurnTaker;
            }
            if (revealedCards.Count<Card>() > 0)
            {
                Card card = revealedCards.First<Card>();
                CardController cardController = this.FindCardController(card);
                TurnTaker ownerTurnTaker = deck.OwnerTurnTaker;


                MoveCardDestination[] possibleDestinations = new MoveCardDestination[]
                {
                new MoveCardDestination(card.Owner.Deck, false, false, false),
                new MoveCardDestination(cardController.GetTrashDestination(), false, false, false)
                };
                IEnumerator coroutine4 = this.GameController.SelectLocationAndMoveCard(this.HeroTurnTakerController, card, possibleDestinations, false, true, null, null, storedResults, false, false, responsibleTurnTaker, isDiscard, this.GetCardSource(null));
                if (this.UseUnityCoroutines)
                {
                    yield return this.GameController.StartCoroutine(coroutine4);
                }
                else
                {
                    this.GameController.ExhaustCoroutine(coroutine4);
                }


                IEnumerator coroutine6 = this.CleanupCardsAtLocations(new List<Location>
            {
                deck.OwnerTurnTaker.Revealed
            }, deck, false, true, false, false, false, true, revealedCards);
                if (this.UseUnityCoroutines)
                {
                    yield return this.GameController.StartCoroutine(coroutine6);
                }
                else
                {
                    this.GameController.ExhaustCoroutine(coroutine6);
                }
                yield break;
            }
        }
    }
}
