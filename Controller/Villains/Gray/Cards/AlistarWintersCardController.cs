using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;

namespace Cauldron.Gray
{
    public class AlistarWintersCardController : CardController
    {
        public AlistarWintersCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //Increase damage dealt to hero targets by 1.
            base.AddIncreaseDamageTrigger((DealDamageAction action) => action.Target.IsHero, 1);
            //Hero tagets cannot gain HP.
            base.AddTrigger<GainHPAction>((GainHPAction action) => action.HpGainer.IsHero, (GainHPAction action) => base.CancelAction(action), TriggerType.CancelAction, TriggerTiming.Before);
        }
    }
}