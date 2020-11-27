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

        public override ITrigger[] AddRegularTriggers()
        {
            //Increase damage dealt by villain targets by 1.
            return new ITrigger[] { base.AddIncreaseDamageTrigger((DealDamageAction action) => action.DamageSource.IsVillainTarget, 1) };
        }

        public override ITrigger[] AddAbsorbTriggers(Card cardThisIsUnder)
        {
            //Absorb: increase damage dealt by {SwarmEater} by 1.
            return new ITrigger[] { base.AddIncreaseDamageTrigger((DealDamageAction action) => action.DamageSource.Card == cardThisIsUnder, 1) };
        }
    }
}