using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace SotMWorkshop.Controller.LadyOfTheWood
{
	public class EnchantedClearingCardController : CardController
    {
		public EnchantedClearingCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}
		public override void AddTriggers()
		{
			base.AddReduceDamageTrigger((Card c) => c == base.CharacterCard, 1);
		}
	}
}
