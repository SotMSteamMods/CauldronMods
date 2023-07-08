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
            SpecialStringMaker.ShowIfElseSpecialString(() => HasBeenSetToTrueThisTurn("FirstTimeDealtDamage"), () => $"{Card.Title} is immune to damage for the rest of this turn.", () => $"{Card.Title} has not been dealt damage this turn.").Condition = () => Card.IsInPlayAndHasGameText;
            SpecialStringMaker.ShowHeroTargetWithHighestHP(numberOfTargets: H);
        }

        public readonly string FirstTimeDealtDamage = "FirstTimeDealtDamage";

        public override void AddTriggers()
        {
            base.AddTriggers();

            AddTrigger<DealDamageAction>(dda => dda.Target == Card && dda.DidDealDamage && dda.Amount > 0 && !HasBeenSetToTrueThisTurn(FirstTimeDealtDamage), dda => SetFirstTimeDealtDamageFlag(), TriggerType.ImmuneToDamage, TriggerTiming.After);
            AddImmuneToDamageTrigger(dda => dda.Target == Card && HasBeenSetToTrueThisTurn(FirstTimeDealtDamage));

            AddDealDamageAtEndOfTurnTrigger(TurnTaker, Card, c => IsHero(c), TargetType.HighestHP, 0, DamageType.Cold,
                    numberOfTargets: H,
                    dynamicAmount: c => Card.HitPoints.Value);
        }

        private IEnumerator SetFirstTimeDealtDamageFlag()
        {
            SetCardPropertyToTrueIfRealAction(FirstTimeDealtDamage);
            return DoNothing();
        }
    }
}
