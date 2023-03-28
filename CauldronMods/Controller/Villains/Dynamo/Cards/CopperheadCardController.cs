using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Dynamo
{
    public class CopperheadCardController : DynamoUtilityCardController
    {
        public CopperheadCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowHeroTargetWithHighestHP(numberOfTargets: 2);
        }

        public override void AddTriggers()
        {
            //If this card has 10 or fewer HP, increase damage it deals by 2.
            base.AddIncreaseDamageTrigger((DealDamageAction action) => action.DamageSource.IsSameCard(this.Card) && this.Card.HitPoints <= 10, 2);

            //At the end of the villain turn, this card deals the 2 hero targets with the highest HP {H} melee damage each.
            base.AddDealDamageAtEndOfTurnTrigger(base.TurnTaker, this.Card, (Card c) => IsHeroTarget(c), TargetType.HighestHP, base.Game.H, DamageType.Melee, numberOfTargets: 2);
        }
    }
}
