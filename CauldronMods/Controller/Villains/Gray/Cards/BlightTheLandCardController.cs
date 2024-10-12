using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.Gray
{
    public class BlightTheLandCardController : CardController
    {
        public BlightTheLandCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //At the end of the villain turn, this card deals each non-villain target 2 toxic damage...
            base.AddDealDamageAtEndOfTurnTrigger(this.TurnTaker, base.Card, (Card c) => !IsVillainTarget(c), TargetType.All, 2, DamageType.Toxic);
            //...and each Radiation card regains 2HP.
            base.AddEndOfTurnTrigger((TurnTaker turnTaker) => turnTaker == base.TurnTaker, (PhaseChangeAction action) => base.GameController.GainHP(this.DecisionMaker, (Card c) => c.DoKeywordsContain("radiation"), 2, cardSource: base.GetCardSource()), TriggerType.GainHP);
        }
    }
}
