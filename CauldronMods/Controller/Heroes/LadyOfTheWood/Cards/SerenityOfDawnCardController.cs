using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Linq;
using System.Collections;

namespace Cauldron.LadyOfTheWood
{
	public class SerenityOfDawnCardController : CardController
    {
		public SerenityOfDawnCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}
		public override void AddTriggers()
		{
			//At the end of your turn, if LadyOfTheWood dealt no damage this turn, she regains 2 HP and you may draw a card.
			base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(this.EndOfTurnResponse), new TriggerType[]
			{
				TriggerType.GainHP,
				TriggerType.DrawCard
			});

			//If LadyOfTheWood deals 3 or more damage to a target, destroy this card.
			Func<DealDamageAction, bool> criteria = (DealDamageAction dd) => dd.DamageSource.IsSameCard(base.CharacterCard) && dd.Amount >= 3;
			base.AddTrigger<DealDamageAction>(criteria, new Func<DealDamageAction, IEnumerator>(this.ExcessDamageResponse), new TriggerType[]{ TriggerType.DestroySelf }, TriggerTiming.After);
		}


		private IEnumerator EndOfTurnResponse(PhaseChangeAction p)
		{
			//if LadyOfTheWood dealt no damage this turn, 
			if (this.GetDamageDealtByLadyOfTheWoodThisTurn() == 0)
			{
				IEnumerator coroutine = base.GameController.SendMessageAction("Lady of the Wood has not dealt any damage this turn. She regains 2 HP and may draw a card", Priority.High, base.GetCardSource());
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine);
				}

				//she regains 2 HP
				IEnumerator coroutine2 = base.GameController.GainHP(base.CharacterCard, new int?(2), cardSource: base.GetCardSource());
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine2);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine2);
				}

				//you may draw a card.
				IEnumerator coroutine3 = base.DrawCards(this.DecisionMaker, 1, upTo: true);
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
			//destroy this card.
			IEnumerator coroutine = base.GameController.DestroyCard(this.DecisionMaker, base.Card, overrideOutput: base.CharacterCard.Title + " has dealt 3 or more damage this turn. Destroying " + base.Card.Title + ".", cardSource: base.GetCardSource());
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

		/// <summary>
		/// Get the amount of damage dealt by Lady of the Wood this turn
		/// </summary>
		/// <returns></returns>
		private int GetDamageDealtByLadyOfTheWoodThisTurn()
		{
			return (from e in base.GameController.Game.Journal.DealDamageEntriesThisTurn()
					where e.SourceCard != null && e.SourceCard == base.CharacterCard
					select e.Amount).Sum();
		}
	}
}
