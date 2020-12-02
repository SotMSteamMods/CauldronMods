using System.Collections;
using System.Collections.Generic;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Dendron
{
    public class InkScarCardController : DendronBaseCardController
    {
        //==============================================================
        // Shuffle all Tattoos from the villain trash into the villain deck.
        // Play the top 2 cards of the villain deck.
        //==============================================================

        public static readonly string Identifier = "InkScar";

        private const int CardsToPlay = 2;

        public InkScarCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsAtLocation(TurnTaker.Trash, new LinqCardCriteria(IsTattoo, "tattoo"));
        }

        public override IEnumerator Play()
        {
            // Shuffle all Tattoos from the villain trash into the villain deck.

            // Move found Tattoo cards from villain trash to villain deck
            List<MoveCardDestination> moveCardDestination = new List<MoveCardDestination>()
            {
                new MoveCardDestination(base.TurnTaker.Deck)
            };

            IEnumerator coroutine = GameController.SelectCardsFromLocationAndMoveThem(DecisionMaker, TurnTaker.Trash, 0, TurnTaker.Trash.NumberOfCards, new LinqCardCriteria(IsTattoo, "tattoo"), moveCardDestination, autoDecideCard: true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }


            // Shuffle the villain deck
            coroutine = GameController.ShuffleLocation(TurnTaker.Deck, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);

            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            // Play the top 2 cards of the villain deck.
            coroutine = GameController.PlayTopCardOfLocation(TurnTakerController, TurnTaker.Deck, false, CardsToPlay, CardsToPlay, true, cardSource: GetCardSource());
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