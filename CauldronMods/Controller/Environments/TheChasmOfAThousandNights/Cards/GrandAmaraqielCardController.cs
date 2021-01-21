using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheChasmOfAThousandNights
{
    public class GrandAmaraqielCardController : TheChasmOfAThousandNightsUtilityCardController
    {
        public GrandAmaraqielCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNonEnvironmentTargetWithHighestHP(numberOfTargets: 2);
        }

        public override void AddTriggers()
        {
            //At the end of the environment turn, this card deals the 2 non-environment targets with the highest HP 3 cold damage each.
            AddDealDamageAtEndOfTurnTrigger(TurnTaker, Card, (Card c) => c.IsNonEnvironmentTarget && GameController.IsCardVisibleToCardSource(c, GetCardSource()), TargetType.HighestHP, 3, DamageType.Cold, numberOfTargets: 2);
        }
    }
}
