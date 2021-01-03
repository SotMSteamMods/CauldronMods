using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Linq;

namespace Cauldron.Anathema
{
	public class HeavyCarapaceCardController : BodyCardController
    {
		public HeavyCarapaceCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

		public override void AddTriggers()
		{
			//Reduce damage dealt to Villain targets by 2.
			base.AddReduceDamageTrigger((Card c) => base.IsVillainTarget(c), 2);
		}


	}
}
