using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheChasmOfAThousandNights
{
    public class HighTemoqCardController : TheChasmOfAThousandNightsUtilityCardController
    {
        public HighTemoqCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNonEnvironmentTargetWithLowestHP(numberOfTargets: 2);
        }

        public override void AddTriggers()
        {
            //At the end of the environment turn, this card deals the 2 non-environment targets with the lowest HP 2 fire damage each.
            AddDealDamageAtEndOfTurnTrigger(TurnTaker, Card, (Card c) => c.IsNonEnvironmentTarget && GameController.IsCardVisibleToCardSource(c, GetCardSource()), TargetType.LowestHP, 2, DamageType.Fire, numberOfTargets: 2);
        }
    }
}
