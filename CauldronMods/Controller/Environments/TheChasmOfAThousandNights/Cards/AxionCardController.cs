using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheChasmOfAThousandNights
{
    public class AxionCardController : TheChasmOfAThousandNightsUtilityCardController
    {
        public AxionCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //At the end of the environment turn, this card deals each villain target 1 melee damage.
            AddDealDamageAtEndOfTurnTrigger(TurnTaker, Card, (Card c) => IsVillainTarget(c) && GameController.IsCardVisibleToCardSource(c, GetCardSource()), TargetType.All, 1, DamageType.Melee);
        }
    }
}
