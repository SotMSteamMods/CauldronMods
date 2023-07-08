using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Dynamo
{
    public class EnergyConversionCardController : DynamoUtilityCardController
    {
        public EnergyConversionCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //When this card enters play, discard the top card of the villain deck.
            List<MoveCardAction> moveCardActions = new List<MoveCardAction>();
            IEnumerator coroutine = base.GameController.DiscardTopCard(base.TurnTaker.Deck, moveCardActions);
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
            //When {Dynamo} is dealt 4 or more damage from a single source, discard the top card of the villain deck and {Dynamo} deals each hero target {H} energy damage. Then, destroy this card.
            base.AddTrigger<DealDamageAction>((DealDamageAction action) => action.Target == base.CharacterCard && action.Amount >= 4, this.TakeDamageResponse, new TriggerType[] { TriggerType.DiscardCard, TriggerType.DealDamage, TriggerType.DestroySelf }, TriggerTiming.After);
        }

        private IEnumerator TakeDamageResponse(DealDamageAction action)
        {
            //...discard the top card of the villain deck
            List<MoveCardAction> moveCardActions = new List<MoveCardAction>();
            IEnumerator coroutine = base.GameController.DiscardTopCard(base.TurnTaker.Deck, moveCardActions);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //...and {Dynamo} deals each hero target {H} energy damage. 
            coroutine = base.DealDamage(base.CharacterCard, (Card c) => IsHeroTarget(c), base.Game.H, DamageType.Energy);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //Then, destroy this card.
            coroutine = base.DestroyThisCardResponse(action);
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
