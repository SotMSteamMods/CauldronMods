using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Northspar
{
    public class LandingSiteCardController : ThirdWaypointCardController
    {

        public LandingSiteCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, "DemolishedCamp")
        {
            SpecialStringMaker.ShowIfSpecificCardIsInPlay("TakAhab");
        }

        public override void AddTriggers()
        {
            //At the start of the environment turn, if Tak Ahab is in play, you may destroy 1 hero ongoing or equipment card. If you do, place the top card of any deck beneath Tak Ahab.
            base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.StartOfTurnResponse, new TriggerType[]
                {
                    TriggerType.DestroyCard,
                    TriggerType.MoveCard
                }, (PhaseChangeAction pca) => base.IsTakAhabInPlay());

            //add all ThirdWaypoint triggers
            base.AddTriggers();
        }

        private bool HeroEquipmentOrOngoingQuery(Card c)
        {
            return c.IsInPlayAndHasGameText && IsHero(c) && (IsOngoing(c) || base.IsEquipment(c)) && GameController.IsCardVisibleToCardSource(c, GetCardSource());
        }

        private IEnumerator StartOfTurnResponse(PhaseChangeAction pca)
        {
            if (FindCardsWhere(HeroEquipmentOrOngoingQuery).Any())
            {
                /// ...you may destroy 1 hero ongoing or equipment card...
                List<DestroyCardAction> storedResults = new List<DestroyCardAction>();
                LinqCardCriteria criteria = new LinqCardCriteria(HeroEquipmentOrOngoingQuery, "hero ongoing or equipment");
                IEnumerator coroutine = base.GameController.SelectAndDestroyCard(base.DecisionMaker, criteria, true, storedResultsAction: storedResults, cardSource: base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                // ...if you do, place the top card of any deck beneath Tak Ahab
                if (storedResults != null && storedResults.Count() > 0 &&  DidDestroyCard(storedResults.First()))
                {
                    List<SelectLocationDecision> selectDeckResults = new List<SelectLocationDecision>();
                    coroutine = base.GameController.SelectADeck(base.DecisionMaker, SelectionType.MoveCardToUnderCard, (Location l) => true, selectDeckResults, cardSource: base.GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                    if (selectDeckResults != null && base.DidSelectDeck(selectDeckResults))
                    {
                        Location deck = selectDeckResults.First().SelectedLocation.Location;
                        coroutine = base.GameController.MoveCard(base.DecisionMaker, deck.TopCard, base.FindTakAhabInPlay().UnderLocation, showMessage: true, cardSource: base.GetCardSource());
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
            else
            {
                IEnumerator coroutine2 = base.GameController.SendMessageAction(base.Card.Title + " has no hero ongoing or equipment to destroy.", Priority.Medium, GetCardSource(), null, showCardSource: true);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine2);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine2);
                }
            }

            yield break;
        }
    }
}