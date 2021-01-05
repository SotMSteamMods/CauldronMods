using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;

namespace Cauldron.Tiamat
{
    public class HydraEarthTiamatCharacterCardController : HydraTiamatCharacterCardController
    {
        public HydraEarthTiamatCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowSpecialString(() => base.Card.Title + " is immune to Melee damage.").Condition = () => !base.Card.IsFlipped;

        }

        protected override ITrigger[] AddFrontTriggers()
        {
            return new ITrigger[]
            { 
				//{Tiamat}, The Maw of the Earth is immune to Melee damage.
				base.AddImmuneToDamageTrigger((DealDamageAction dealDamage) => dealDamage.Target == base.Card && dealDamage.DamageType == DamageType.Melee),
                //Reduce damage dealt to other villain targets by 1.
                base.AddReduceDamageTrigger((Card c) => IsVillainTarget(c) && c != this.Card, 1)
            };
        }
    }
}
