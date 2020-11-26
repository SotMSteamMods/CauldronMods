using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.SwarmEater
{
    public class JumperAugCardController : AugCardController
    {
        public JumperAugCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            if (base.Card.IsInPlayAndNotUnderCard)
            {
                base.SpecialStringMaker.ShowHeroTargetWithLowestHP(numberOfTargets: 2);
            }
        }

        public override ITrigger[] AddRegularTriggers()
        {
            return new ITrigger[] {
                //Reduce damage dealt to this card by non-villain cards by 1.
                base.AddReduceDamageTrigger((DealDamageAction action) => action.Target == base.Card && !action.DamageSource.IsVillain, (DealDamageAction action) => 1),
                //At the end of the villain turn this card deals the 2 hero targets with the lowest HP {H - 2} melee damage each.
                base.AddDealDamageAtEndOfTurnTrigger(base.TurnTaker, base.Card, (Card c) => c.IsHero, TargetType.LowestHP, Game.H - 2, DamageType.Melee, numberOfTargets: 2)
            };
        }

        public override ITrigger[] AddAbsorbTriggers(Card cardThisIsUnder)
        {
            //Absorb: reduce damage dealt to {SwarmEater} by 1.
            return new ITrigger[] { base.AddReduceDamageTrigger((Card c) => c == cardThisIsUnder, 1) };
        }
    }
}