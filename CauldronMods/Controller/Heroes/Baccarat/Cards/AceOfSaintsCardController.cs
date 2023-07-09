using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Baccarat
{
    public class AceOfSaintsCardController : AceOfCardController
    {
        public AceOfSaintsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        protected override ITrigger EffectTrigger()
        {
            //Reduce damage dealt to hero targets by 1.
            return base.AddReduceDamageTrigger((Card c) => IsHero(c), 1);
        }
    }
}