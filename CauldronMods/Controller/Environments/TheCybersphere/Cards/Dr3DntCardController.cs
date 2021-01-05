using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.TheCybersphere
{
    public class Dr3DntCardController : TheCybersphereCardController
    {

        public Dr3DntCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //At the end of the environment turn, this card deals each other target 2 projectile damage.
            AddDealDamageAtEndOfTurnTrigger(base.TurnTaker, base.Card, (Card c) => c.IsTarget && c != base.Card && GameController.IsCardVisibleToCardSource(c, GetCardSource()), TargetType.All, 2, DamageType.Projectile);
        }
    }
}