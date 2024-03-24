using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Oriphel
{
    public class HighDjarilCardController : OriphelGuardianCardController
    {
        public HighDjarilCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroTargetWithHighestHP();
        }

        public override void AddTriggers()
        {
            //Guardian destroy trigger
            base.AddTriggers();

            //"At the end of the villain turn, this card deals the hero target with the highest HP {H} lightning damage.",
            AddDealDamageAtEndOfTurnTrigger(TurnTaker, this.Card, (Card c) => IsHeroTarget(c), TargetType.HighestHP, H, DamageType.Lightning);
        }
    }
}