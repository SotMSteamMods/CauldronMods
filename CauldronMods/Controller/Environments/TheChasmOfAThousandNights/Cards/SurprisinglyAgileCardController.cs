using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheChasmOfAThousandNights
{
    public class SurprisinglyAgileCardController : NatureCardController
    {
        public SurprisinglyAgileCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //Reduce damage dealt to the target next to this card by 1.
            AddReduceDamageTrigger((Card c) => GetCardThisCardIsBelow() != null && c == GetCardThisCardIsBelow(), 1);
            base.AddTriggers();
        }
    }
}
