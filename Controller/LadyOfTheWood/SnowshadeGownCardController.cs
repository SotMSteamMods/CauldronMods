using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace SotMWorkshop.Controller.LadyOfTheWood
{
	public class SnowshadeGownCardController : CardController
    {
		public SnowshadeGownCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}
		public override void AddTriggers()
		{
			base.AddTrigger<GainHPAction>((GainHPAction hp) => hp.HpGainer == base.CharacterCard, new Func<GainHPAction, IEnumerator>(this.DealDamageResponse), TriggerType.WouldGainHP, TriggerTiming.After, ActionDescription.Unspecified, false, true, null, false, null, null, false, false);
		}

		public override IEnumerator UsePower(int index = 0)
		{
			int powerNumeral = base.GetPowerNumeral(1, 3);
			IEnumerator coroutine = base.GameController.GainHP(base.CharacterCard, new int?(powerNumeral), null, null, base.GetCardSource(null));
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

		private IEnumerator DealDamageResponse(GainHPAction hp)
		{
			int powerNumeral = base.GetPowerNumeral(0, 1);
			IEnumerator coroutine = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), powerNumeral, DamageType.Cold, new int?(powerNumeral), false, new int?(powerNumeral), false, false, false, (Card c) => !base.HasBeenDealtDamageThisTurn(c), null, null, null, null, false, null, null, false, null, base.GetCardSource(null));
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
