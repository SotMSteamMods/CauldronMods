using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.LadyOfTheWood
{
	public class NobilityOfDuskCardController : CardController
	{
		public NobilityOfDuskCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
			this.AllowFastCoroutinesDuringPretend = false;
			this.RunModifyDamageAmountSimulationForThisCard = false;
			base.SetCardProperty(base.GeneratePerTargetKey("BuffUsed", base.CharacterCard), false);
		}
		public override void AddTriggers()
		{
			//Once per turn when LadyOfTheWood would deal damage you may increase that damage by 2
			Func<DealDamageAction, bool> criteria = (DealDamageAction dd) => dd.DamageSource != null && dd.DamageSource.IsSameCard(base.CharacterCard) && !base.CharacterCardController.IsPropertyTrue("BuffUsed");
			base.AddTrigger<DealDamageAction>(criteria, new Func<DealDamageAction, IEnumerator>(this.IncreaseDamageDecision), TriggerType.ModifyDamageAmount, TriggerTiming.Before);
		}

		private DealDamageAction DealDamageAction { get; set; }

		private IEnumerator IncreaseFunction()
		{
			//Increase the damage by 2
			IEnumerator coroutine = base.GameController.IncreaseDamage(this.DealDamageAction, 2, cardSource: base.GetCardSource());
						
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
			//Offer the player a yes/no decision if they want to increase that damage by 2
			//This currently doesn't have any text on the decision other than yes/no, room for improvement
			this.DealDamageAction = dd;
			List<YesNoCardDecision> storedResults = new List<YesNoCardDecision>();
			
			IEnumerator coroutine = base.GameController.MakeYesNoCardDecision(base.HeroTurnTakerController, SelectionType.IncreaseNextDamage, base.Card,storedResults: storedResults,cardSource: base.GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			if (base.DidPlayerAnswerYes(storedResults))
			{
                //if player said yes, set BuffUsed to true and Increase Damage

                base.CharacterCardController.SetCardPropertyToTrueIfRealAction("BuffUsed");
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
	}
}
