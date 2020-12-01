using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.Titan
{
    public class TitanformCardController : CardController
    {
        public TitanformCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SetCardProperty("FirstTimeDamageDealt", false);
        }

        private const string FirstTimeDamageDealt = "FirstTimeDamageDealt";

        public override void AddTriggers()
        {
            //Whenever {Titan} is dealt damage by another target, reduce damage dealt to {Titan} by 1 until the start of your next turn.
            base.AddTrigger<DealDamageAction>((DealDamageAction action) => !base.IsPropertyTrue("FirstTimeDamageDealt") && action.Target == base.CharacterCard, this.DealtDamageResponse, TriggerType.Hidden, TriggerTiming.After);
            base.AddReduceDamageTrigger((Card c) => base.IsPropertyTrue("FirstTimeDamageDealt") && c == base.CharacterCard, 1);
            base.AddAfterLeavesPlayAction((GameAction ga) => base.ResetFlagAfterLeavesPlay("FirstTimeDamageDealt"), TriggerType.Hidden);

            //When {Titan} would deal damage, you may destroy this card to increase that damage by 2.
            base.AddTrigger<DealDamageAction>((DealDamageAction action) => action.DamageSource.Card == base.CharacterCard, this.DestroyThisCardToIncreaseDamageResponse, new TriggerType[] { TriggerType.DestroySelf, TriggerType.IncreaseDamage }, TriggerTiming.Before);
        }

        private IEnumerator DealtDamageResponse(DealDamageAction action)
        {
            base.SetCardPropertyToTrueIfRealAction("FirstTimeDamageDealt");
            yield break;
        }

        private IEnumerator DestroyThisCardToIncreaseDamageResponse(DealDamageAction action)
        {
            //...you may destroy this card...
            IEnumerator coroutine = base.DestroyThisCardResponse(action);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            //...to increase that damage by 2.
            coroutine = base.GameController.IncreaseDamage(action, 2, cardSource: base.GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }
    }
}