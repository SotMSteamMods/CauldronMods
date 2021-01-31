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
            AddIncreaseDamageTrigger((DealDamageAction action) => action.DamageSource.IsSameCard(CharacterCard), 1);

            //Reduce damage dealt to {Outlander} by 1.
            AddReduceDamageTrigger((Card c) => c == CharacterCard, 1);

            //At the start of the villain turn, destroy this card.
            AddStartOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, DestroyThisCardResponse, TriggerType.DestroySelf);
        }
    }
}
