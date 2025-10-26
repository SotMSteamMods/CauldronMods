using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.LadyOfTheWood
{
	public class SnowshadeGownCardController : CardController
    {
		public SnowshadeGownCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}
		public override void AddTriggers()
		{
			//Whenever LadyOfTheWood regains HP, you may select a target that has not been dealt damage this turn. LadyOfTheWood deals that target 1 cold damage.
			base.AddTrigger<GainHPAction>((GainHPAction hp) => hp.HpGainer == base.CharacterCard && hp.AmountActuallyGained > 0, new Func<GainHPAction, IEnumerator>(this.DealDamageResponse), TriggerType.WouldGainHP, TriggerTiming.After);
		}

		public override IEnumerator UsePower(int index = 0)
		{
			//LadyOfTheWood regains 3 HP.
			int powerNumeral = base.GetPowerNumeral(0, 3);
			IEnumerator coroutine = base.GameController.GainHP(base.CharacterCard, new int?(powerNumeral), cardSource: base.GetCardSource());
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
			//you may select a target that has not been dealt damage this turn- LadyOfTheWood deals that target 1 cold damage.

			IEnumerator coroutine = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), 1, DamageType.Cold, new int?(1), true, new int?(1), additionalCriteria: (Card c) => !HasInstanceBeenDealtDamageThisTurn(c), cardSource: GetNonPowerCardSource());
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

		private CardSource GetNonPowerCardSource(StatusEffect statusEffectSource = null)
        {
			bool? isFlipped = CardWithoutReplacements.IsFlipped;
			if (AllowActionsFromOtherSide)
			{
				isFlipped = null;
			}
			Power powerSource = null;
			List<string> villainCharacterIdentifiers = new List<string>();
			CardSource cardSource = new CardSource(this, isFlipped, canPerformActionsFromOtherSide: false, AssociatedCardSources, powerSource, CardSourceLimitation, AssociatedTriggers, null, villainCharacterIdentifiers, ActionSources, statusEffectSource);
			CardSource cardSource2 = GameController.DoesCardSourceGetReplaced(cardSource);
			if (cardSource2 != null)
			{
				return cardSource2;
			}
			return cardSource;
		}


		// The built in HasBeenDealtDamageThisTurn only factors in whether the card is the same
		// not whether it has the same play index
		// As such, we need a custom function for this
        private bool HasInstanceBeenDealtDamageThisTurn(Card card)
        {
            return Journal.DealDamageEntriesThisTurn().Any(
				(DealDamageJournalEntry e) =>  e.TargetCard == card && 
											   e.TargetPlayIndex == card.PlayIndex && 
											   e.Amount > 0);
        }

    }
}
