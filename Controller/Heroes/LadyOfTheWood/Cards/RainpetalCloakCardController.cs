using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.LadyOfTheWood
{
	public class RainpetalCloakCardController : CardController
	{
		public RainpetalCloakCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

		public override void AddTriggers()
		{
			//The first time LadyOfTheWood would be dealt 1 damage each turn, she regains 1 HP instead.
			Func<DealDamageAction, bool> criteria = (DealDamageAction dd) => dd.Target == base.CharacterCard && dd.Amount == 1 && !base.IsPropertyTrue(base.GeneratePerTargetKey("FirstDamageTakenThisTurn", dd.Target));

			base.AddPreventDamageTrigger(criteria, new Func<DealDamageAction, IEnumerator>(this.PreventDamageResponse), new TriggerType[]
			{
				TriggerType.GainHP
			}, true);

			AddAfterLeavesPlayAction((GameAction ga) => ResetFlagsAfterLeavesPlay("FirstDamageTakenThisTurn"), TriggerType.Hidden);
		}


		public override IEnumerator UsePower(int index = 0)
		{
			//Draw a card.
			IEnumerator coroutine = base.DrawCard();
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
			//Indicate that this damage has been taken and regain 1 HP
			base.SetCardPropertyToTrueIfRealAction(base.GeneratePerTargetKey("FirstDamageTakenThisTurn", dd.Target));
			IEnumerator coroutine = base.GameController.GainHP(base.CharacterCard, new int?(dd.Amount), cardSource: base.GetCardSource());
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
