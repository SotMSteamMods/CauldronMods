using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.TheCybersphere
{
    public class InfectedFirewallCardController : TheCybersphereCardController
    {

        public InfectedFirewallCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //Reduce damage dealt to environment targets by 1.
            AddReduceDamageTrigger((Card c) => c.IsEnvironmentTarget && GameController.IsCardVisibleToCardSource(c, GetCardSource()), 1);

            //At the end of the environment turn, play the top card of the environment deck.
            AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, PlayTheTopCardOfTheEnvironmentDeckWithMessageResponse, TriggerType.PlayCard);
        }
    }
}