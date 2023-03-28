using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.HalberdExperimentalResearchCenter
{
    public class HalberdProphetCardController : TestSubjectCardController
    {
        #region Constructors

        public HalberdProphetCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        #endregion Constructors

        #region Methods
        public override void AddTriggers()
        {
            //At the end of the environment turn, if there are no Chemical Triggers in play, each player may look at the top card of their deck, and put it back on either the top or bottom of their deck.
            //Otherwise, play the top card of the villain deck.
            base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(this.EndOfTurnResponse), TriggerType.DealDamage);
        }

        private IEnumerator EndOfTurnResponse(PhaseChangeAction pca)
        {
            //if there are no Chemical Triggers in play...
            if (!base.IsChemicalTriggerInPlay())
            {
                //each player may look at the top card of their deck, and put it back on either the top or bottom of their deck
                IEnumerator coroutine = GameController.SelectTurnTakersAndDoAction(DecisionMaker, new LinqTurnTakerCriteria(tt => IsHero(tt) && !tt.IsIncapacitatedOrOutOfGame && tt.Deck.NumberOfCards > 1),
                                            SelectionType.RevealTopCardOfDeck,
                                            tt => RevealCardsResponse(tt),
                                            requiredDecisions: 0,
                                            allowAutoDecide: true,
                                            cardSource: GetCardSource());


                //IEnumerator coroutine = base.DoActionToEachTurnTakerInTurnOrder((TurnTakerController ttc) => IsHero(ttc.TurnTaker) && !ttc.IsIncapacitatedOrOutOfGame && ttc.TurnTaker.Deck.HasCards, (TurnTakerController ttc) => this.RevealCardsResponse(ttc));
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
                //Otherwise, play the top card of the villain deck.
                IEnumerator coroutine2 = base.PlayTheTopCardOfTheVillainDeckWithMessageResponse(null);
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

        private IEnumerator RevealCardsResponse(TurnTaker tt)
        {
            //get this turntaker's deck
            Location deck = tt.Deck;
            HeroTurnTakerController httc = base.FindHeroTurnTakerController(tt.ToHero());
            List<Card> storedResultsCard = new List<Card>();
            //reveal the top card of the deck
            IEnumerator coroutine = base.GameController.RevealCards(httc, deck, 1, storedResultsCard, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            Card card = storedResultsCard.FirstOrDefault<Card>();
            if (card != null)
            {
                //turntakercontroller decides whether to put on top or bottom of deck
                List<MoveCardDestination> list = new List<MoveCardDestination>()
                { 
                    //top of deck
                    new MoveCardDestination(deck),
                    //bottom of deck
                    new MoveCardDestination(deck, true),
                };
                coroutine = base.GameController.SelectLocationAndMoveCard(httc, card, list, cardSource: base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            //clean up any revealed cards that should not be in play any more
            coroutine = base.CleanupCardsAtLocations(new List<Location> { deck.OwnerTurnTaker.Revealed }, deck, cardsInList: storedResultsCard);
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
        #endregion Methods
    }
}