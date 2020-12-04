using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.SwarmEater
{
    public class HuntingGroundsCardController : CardController
    {
        public HuntingGroundsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //Reduce damage dealt to {SwarmEater} by environment targets by 1.
            base.AddReduceDamageTrigger((DealDamageAction action) => action.DamageSource != null && action.Target == base.CharacterCard && action.DamageSource.IsEnvironmentTarget, (DealDamageAction action) => 1);
            //Increase damage dealt by {SwarmEater} to environment targets by 1.
            base.AddIncreaseDamageTrigger((DealDamageAction action) => action.DamageSource != null && action.Target.IsEnvironmentTarget && action.DamageSource.Card == base.CharacterCard, 1);
            //Whenever {SwarmEater} destroys a target, play the top card of the environment deck.
            base.AddTrigger<DestroyCardAction>((DestroyCardAction action) => action.ResponsibleCard == base.CharacterCard, base.PlayTheTopCardOfTheEnvironmentDeckResponse, TriggerType.PlayCard, TriggerTiming.After);
        }
    }
}