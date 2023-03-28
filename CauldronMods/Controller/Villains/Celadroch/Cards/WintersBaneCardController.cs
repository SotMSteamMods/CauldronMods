using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Celadroch
{
    public class WintersBaneCardController : CardController
    {
        /*
         * 	"Reduce damage dealt to villain cards by 1.",
			"At the end of the villain turn, this card deals the hero target with the highest HP {H} cold damage."
         */

        public WintersBaneCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroTargetWithHighestHP();
        }

        public override void AddTriggers()
        {
            AddReduceDamageTrigger(c => IsVillain(c), 1);

            AddDealDamageAtEndOfTurnTrigger(TurnTaker, Card, c => IsHeroTarget(c), TargetType.HighestHP, H, DamageType.Cold);
        }
    }
}