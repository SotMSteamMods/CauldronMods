using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Handelabra;

namespace Cauldron.TangoOne
{
    public class TangoOneCharacterCardController : HeroCharacterCardController
    {
        private const int PowerDamageAmount = 1;
        private const int PowerTargetAmount = 1;
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
            int targetsNumeral = base.GetPowerNumeral(0, PowerTargetAmount);
            int damageNumeral = base.GetPowerNumeral(1, PowerDamageAmount);

            IEnumerator routine = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, damageSource,
                damageNumeral,
                DamageType.Projectile, targetsNumeral, false, targetsNumeral,
                cardSource: base.GetCardSource());

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

                    IEnumerator drawCardRoutine = base.GameController.SelectHeroToDrawCard(DecisionMaker, cardSource: GetCardSource());
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
                        location => location.IsDeck && !location.OwnerTurnTaker.IsIncapacitatedOrOutOfGame, locationResults, cardSource: GetCardSource());
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
                        true, base.TurnTaker);

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

                    
                    //Play first ongoing
                    var storedResults = new List<SelectCardsDecision> { };
                    Func<Card, bool> playableOngoingInHand = (delegate (Card c)
                    {
                        return c.IsInHand && IsHero(c) && IsOngoing(c) &&
                                  AskIfCardIsVisibleToCardSource(c, GetCardSource()) != false &&
                                  GameController.CanPlayCard(FindCardController(c)) == CanPlayCardResult.CanPlay;
                    });
                    IEnumerator coroutine = GameController.SelectCardsAndStoreResults(DecisionMaker, SelectionType.PlayCard, playableOngoingInHand, 2, storedResults, false, 0, cardSource: GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }

                    var cards = GetSelectedCards(storedResults);
                    foreach(var card in cards)
                    {
                        coroutine = GameController.PlayCard(FindTurnTakerController(card.Owner), card, responsibleTurnTaker: TurnTaker, cardSource: GetCardSource());
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
                revealedCardDisplay, null, this.GetCardSource());

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

            MoveCardDestination[] possibleDestinations = {
                new MoveCardDestination(deck),
                new MoveCardDestination(cardController.GetTrashDestination().Location)
            };

            IEnumerator setLocationAndMoveRoutine = this.GameController.SelectLocationAndMoveCard(this.HeroTurnTakerController, card,
                possibleDestinations, false, true, null, null, storedResults, false, false, responsibleTurnTaker, isDiscard, this.GetCardSource());

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
