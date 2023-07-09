using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.DungeonsOfTerror
{
    public class ShopkeeperCardController : DungeonsOfTerrorUtilityCardController
    {
        public ShopkeeperCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            Card.UnderLocation.OverrideIsInPlay = false;
            SpecialStringMaker.ShowNumberOfCardsUnderCard(Card);
        }

        public override void AddTriggers()
        {
            //At the start of their turn, a player may discard a card to play or draw one of their cards from beneath this.
            AddStartOfTurnTrigger((TurnTaker tt) => IsHero(tt) && Card.UnderLocation.Cards.Any(c => c.Owner == tt), DiscardToPlayOrDrawResponse, new TriggerType[]
            {
                TriggerType.DiscardCard,
                TriggerType.PlayCard,
                TriggerType.DrawCard
            });
        }

        private IEnumerator DiscardToPlayOrDrawResponse(PhaseChangeAction pca)
        {
            HeroTurnTakerController httc = FindHeroTurnTakerController(pca.ToPhase.TurnTaker.ToHero());
            IEnumerable<Card> ownedCards = Card.UnderLocation.Cards.Where(c => c.Owner == httc.TurnTaker);
            List<DiscardCardAction> storedResults = new List<DiscardCardAction>() ;
            IEnumerator coroutine = GameController.SelectAndDiscardCard(httc, optional: true, storedResults: storedResults, associatedCards: ownedCards, cardSource: GetCardSource());
            if (this.UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(coroutine);
            }
            else
            {
                this.GameController.ExhaustCoroutine(coroutine);
            }
            if(DidDiscardCards(storedResults))
            {
                List<SelectCardDecision> storedChoice = new List<SelectCardDecision>() ;
                coroutine = GameController.SelectCardAndStoreResults(httc, SelectionType.PlayCard, ownedCards, storedChoice, cardSource: GetCardSource());
                if (this.UseUnityCoroutines)
                {
                    yield return this.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    this.GameController.ExhaustCoroutine(coroutine);
                }
                if(DidSelectCard(storedChoice))
                {
                    Card selectedCard = GetSelectedCard(storedChoice);
                    //op1: play that card
                    var response1 = GameController.PlayCard(httc, selectedCard, cardSource: GetCardSource());
                    var op1 = new Function(httc, $"Play {selectedCard.Title}", SelectionType.PlayCard, () => response1);

                    //op2: draw that card
                    var response2 = GameController.MoveCard(httc, selectedCard, httc.HeroTurnTaker.Hand, cardSource: GetCardSource());
                    var op2 = new Function(httc, $"Draw {selectedCard.Title}", SelectionType.MoveCard, () => response2);

                    //Execute
                    var options = new Function[] { op1, op2, };
                    var selectFunctionDecision = new SelectFunctionDecision(GameController, httc, options, false, cardSource: GetCardSource());
                    coroutine = GameController.SelectAndPerformFunction(selectFunctionDecision);
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

        public override IEnumerator Play()
        {
            //When this card enters play, reveal the top card of each hero deck, and move it beneath this card. Cards beneath this one are not considered in play.
            IEnumerator coroutine = DoActionToEachTurnTakerInTurnOrder((TurnTakerController ttc) => IsHero(ttc.TurnTaker) && !ttc.IsIncapacitatedOrOutOfGame && ttc.TurnTaker.Deck.HasCards, (TurnTakerController ttc) => RevealAndMoveCardsUnderResponse(ttc));
            if (this.UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(coroutine);
            }
            else
            {
                this.GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }

        private IEnumerator RevealAndMoveCardsUnderResponse(TurnTakerController ttc)
        {
            //place the top card of each hero deck beneath this one.
            List<Card> storedResults = new List<Card>(); ;
            IEnumerator coroutine = GameController.RevealCards(ttc, ttc.TurnTaker.Deck, 1, storedResults, revealedCardDisplay: RevealedCardDisplay.ShowRevealedCards, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if(!storedResults.Any())
            {
                yield break;
            }

            coroutine = GameController.MoveCard(DecisionMaker, storedResults.First(), Card.UnderLocation, showMessage: true, cardSource: base.GetCardSource()) ;
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
