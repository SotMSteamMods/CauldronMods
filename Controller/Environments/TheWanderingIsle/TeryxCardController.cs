using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Studio29.TheWanderingIsle
{
	public class TeryxCardController : CardController
	{
		public TeryxCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
			//This card is indestructible
			base.AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
		}

		public override void AddTriggers()
		{
			//If this card reaches 0HP, the heroes lose.
			base.AddTrigger<GameAction>((GameAction a) => !this._doneChecking && base.Card.HitPoints.Value <= 0, new Func<GameAction, IEnumerator>(this.CheckIfGameOverResponse), TriggerType.GameOver, TriggerTiming.After, ActionDescription.Unspecified, false, true, null, false, null, null, false, false);
			//At the end of the environment turn, the villain target with the highest HP deals Teryx { H + 2} energy damage
			base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(this.DealVillainDamageResponse), TriggerType.DealDamage, null, false);
			//Whenever a hero target would deal damage to Teryx, Teryx Instead regains that much HP.
			base.AddPreventDamageTrigger((DealDamageAction dd) => dd.DamageSource.IsTarget && dd.DamageSource.IsHero && dd.Target == base.Card, (DealDamageAction dd) => base.GameController.GainHP(base.Card, new int?(dd.Amount), null, null, base.GetCardSource(null)), new TriggerType[]
			{
				TriggerType.GainHP
			}, true);
		}

		

		private IEnumerator DealVillainDamageResponse(PhaseChangeAction pca)
		{
			//Find the villain target with the highest HP
			List<Card> storedResults = new List<Card>();
			IEnumerator coroutine = base.GameController.FindTargetsWithHighestHitPoints(1, 1, (Card c) => c.IsVillainTarget, storedResults, null, null, false, false, null, false);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}

			if(storedResults != null)
			{
				//that target deals Teryx H + 2 energy damage
				Card highestVillain = storedResults.FirstOrDefault<Card>();
				IEnumerator coroutine2 = base.DealDamage(highestVillain, base.Card, base.H + 2, DamageType.Energy, false, false, false, null, null, null, false, base.GetCardSource(null));
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

		private IEnumerator CheckIfGameOverResponse(GameAction a)
		{
			//so we don't go in an infinite loop
			this._doneChecking = true;

			//the heroes lose
			IEnumerator coroutine = base.GameController.GameOver(EndingResult.EnvironmentDefeat, "Teryx has been destroyed. The island is sinking...", false, null, null, base.GetCardSource(null));
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

		//This card is indestructible
		public override bool AskIfCardIsIndestructible(Card card)
		{
			return card == base.Card;
		}

		private bool _doneChecking;

	}
}
