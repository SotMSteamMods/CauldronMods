using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheChasmOfAThousandNights
{
    public class ViciousCardController : NatureCardController
    {
        public ViciousCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //Increase damage dealt by the target next to this card by 1.
            AddIncreaseDamageTrigger((DealDamageAction dd) => GetCardThisCardIsBelow() != null && dd.DamageSource != null && dd.DamageSource.Card != null && dd.DamageSource.Card == GetCardThisCardIsBelow(), 1);
            base.AddTriggers();
        }
    }
}
