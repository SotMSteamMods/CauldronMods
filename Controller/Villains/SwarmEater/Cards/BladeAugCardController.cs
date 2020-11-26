using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.SwarmEater
{
    public class BladeAugCardController : AugCardController
    {
        public BladeAugCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            if (base.Card.IsInPlayAndNotUnderCard)
            {
                base.SpecialStringMaker.ShowHeroTargetWithHighestHP();
            }
            else
            {
                base.SpecialStringMaker.ShowHighestHP(cardCriteria: new LinqCardCriteria((Card c) => c != base.CharacterCard));
            }
        }

        public override ITrigger[] AddRegularTriggers()
        {
            //At the end of the villain turn, this card deals the hero target with the highest HP 2 lightning damage.
            return new ITrigger[] { base.AddDealDamageAtEndOfTurnTrigger(base.TurnTaker, base.Card, (Card c) => c.IsHero, TargetType.HighestHP, 2, DamageType.Lightning) };
        }

        public override ITrigger[] AddAbsorbTriggers(Card cardThisIsUnder)
        {
            //Absorb: at the end of the villain turn, {SwarmEater} deals the target other than itself with the highest HP 2 lightning damage.
            return new ITrigger[] { base.AddDealDamageAtEndOfTurnTrigger(base.TurnTaker, base.Card, (Card c) => c != cardThisIsUnder, TargetType.HighestHP, 2, DamageType.Lightning) };
        }
    }
}