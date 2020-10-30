using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace SotMWorkshop.Controller.LadyOfTheWood
{
	public class SpringCardController : CardController
    {
		public SpringCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}
		public override void AddTriggers()
		{
			//Whenever LadyOfTheWood deals toxic damage to a target, she regains that much HP.
			base.AddTrigger<DealDamageAction>((DealDamageAction dd) => dd.DamageSource.IsSameCard(base.CharacterCard) && dd.DamageType == DamageType.Toxic, (DealDamageAction dd) => base.GameController.GainHP(base.CharacterCard, new int?(dd.Amount), null, null, base.GetCardSource(null)), new TriggerType[]
			{
				TriggerType.GainHP
			}, TriggerTiming.After, null, false, true, null, false, new bool?(false), null, false, false);
		}
	}
}
