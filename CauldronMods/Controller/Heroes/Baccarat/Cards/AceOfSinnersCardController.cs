using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Baccarat
{
    public class AceOfSinnersCardController : AceOfCardController
    {
        public AceOfSinnersCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {


        }

        protected override ITrigger EffectTrigger()
        {
            //Increase damage dealt by hero targets by 1.
            return base.AddIncreaseDamageTrigger((DealDamageAction dealDamage) => dealDamage.DamageSource != null && dealDamage.DamageSource.IsHeroTarget, 1);
        }
    }
}
