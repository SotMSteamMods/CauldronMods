using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron
{
    public class BladeAugCardController : AugCardController
    {
        public BladeAugCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //At the end of the villain turn, this card deals the hero target with the highest HP 2 lightning damage.
            base.AddDealDamageAtEndOfTurnTrigger(base.TurnTaker, base.Card, (Card c) => c.IsHero, TargetType.HighestHP, 2, DamageType.Lightning);
        }

        public override IEnumerator ActivateAbsorb(Card cardThisIsUnder)
        {
            if (cardThisIsUnder.Identifier == "AbsorbedNanites")
            {
                cardThisIsUnder = base.CharacterCard;
            }
            //Absorb: at the end of the villain turn, {SwarmEater} deals the target other than itself with the highest HP 2 lightning damage.
            base.AddDealDamageAtEndOfTurnTrigger(base.TurnTaker, base.Card, (Card c) => c != cardThisIsUnder, TargetType.HighestHP, 2, DamageType.Lightning);
            yield break;
        }
    }
}