using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.SwarmEater
{
    public class SpeedAugCardController : AugCardController
    {
        public SpeedAugCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //Increase damage dealt by villain targets by 1.
            base.AddIncreaseDamageTrigger((DealDamageAction action) => action.DamageSource != null && action.DamageSource.Card != null  && base.Card.IsInPlayAndNotUnderCard && action.DamageSource.IsVillainTarget, 1);

            //Absorb: increase damage dealt by {SwarmEater} by 1.
            base.AddIncreaseDamageTrigger((DealDamageAction action) => action.DamageSource != null && action.DamageSource.Card != null  && CanAbsorbEffectTrigger() && action.DamageSource.Card == this.CardThatAbsorbedThis(), 1);
        }
    }
}