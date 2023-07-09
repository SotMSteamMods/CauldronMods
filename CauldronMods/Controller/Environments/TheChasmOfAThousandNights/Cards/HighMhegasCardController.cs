using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheChasmOfAThousandNights
{
    public class HighMhegasCardController : TheChasmOfAThousandNightsUtilityCardController
    {
        public HighMhegasCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroTargetWithHighestHP(numberOfTargets: 3);
        }

        public override void AddTriggers()
        {
            //At the end of the environment turn, this card deals the 3 hero targets with the highest HP 2 melee damage each.
            AddDealDamageAtEndOfTurnTrigger(TurnTaker, Card, (Card c) => IsHeroTarget(c) && GameController.IsCardVisibleToCardSource(c, GetCardSource()), TargetType.HighestHP, 2, DamageType.Melee, numberOfTargets: 3);
        }
    }
}
