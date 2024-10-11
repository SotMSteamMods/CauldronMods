using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;

namespace Cauldron.Tiamat
{
    public class HydraDecayTiamatCharacterCardController : HydraTiamatCharacterCardController
    {
        public HydraDecayTiamatCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowSpecialString(() => base.Card.Title + " is immune to Toxic damage.").Condition = () => !base.Card.IsFlipped;
        }

        protected override ITrigger[] AddFrontTriggers()
        {
            return new ITrigger[]
            { 
				//{Tiamat}, The Breath of Decay is immune to Toxic damage.
				base.AddImmuneToDamageTrigger((DealDamageAction dealDamage) => dealDamage.Target == base.Card && dealDamage.DamageType == DamageType.Toxic),
                //Increase damage dealt to hero targets by 1.
                base.AddIncreaseDamageTrigger((DealDamageAction action) => action.Target != null && IsHeroTarget(action.Target) && action.Target.IsTarget, 1)
            };
        }
    }
}
