using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Tiamat
{
    public class HydraWinterTiamatCharacterCardController : HydraTiamatCharacterCardController
    {
        public HydraWinterTiamatCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowSpecialString(() => base.Card.Title + " is immune to Cold damage.").Condition = () => !base.Card.IsFlipped;
        }

        protected override ITrigger[] AddFrontTriggers()
        {
            return new ITrigger[]
            { 
				//{Tiamat}, The Jaws of Winter is immune to Cold damage.
				base.AddImmuneToDamageTrigger((DealDamageAction dealDamage) => dealDamage.Target == base.Card && dealDamage.DamageType == DamageType.Cold)
            };
        }
    }
}
