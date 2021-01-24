using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.OblaskCrater
{
    public class IndustriousHulkCardController : OblaskCraterUtilityCardController
    {
        /*
         * Whenever a hero draws a card, this card deals them 1 melee damage.
         * When this card is destroyed, each player may put the top card of 
         * their deck into their hand.
        */
        public IndustriousHulkCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            base.AddTrigger<DrawCardAction>((dca) => dca.DrawnCard.IsHero, (dca) => base.DealDamage(base.Card, dca.HeroTurnTaker.CharacterCard, 1, DamageType.Melee, cardSource: base.GetCardSource()), TriggerType.DealDamage, TriggerTiming.After);
            base.AddWhenDestroyedTrigger((dca) => base.EachPlayerDrawsACard(optional: true), TriggerType.DrawCard);
        }
    }
}
