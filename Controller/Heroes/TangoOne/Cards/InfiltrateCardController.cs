using System;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TangoOne
{
    public class InfiltrateCardController : TangoOneBaseCardController
    {
        //==============================================================
        // Reveal the top 2 cards of any deck, then replace them in any order.
        // You may draw a card. You may play a card.
        //==============================================================

        public static string Identifier = "Infiltrate";

        private const int CardsToReveal = 2;

        public InfiltrateCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            // Select deck
            List<SelectLocationDecision> locationResults = new List<SelectLocationDecision>();
            IEnumerator selectDeckRoutine = base.GameController.SelectADeck(this.DecisionMaker, SelectionType.RevealCardsFromDeck,
                location => location.IsDeck, locationResults);

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(selectDeckRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(selectDeckRoutine);
            }

            // Reveal the top 2 cards, then replace them in any order
            List<Card> revealedCards = new List<Card>();
            Location deck = base.GetSelectedLocation(locationResults);

            if (deck != null)
            {
                IEnumerator routine = base.RevealCardsFromTopOfDeck_DetermineTheirLocation(this.DecisionMaker, base.TurnTakerController, deck, 
                    new MoveCardDestination(deck), 
                    new MoveCardDestination(deck),
                    CardsToReveal, CardsToReveal, CardsToReveal, 
                    base.TurnTaker, revealedCards);

                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(routine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(routine);
                }
            }

            // You may draw a card, You may play a card
            IEnumerator drawCardRoutine = base.DrawCard(base.HeroTurnTaker, true);
            IEnumerator playCardRoutine = this.SelectAndPlayCardFromHand(this.HeroTurnTakerController, true, 
                null, null, true);

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(drawCardRoutine);
                yield return base.GameController.StartCoroutine(playCardRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(drawCardRoutine);
                base.GameController.ExhaustCoroutine(playCardRoutine);
            }
        }
    }
}