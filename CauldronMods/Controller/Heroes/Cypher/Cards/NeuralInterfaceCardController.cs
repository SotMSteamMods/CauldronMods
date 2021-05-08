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
            ShowSpecialStringAugmentsInPlay();
        }

        private int customTextPowerNumeral = 1;
        public override IEnumerator UsePower(int index = 0)
        {
            int augsToMove = GetPowerNumeral(0, 1);
            customTextPowerNumeral = augsToMove;
            // You may move 1 Augment in play next to a new hero.
            var scd = new SelectCardsDecision(GameController, DecisionMaker, (Card c) => IsInPlayAugment(c), SelectionType.Custom,
                            numberOfCards: augsToMove,
                            isOptional: false,
                            requiredDecisions: 0,
                            eliminateOptions: true,
                            cardSource: GetCardSource());

            IEnumerator routine = base.GameController.SelectCardsAndDoAction(scd, MoveInPlayAugment);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }

            // Draw 2 cards.
            int cardsToDrawNumeral = GetPowerNumeral(1, CardsToDraw);
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
            routine = base.GameController.SelectAndDiscardCards(this.HeroTurnTakerController, CardsToDiscard, false,
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

        public override CustomDecisionText GetCustomDecisionText(IDecision decision)
        {
            string augment = customTextPowerNumeral == 1 ? "an Augment" : customTextPowerNumeral + " Augments";
            string heroes = customTextPowerNumeral == 1 ? "a new hero" : "new heroes";

            return new CustomDecisionText($"Select {augment} in play to move next to {heroes}.", $"Selecting {augment} in play to move next to {heroes}.", $"Vote for {augment} in play to move next to {heroes}.", $"move {augment} in play next to {heroes}");

        }
    }
}