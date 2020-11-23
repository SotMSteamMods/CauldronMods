using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.SwarmEater
{
    public class BehindYouCardController : CardController
    {
        public BehindYouCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //Whenever a target enters play, {SwarmEater} deals that target 1 melee damage.
            base.AddTrigger<PlayCardAction>((PlayCardAction action) => action.CardToPlay.IsTarget, this.TargetEnterResponse, new TriggerType[] { TriggerType.DealDamage }, TriggerTiming.After);
            //When this card is destroyed, play the top card of the villain deck.
            base.AddWhenDestroyedTrigger(new Func<DestroyCardAction, IEnumerator>(this.OnDestroyResponse), TriggerType.PlayCard);
        }

        private IEnumerator TargetEnterResponse(PlayCardAction action)
        {
            //Whenever a target enters play, {SwarmEater} deals that target 1 melee damage.
            IEnumerator coroutine = base.DealDamage(base.CharacterCard, action.CardToPlay, 1, DamageType.Melee, cardSource: base.GetCardSource());
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

        private IEnumerator OnDestroyResponse(DestroyCardAction action)
        {
            //play the top card of the villain deck
            IEnumerator coroutine = base.GameController.PlayTopCard(this.DecisionMaker, base.TurnTakerController, cardSource: base.GetCardSource());
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