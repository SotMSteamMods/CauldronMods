using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.TheCybersphere
{
    public class Sp4rkCardController : TheCybersphereCardController
    {

        public Sp4rkCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNonEnvironmentTargetWithHighestHP(ranking: 2);
        }

        public override void AddTriggers()
        {
            //At the end of the environment turn, this card deals the non-environment target with the second highest HP 3 toxic damage.
            AddDealDamageAtEndOfTurnTrigger(base.TurnTaker, base.Card, (Card c) => c.IsNonEnvironmentTarget && GameController.IsCardVisibleToCardSource(c, GetCardSource()), TargetType.HighestHP, 3, DamageType.Toxic, highestLowestRanking: 2);
        }
    }
}