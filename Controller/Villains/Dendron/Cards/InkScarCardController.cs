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

        public static string Identifier = "InkScar";

        private const int CardsToPlay = 2;

        public InkScarCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            // Shuffle all Tattoos from the villain trash into the villain deck.


            // Move found Tattoo cards from villain trash to villain deck
            List<MoveCardDestination> moveCardDestination = new List<MoveCardDestination>();
            moveCardDestination.Add(new MoveCardDestination(base.TurnTaker.Deck));

            this.RevealCards_MoveMatching_ReturnNonMatchingCards(this.TurnTakerController, this.TurnTaker.Trash, false,
                false, false, new LinqCardCriteria(IsTattoo), TattooCardsInDeck, TattooCardsInDeck);



            IEnumerator moveCardsRoutine = base.GameController.SelectCardsFromLocationAndMoveThem(this.DecisionMaker, this.TurnTaker.Trash, 0, TattooCardsInDeck,
                new LinqCardCriteria(IsTattoo), moveCardDestination, autoDecideCard: true);

            // Shuffle the villain deck
            IEnumerator shuffleDeckRoutine = base.GameController.ShuffleLocation(this.TurnTaker.Deck, cardSource: this.GetCardSource());

            // Play the top 2 cards of the villain deck.
            IEnumerator playCardsRoutine
                = this.GameController.PlayTopCardOfLocation(this.TurnTakerController, this.Card.Owner.Deck, false, CardsToPlay, CardsToPlay,
                    true);

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(moveCardsRoutine);
                yield return base.GameController.StartCoroutine(shuffleDeckRoutine);
                yield return base.GameController.StartCoroutine(playCardsRoutine);

            }
            else
            {
                base.GameController.ExhaustCoroutine(moveCardsRoutine);
                base.GameController.ExhaustCoroutine(shuffleDeckRoutine);
                base.GameController.ExhaustCoroutine(playCardsRoutine);
            }
        }
    }
}