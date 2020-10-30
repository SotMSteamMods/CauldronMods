using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace SotMWorkshop.Controller.TheChef
{
	public abstract class IngredientsCardController : CardController
	{
		public IngredientsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
			this._garnishActive = false;
		}


		public override void AddTriggers()
		{
			base.AddTrigger<UsePowerAction>((UsePowerAction p) => p.Power.CardController.Card == base.CharacterCard, new Func<UsePowerAction, IEnumerator>(this.AfterPowerResponse), TriggerType.UsePower, TriggerTiming.After, ActionDescription.Unspecified, false, true, null, false, null, null, false, true);
		}

		public IEnumerator UseGarnish(Card c)
		{
			if (!this._garnishActive)
			{
				this._garnishActive = true;
				IEnumerator coroutine = base.GameController.SendMessageAction("The garnish has been called on!", Priority.High, base.GetCardSource(null), null, false);
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine);
				}

				IEnumerator coroutine2 = base.FindCardController(c).UsePower(0);
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
		protected virtual IEnumerator AfterPowerResponse(UsePowerAction action)
		{
			if (this._garnishActive)
			{
				this._garnishActive = false;
				IEnumerator coroutine = base.GameController.SendMessageAction("Dinner has been served!", Priority.High, base.GetCardSource(null), null, false);
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine);
				}
			}
			yield break;
		}

		protected bool _garnishActive;

	}
}
