using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Tiamat
{
    public class HydraInfernoTiamatCharacterCardController : HydraTiamatCharacterCardController
    {
        public HydraInfernoTiamatCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowSpecialString(() => base.Card.Title + " is immune to Fire damage.").Condition = () => !base.Card.IsFlipped;
        }

        protected override ITrigger[] AddFrontTriggers()
        {
            return new ITrigger[]
            { 
				//{Tiamat}, The Mouth of the Inferno is immune to Fire damage.
				base.AddImmuneToDamageTrigger((DealDamageAction dealDamage) => dealDamage.Target == base.Card && dealDamage.DamageType == DamageType.Fire)
            };
        }
    }
}