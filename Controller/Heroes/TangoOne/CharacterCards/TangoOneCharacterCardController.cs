using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TangoOne
{
    public class TangoOneCharacterCardController : HeroCharacterCardController
    {
        private const int PowerDamageAmount = 1;
        private const int Incapacitate3OngoingCardCount = 2;

        public TangoOneCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card,
            turnTakerController)
        {

        }

        public override IEnumerator UsePower(int index = 0)
        {
            //==============================================================
            // {TangoOne} deals 1 target 1 projectile damage.
            //==============================================================

            DamageSource damageSource = new DamageSource(base.GameController, base.CharacterCard);
            int powerNumeral = base.GetPowerNumeral(0, PowerDamageAmount);

            IEnumerator routine = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, damageSource,
                powerNumeral,
                DamageType.Projectile, new int?(1), false, new int?(1),
                cardSource: base.GetCardSource(null));

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            switch (index)
            {
                case 0:

                    //==============================================================
                    // One player may draw a card now.
                    //==============================================================

                    IEnumerator drawCardRoutine = base.GameController.SelectHeroToDrawCard(this.HeroTurnTakerController);

                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(drawCardRoutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(drawCardRoutine);
                    }

                    break;

                case 1:

                    //==============================================================
                    // Reveal the top card of a deck, then replace it or discard it.
                    //==============================================================

                    // Select deck
                    List<SelectLocationDecision> locationResults = new List<SelectLocationDecision>();
                    IEnumerator selectDeckRoutine = base.GameController.SelectADeck(this.DecisionMaker, SelectionType.RevealTopCardOfDeck,
                        location => location.IsDeck && !location.OwnerTurnTaker.IsIncapacitatedOrOutOfGame, locationResults);

                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(selectDeckRoutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(selectDeckRoutine);
                    }

                    // Reveal the top card, then replace or discard
                    Location deck = base.GetSelectedLocation(locationResults);

                    if (deck == null)
                    {
                        yield break;
                    }

                    IEnumerator cardActionRoutine = this.RevealCard_PutItBackOrDiscardIt(base.TurnTakerController, deck, null, null,
                        true, base.TurnTaker, true);

                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(cardActionRoutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(cardActionRoutine);
                    }

                    break;

                case 2:

                    //==============================================================
                    // Up to 2 ongoing hero cards may be played now.
                    //==============================================================

                    // Choose up to 2 cards
                    List<SelectCardsDecision> storedCardResults = new List<SelectCardsDecision>();
                    
                    IEnumerator selectCardsRoutine 
                        = base.GameController.SelectCardsAndStoreResults(base.HeroTurnTakerController, 
                            SelectionType.PlayCard, c => c.IsInHand && c.Location.IsHero && c.IsOngoing, Incapacitate3OngoingCardCount, 
                            storedCardResults, true, 0);
                    
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(selectCardsRoutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(selectCardsRoutine);
                    }
                    
                    if (storedCardResults[0].NumberOfCards > 0)
                    {
                        // Play each selected card
                        foreach (SelectCardDecision scd in storedCardResults[0].SelectCardDecisions)
                        {
                            Card selectedCard = scd.SelectedCard;

                            HeroTurnTakerController hero = (base.FindTurnTakerController(selectedCard.Location.OwnerTurnTaker)).ToHero();
                            
                            IEnumerator selectCardRoutine = this.SelectAndPlayCardFromHand(hero, false, null, 
                                new LinqCardCriteria(c => c == selectedCard), true);

                            if (base.UseUnityCoroutines)
                            {
                                yield return base.GameController.StartCoroutine(selectCardRoutine);
                            }
                            else
                            {
                                base.GameController.ExhaustCoroutine(selectCardRoutine);
                            }
                        }
                    }

                    break;
            }
        }


        private IEnumerator RevealCard_PutItBackOrDiscardIt(TurnTakerController revealingTurnTaker, Location deck, 
            LinqCardCriteria autoPlayCriteria = null, List<MoveCardAction> storedResults = null, bool showRevealedCards = true, 
            TurnTaker responsibleTurnTaker = null, bool isDiscard = true)
        {
            RevealedCardDisplay revealedCardDisplay = RevealedCardDisplay.None;
            if (showRevealedCards)
            {
                revealedCardDisplay = RevealedCardDisplay.Message;
            }

            List<Card> revealedCards = new List<Card>();
            IEnumerator revealCardRoutine = this.GameController.RevealCards(revealingTurnTaker, deck, 1, revealedCards, false, 
                revealedCardDisplay, null, this.GetCardSource(null));

            if (this.UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(revealCardRoutine);
            }
            else
            {
                this.GameController.ExhaustCoroutine(revealCardRoutine);
            }

            if (responsibleTurnTaker == null)
            {
                responsibleTurnTaker = this.TurnTaker;
            }

            if (!revealedCards.Any())
            {
                yield break;
            }

            Card card = revealedCards.First<Card>();
            CardController cardController = this.FindCardController(card);
            TurnTaker ownerTurnTaker = deck.OwnerTurnTaker;

            MoveCardDestination[] possibleDestinations = new[]
            {
                new MoveCardDestination(card.Owner.Deck, false, false, false),
                new MoveCardDestination(cardController.GetTrashDestination(), false, false, false)
            };
                
            IEnumerator setLocationAndMoveRoutine = this.GameController.SelectLocationAndMoveCard(this.HeroTurnTakerController, card, 
                possibleDestinations, false, true, null, null, storedResults, false, false, responsibleTurnTaker, isDiscard, this.GetCardSource(null));
                
            if (this.UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(setLocationAndMoveRoutine);
            }
            else
            {
                this.GameController.ExhaustCoroutine(setLocationAndMoveRoutine);
            }


            IEnumerator cleanupRoutine = this.CleanupCardsAtLocations(new List<Location>
            {
                deck.OwnerTurnTaker.Revealed
            }, deck, false, true, false, false, false, true, revealedCards);
                
            if (this.UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(cleanupRoutine);
            }
            else
            {
                this.GameController.ExhaustCoroutine(cleanupRoutine);
            }
        }
    }
}
