using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Celadroch
{
    public class SpringsAtrophyCardController : CardController
    {
        /*
         * "At the end of the villain turn, this card deals the hero target with the lowest HP {H - 1} toxic damage."
         */

        public SpringsAtrophyCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroTargetWithLowestHP();
        }

        public override void AddTriggers()
        {
            AddDealDamageAtEndOfTurnTrigger(TurnTaker, Card, c => IsHeroTarget(c), TargetType.LowestHP, H - 1, DamageType.Toxic);
        }
    }
}