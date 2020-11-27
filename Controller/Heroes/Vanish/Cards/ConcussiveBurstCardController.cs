using System;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Vanish
{
    public class ConcussiveBurstCardController : CardController
    {
        private readonly static string TrackingKey = "UsedConcussiveBurst";

        public ConcussiveBurstCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            var ss = SpecialStringMaker.ShowSpecialString(() => $"{Card.Title} has been used this turn.");
            ss.Condition = () => Card.IsInPlayAndHasGameText && base.HasBeenSetToTrueThisTurn(TrackingKey);
        }

        public override void AddTriggers()
        {
            AddTrigger<DealDamageAction>(dda => dda.DamageSource.IsSameCard(this.CharacterCard) && !dda.Target.IsHero && !base.HasBeenSetToTrueThisTurn(TrackingKey), DealDamageResponse, TriggerType.AddStatusEffectToDamage, TriggerTiming.Before);
        }

        private IEnumerator DealDamageResponse(DealDamageAction dda)
        {
            dda.AddStatusEffectResponse(ApplyStatusEffectResponse);
            return DoNothing();
        }

        private IEnumerator ApplyStatusEffectResponse(DealDamageAction dda)
        {
            if (dda.DidDealDamage)
            {
                base.SetCardPropertyToTrueIfRealAction(TrackingKey);
                return base.ReduceDamageDealtByThatTargetUntilTheStartOfYourNextTurnResponse(dda, 1);
            }
            else
            {
                return DoNothing();
            }
        }
    }
}