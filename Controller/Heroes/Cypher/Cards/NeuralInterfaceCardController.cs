using System;
using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Cypher
{
    public class NeuralInterfaceCardController : CypherBaseCardController
    {
        //==============================================================
        // Power: You may move 1 Augment in play next to a new hero.
        // Draw 2 cards. Discard a card
        //==============================================================

        public static string Identifier = "NeuralInterface";

        private const int CardsToDraw = 2;
        private const int CardsToDiscard = 1;

        public NeuralInterfaceCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            // You may move 1 Augment in play next to a new hero.
            SelectCardDecision scd = new SelectCardDecision(GameController, DecisionMaker,
                SelectionType.MoveCardNextToCard, GetAugmentsInPlay(), true, cardSource: GetCardSource());

            IEnumerator routine = base.GameController.SelectCardAndDoAction(scd, MoveAugment);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

            // Draw 2 cards.
            int cardsToDrawNumeral = GetPowerNumeral(0, CardsToDraw);
            routine = base.DrawCards(this.HeroTurnTakerController, cardsToDrawNumeral);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

            // Discard a card
            int cardsToDiscardNumeral = GetPowerNumeral(1, CardsToDiscard);
            routine = base.GameController.SelectAndDiscardCards(this.HeroTurnTakerController, cardsToDiscardNumeral, false,
                CardsToDiscard, cardSource: GetCardSource());
            
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }
        }
    }
}