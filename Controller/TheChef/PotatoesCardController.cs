using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace SotMWorkshop.Controller.TheChef
{
	public class PotatoesCardController : IngredientsCardController
	{
		public PotatoesCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
			
		}

		public override void AddTriggers()
		{
			base.AddTriggers();
		}

		public override IEnumerator UsePower(int index = 0)
		{
			int amountToDeal = 1;
			if (this._garnishActive)
			{
				amountToDeal += 2;
			}
			int powerNumeral = base.GetPowerNumeral(0, 1);
			int powerNumeral2 = base.GetPowerNumeral(1, amountToDeal);

			IEnumerator coroutine = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(base.GameController, base.Card), powerNumeral2, DamageType.Melee, new int?(powerNumeral), false, new int?(powerNumeral), false, false, false, null, null, null, null, null, false, null, null, false, null, base.GetCardSource(null));
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			yield break;
		}
	}
}
