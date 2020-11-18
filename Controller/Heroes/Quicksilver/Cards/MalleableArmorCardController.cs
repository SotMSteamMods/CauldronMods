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
            base.AddTrigger(base.AddTrigger<DealDamageAction>((DealDamageAction action) => action.Target == base.CharacterCard && action.Target.HitPoints > 1 && action.Target.HitPoints - action.Amount <= 0, (DealDamageAction action) => DealDamageResponse(action), TriggerType.GainHP, TriggerTiming.After));
        }

        private IEnumerator DealDamageResponse(DealDamageAction action)
        {
            //...restore her to 1HP.
            base.CharacterCard.SetHitPoints(1);
            yield break;
        }
    }
}