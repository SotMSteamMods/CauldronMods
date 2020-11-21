using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Northspar
{
    public class TakAhabCardController : NorthsparCardController
    {

        public TakAhabCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            
        }

        public override void AddTriggers()
        {
            //Whenever this card has no cards beneath it, place the top card of each hero deck beneath this one.
            Func<GameAction, bool> criteria = (GameAction ga) => base.Card.UnderLocation.NumberOfCards == 0 && !this._movingCards;
            base.AddTrigger<GameAction>(criteria, (GameAction ga) =>  base.DoActionToEachTurnTakerInTurnOrder((TurnTakerController ttc) => ttc.IsHero && !ttc.IsIncapacitatedOrOutOfGame && ttc.TurnTaker.Deck.HasCards, 
                (TurnTakerController ttc) => this.MoveCardsUnderResponse(ttc)),
                TriggerType.MoveCard, TriggerTiming.After);

            //At the end of the environment turn, discard a card from beneath this one. This card deals the target from that deck with the highest HP X irreducible sonic damage, where X = the number of Waypoints in play plus 2.
            base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.EndOfTurnResponse, new TriggerType[]
            {
                TriggerType.MoveCard,
                TriggerType.DealDamage
            });

        }

        private IEnumerator EndOfTurnResponse(PhaseChangeAction arg)
        {
            //Discard a card from beneath this one. 
            if (this.FindCardsWhere((Card c) => c.Location == base.Card.UnderLocation).Any((Card c) => this.DoesAnyCardControllerMakeAnotherCardIndestructible(c) == null))
            {

                //have players select a card to move
				List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
				IEnumerable<Card> cardList = from c in this.FindCardsWhere((Card c) => c.Location == base.Card.UnderLocation)
                           orderby c.Owner.Name
                           select c;
                IEnumerator coroutine = this.GameController.SelectCardAndStoreResults(this.DecisionMaker, SelectionType.MoveCardToTrash, cardList, storedResults, maintainCardOrder: true);
				if (this.UseUnityCoroutines)
				{
					yield return this.GameController.StartCoroutine(coroutine);
				}
				else
				{
					this.GameController.ExhaustCoroutine(coroutine);
				}
				
				if (storedResults.Count() > 0)
				{
                    //if a card was selected, move the card
					SelectCardDecision selectCardDecision = (from d in storedResults
															 where d.Completed && d.SelectedCard != null
															 select d).FirstOrDefault();
                    if (selectCardDecision != null)
                    {
                        List<MoveCardAction> storedResultsMove = new List<MoveCardAction>();
                        MoveCardDestination trashDestination = this.FindCardController(selectCardDecision.SelectedCard).GetTrashDestination();
                        coroutine = this.GameController.MoveCard(base.DecisionMaker, selectCardDecision.SelectedCard, trashDestination.Location, trashDestination.ToBottom, showMessage: true, decisionSources: storedResults.CastEnumerable<SelectCardDecision, IDecision>(), storedResults: storedResultsMove, cardSource: base.GetCardSource());
                        if (this.UseUnityCoroutines)
                        {
                            yield return this.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            this.GameController.ExhaustCoroutine(coroutine);
                        }

                        if (this.DidMoveCard(storedResultsMove))
                        {
                            //if the card was moved, this card deals the target from that deck with the highest HP X irreducible sonic damage, where X = the number of Waypoints in play plus 2.
                            Location nativeDeck = storedResultsMove.First().CardToMove.NativeDeck;
                            Func<Card, bool> criteria = (Card c) => c.NativeDeck == nativeDeck && c.IsInPlayAndHasGameText && c.IsTarget;
                            int X = base.GetNumberOfWaypointsInPlay();
                            Func<Card, int?> amount = (Card c) => new int?(X + 2);
                            coroutine = base.DealDamageToHighestHP(base.Card, 1, criteria, amount, DamageType.Sonic, true);
                            if (this.UseUnityCoroutines)
                            {
                                yield return this.GameController.StartCoroutine(coroutine);
                            }
                            else
                            {
                                this.GameController.ExhaustCoroutine(coroutine);
                            }
                        }
                    }
				}
			}
            yield break;
        }

        private IEnumerator MoveCardsUnderResponse(TurnTakerController ttc)
        {
            this._movingCards = true;
            //place the top card of each hero deck beneath this one.
            IEnumerator coroutine = base.GameController.MoveCard(base.DecisionMaker, ttc.TurnTaker.Deck.TopCard, base.Card.UnderLocation, showMessage: true, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            this._movingCards = false;
            yield break;
        }

        private bool _movingCards = false;

    }
}