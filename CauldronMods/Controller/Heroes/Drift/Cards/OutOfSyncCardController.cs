using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Drift
{
    public class OutOfSyncCardController : DriftUtilityCardController
    {
        public OutOfSyncCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //{DriftPast} Reduce damage dealt to {Drift} by 1.
            base.AddReduceDamageTrigger((Card c) => base.IsTimeMatching(Past) && c == base.GetActiveCharacterCard(), 1);

            //{DriftFuture} Increase damage dealt by {Drift} to other targets by 1.
            base.AddIncreaseDamageTrigger((DealDamageAction action) => base.IsTimeMatching(Future) && action.DamageSource.IsSameCard(base.GetActiveCharacterCard()) && action.Target != base.GetActiveCharacterCard(), 1);
        }
    }
}
