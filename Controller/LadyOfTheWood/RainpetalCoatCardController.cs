using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace SotMWorkshop.Controller.LadyOfTheWood
{
	public class RainpetalCoatCardController : CardController
	{
		public RainpetalCoatCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

		public override void AddTriggers()
		{
			base.AddPreventDamageTrigger((DealDamageAction dd) => dd.Target == base.CharacterCard && dd.Amount == 1 && !base.IsPropertyTrue(base.GeneratePerTargetKey("FirstDamageTakenThisTurn", dd.Target, null), null), new Func<DealDamageAction, IEnumerator>(this.PreventDamageResponse), new TriggerType[]
			{
				TriggerType.GainHP
			}, true);
		}

		public override IEnumerator UsePower(int index = 0)
		{
			IEnumerator coroutine = base.DrawCard(null, false, null, true);
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

		private IEnumerator PreventDamageResponse(DealDamageAction dd)
		{
			base.SetCardPropertyToTrueIfRealAction(base.GeneratePerTargetKey("FirstDamageTakenThisTurn", dd.Target, null), null);
			IEnumerator coroutine = base.GameController.GainHP(base.CharacterCard, new int?(dd.Amount), null, null, base.GetCardSource(null));
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
