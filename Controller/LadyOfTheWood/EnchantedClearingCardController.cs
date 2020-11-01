using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.LadyOfTheWood
{
	public class EnchantedClearingCardController : CardController
    {
		public EnchantedClearingCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}
		public override void AddTriggers()
		{
			//Reduce damage dealt to Lady of the Wood by 1
			base.AddReduceDamageTrigger((Card c) => c == base.CharacterCard, 1);
		}
	}
}
