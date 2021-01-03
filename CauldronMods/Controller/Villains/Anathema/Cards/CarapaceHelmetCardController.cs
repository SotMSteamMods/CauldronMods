using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Anathema
{
    public class CarapaceHelmetCardController : HeadCardController
    {
		public CarapaceHelmetCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

		public override void AddTriggers()
		{
			//Reduce damage dealt to this card by 1.
			base.AddReduceDamageTrigger((Card c) => c == base.Card, 1);
			//Villain targets are immune to damage dealt by environment cards. 
			base.AddImmuneToDamageTrigger((DealDamageAction dd) => dd.DamageSource != null &&  dd.DamageSource.IsEnvironmentCard && base.IsVillainTarget(dd.Target));
		}
	}
}
