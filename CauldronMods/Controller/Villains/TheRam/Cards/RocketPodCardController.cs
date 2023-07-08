using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace Cauldron.TheRam
{
    public class RocketPodCardController : TheRamUtilityCardController
    {
        public RocketPodCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            AddUpCloseTrackers();
        }

        public override void AddTriggers()
        {
            //"This card is immune to damage from heroes that are not Up Close.",
            AddImmuneToDamageTrigger((DealDamageAction dd) => dd.Target == this.Card && dd.DamageSource != null && dd.DamageSource.Card != null && dd.DamageSource.IsHero && dd.DamageSource.IsCard && !IsUpClose(dd.DamageSource.Card));
            //"At the end of the villain turn, this card deals each hero that is not Up Close {H - 2} cold damage."
            AddDealDamageAtEndOfTurnTrigger(TurnTaker, this.Card, (Card c) => IsHero(c) && !IsUpClose(c), TargetType.All, H - 2, DamageType.Cold);
        }
    }
}