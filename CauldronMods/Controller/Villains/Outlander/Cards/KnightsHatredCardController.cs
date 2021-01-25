using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Outlander
{
    public class KnightsHatredCardController : OutlanderUtilityCardController
    {
        public KnightsHatredCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //Increase damage dealt by {Outlander} by 1.
            base.AddIncreaseDamageTrigger((DealDamageAction action) => action.DamageSource.IsSameCard(base.CharacterCard), 1);

            //Reduce damage dealt to {Outlander} by 1.
            base.AddReduceDamageTrigger((Card c) => c == base.CharacterCard, 1);

            //At the start of the villain turn, destroy this card.
            base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, base.DestroyThisCardResponse, TriggerType.DestroySelf);
        }
    }
}
