using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;

namespace Cauldron.Tiamat
{
    public class HydraStormTiamatCharacterCardController : HydraTiamatCharacterCardController
    {
        public HydraStormTiamatCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        protected override ITrigger[] AddFrontTriggers()
        {
            return new ITrigger[]
            { 
				//{Tiamat}, The Eye of the Storm is immune to Lightning damage.
				base.AddImmuneToDamageTrigger((DealDamageAction dealDamage) => dealDamage.Target == base.Card && dealDamage.DamageType == DamageType.Lightning)
            };
        }
    }
}
