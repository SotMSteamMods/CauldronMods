using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Linq;
using System.Collections;

namespace SotMWorkshop.Controller.LadyOfTheWood
{
	public class SerenityOfDawnController : CardController
    {
		public SerenityOfDawnController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}
		public override void AddTriggers()
		{
			base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(this.EndOfTurnResponse), new TriggerType[]
			{
				TriggerType.GainHP,
				TriggerType.DrawCard
			}, null, false);
			base.AddTrigger<DealDamageAction>((DealDamageAction dd) => dd.DamageSource.IsSameCard(base.CharacterCard) && dd.Amount >= 3, new Func<DealDamageAction, IEnumerator>(this.ExcessDamageResponse), new TriggerType[]
			{
				TriggerType.DestroySelf
			}, TriggerTiming.After, null, false, true, null, false, null, null, false, false);
		}

		private IEnumerator EndOfTurnResponse(PhaseChangeAction p)
		{
			if (this.GetDamageDealtByLadyOfTheWoodThisTurn() == 0)
			{
				IEnumerator coroutine = base.GameController.SendMessageAction("Lady of the Wood has not dealt any damage this turn. She regains 2 HP and may draw a card", Priority.High, base.GetCardSource(null), null, false);
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine);
				}
				int powerNumeral = base.GetPowerNumeral(1, 2);
				IEnumerator coroutine2 = base.GameController.GainHP(base.CharacterCard, new int?(powerNumeral), null, null, base.GetCardSource(null));
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine2);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine2);
				}
				IEnumerator coroutine3 = base.DrawCards(this.DecisionMaker, 1, false, true, null, false, null);
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine3);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine3);
				}
			}
			yield break;
		}

		private IEnumerator ExcessDamageResponse(DealDamageAction dd)
		{
			IEnumerator coroutine = base.GameController.DestroyCard(this.DecisionMaker, base.Card, false, null, base.CharacterCard.Title + " has dealt 3 or more damage this turn. Destroying " + base.Card.Title + ".", null, null, null, null, null, null, base.GetCardSource(null));
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

		private int GetDamageDealtByLadyOfTheWoodThisTurn()
		{
			return (from e in base.GameController.Game.Journal.DealDamageEntriesThisTurn()
					where e.SourceCard != null && e.SourceCard == base.CharacterCard
					select e.Amount).Sum();
		}
	}
}
