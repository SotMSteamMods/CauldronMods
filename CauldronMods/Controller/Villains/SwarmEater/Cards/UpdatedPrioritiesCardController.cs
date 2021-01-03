using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.SwarmEater
{
    public class UpdatedPrioritiesCardController : CardController
    {
        public UpdatedPrioritiesCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //Search the villain deck and trash for Single-Minded Pursuit and put it into play. If you searched the deck, shuffle it.
            Card pursuit = base.FindCard("SingleMindedPursuit");
            Location pursuitLocation = pursuit.Location;
            IEnumerator coroutine = base.PlayCardFromLocation(pursuitLocation, pursuit.Identifier, shuffleAfterwardsIfDeck: true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            //Play the top card of the villain deck.
            coroutine = base.GameController.PlayTopCard(this.DecisionMaker, base.TurnTakerController, cardSource: base.GetCardSource());
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