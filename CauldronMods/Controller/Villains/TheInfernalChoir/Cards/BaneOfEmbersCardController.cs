using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheInfernalChoir
{
    public class BaneOfEmbersCardController : TheInfernalChoirUtilityCardController
    {
        public BaneOfEmbersCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroTargetWithHighestHP(1, H);
        }

        public override void AddTriggers()
        {
            base.AddTriggers();

            AddTrigger<DealDamageAction>(dda => dda.Target == Card && dda.DidDealDamage && dda.Amount > 0 && !HasBeenSetToTrueThisTurn("FirstTimeDealtDamage"), dda => SetFirstTimeDealtDamageFlag(), TriggerType.ImmuneToDamage, TriggerTiming.After);
            AddImmuneToDamageTrigger(dda => dda.Target == Card && HasBeenSetToTrueThisTurn("FirstTimeDealtDamage"));

            AddDealDamageAtEndOfTurnTrigger(TurnTaker, Card, c => c.IsHero, TargetType.HighestHP, 0, DamageType.Cold,
                    numberOfTargets: H,
                    dynamicAmount: c => Card.HitPoints.Value);
        }

        private IEnumerator SetFirstTimeDealtDamageFlag()
        {
            SetCardPropertyToTrueIfRealAction("FirstTimeDealtDamage");
            return DoNothing();
        }
    }
}
