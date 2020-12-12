using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron
{
    public class AquaticSphereCardController : CardController
    {
        public AquaticSphereCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //When this card enters play, place the top card of the villain deck beneath it face down.
            IEnumerator coroutine = base.GameController.MoveCard(base.TurnTakerController, base.TurnTaker.Deck.TopCard, base.Card.UnderLocation, flipFaceDown: true, cardSource: base.GetCardSource());
            coroutine = base.GameController.GainHP(base.Card, 6, cardSource: base.GetCardSource());
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

        public override void AddTriggers()
        {
            //Whenever a hero uses a power, they must discard a card.
            base.AddTrigger<UsePowerAction>((UsePowerAction action) => true, this.DiscardCardResponse, TriggerType.DiscardCard, TriggerTiming.After);
        }

        private IEnumerator DiscardCardResponse(UsePowerAction action)
        {
            //...they must discard a card.
            IEnumerator coroutine = base.GameController.SelectAndDiscardCard(action.HeroUsingPower, cardSource: base.GetCardSource());
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