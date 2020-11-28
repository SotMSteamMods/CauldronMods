using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.TheCybersphere
{
    public class B3h3mthCardController : TheCybersphereCardController
    {

        public B3h3mthCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            //At the end of the environment turn, this card deals the non-environment target with the second lowest HP 4 fire damage.
            AddDealDamageAtEndOfTurnTrigger(base.TurnTaker, base.Card, (Card c) => c.IsNonEnvironmentTarget, TargetType.LowestHP, 4, DamageType.Fire, highestLowestRanking: 2);
        }


    }
}