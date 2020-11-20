using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.Quicksilver
{
    public class MalleableArmorCardController : CardController
    {
        public MalleableArmorCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }
        public override void AddTriggers()
        {
            //If {Quicksilver} would be reduced from greater than 1 HP to 0 or fewer HP...
            base.AddTrigger<DealDamageAction>(delegate (DealDamageAction action)
            {
                if (action.Target == base.CharacterCard && base.CharacterCard.HitPoints > 1)
                {
                    int amount = action.Amount;
                    int? hitPoints = action.Target.HitPoints;
                    return amount >= hitPoints.GetValueOrDefault() & hitPoints != null;
                }
                return false;
            }, new Func<DealDamageAction, IEnumerator>(this.DealDamageResponse), new TriggerType[]
            {
                TriggerType.WouldBeDealtDamage,
                TriggerType.GainHP
            }, TriggerTiming.Before);
        }

        private IEnumerator DealDamageResponse(DealDamageAction action)
        {
            //...restore her to 1HP.
            IEnumerator coroutine = base.CancelAction(action);
            base.CharacterCard.SetHitPoints(action.Amount + 1);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            //base.CharacterCard.SetHitPoints(action.Amount + 1);
            yield break;
        }
    }
}