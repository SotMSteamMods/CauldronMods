using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.PhaseVillain
{
    public class NowhereToGoButDownCardController : PhaseCardController
    {
        public NowhereToGoButDownCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //Reveal cards from the top of the villain deck until {H - 1} Obstacle cards are revealed. Shuffle the other revealed cards back into the villain deck.
            //Put the Obstacles into play in the order they were revealed.
            IEnumerator coroutine = base.RevealCards_MoveMatching_ReturnNonMatchingCards(base.TurnTakerController, base.TurnTaker.Deck, false, true, false, new LinqCardCriteria((Card c) => base.IsObstacle(c)), Game.H - 1, shuffleSourceAfterwards: true, revealedCardDisplay: RevealedCardDisplay.ShowMatchingCards);
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