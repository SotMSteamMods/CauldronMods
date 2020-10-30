using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace SotMWorkshop.Controller.LadyOfTheWood
{
	public class NobilityOfDuskCardController : CardController
    {
		public NobilityOfDuskCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
			this.AllowFastCoroutinesDuringPretend = false;
			this.RunModifyDamageAmountSimulationForThisCard = false;
			base.SetCardProperty(base.GeneratePerTargetKey("BuffUsed", base.CharacterCard, null), false);
		}
		public override void AddTriggers()
		{
			base.AddTrigger<DealDamageAction>((DealDamageAction dealDamage) => dealDamage.DamageSource.IsSameCard(base.CharacterCard) && !base.CharacterCardController.IsPropertyTrue("BuffUsed", null), new Func<DealDamageAction, IEnumerator>(this.IncreaseDamageDecision), TriggerType.ModifyDamageAmount, TriggerTiming.Before, ActionDescription.Unspecified, false, true, null, false, null, null, false, false);
			base.AddStartOfTurnTrigger((TurnTaker tt) => tt.CharacterCard == base.CharacterCard, new Func<PhaseChangeAction, IEnumerator>(this.ResetBuffUsed), TriggerType.Other, null, false);
		}

		private DealDamageAction DealDamageAction { get; set; }

		private IEnumerator IncreaseFunction()
		{
			IEnumerator coroutine = base.GameController.IncreaseDamage(this.DealDamageAction, 2, false, base.GetCardSource(null));
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

		private IEnumerator IncreaseDamageDecision(DealDamageAction dd)
		{
			this.DealDamageAction = dd;
			List<Card> list = new List<Card>();
			list.Add(base.Card);
			YesNoDecision yesNo = new YesNoDecision(base.GameController, base.HeroTurnTakerController, SelectionType.IncreaseNextDamage, false, dd, list, base.GetCardSource(null));
			IEnumerator coroutine = base.GameController.MakeDecisionAction(yesNo, true);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			if (yesNo.Answer != null && yesNo.Answer.Value)
			{
				base.CharacterCardController.SetCardPropertyToTrueIfRealAction("BuffUsed", null);
				IEnumerator coroutine2 = this.IncreaseFunction();
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine2);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine2);
				}
			}
			yield break;
		}

		private IEnumerator ResetBuffUsed(PhaseChangeAction pa)
		{
			base.CharacterCardController.SetCardProperty("BuffUsed", false);
			yield break;
		}
	}
}
