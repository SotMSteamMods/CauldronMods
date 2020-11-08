using System;
using System.Collections;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Tiamat
{
    public class ElementalFormCardController : CardController
    {
        #region Constructors

        public ElementalFormCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

		#endregion Constructors

		#region Methods

		public override void AddTriggers()
		{
			//Whenever a Head takes damage, it becomes immune to damage of that type until the start of the next villain turn.
			CreateTriggers(base.TurnTaker.FindCard("InfernoTiamatCharacter"));
			CreateTriggers(base.TurnTaker.FindCard("StormTiamatCharacter"));
			CreateTriggers(base.TurnTaker.FindCard("WinterTiamatCharacter"));
		}

		private void CreateTriggers(Card characterCard)
		{
			base.AddTrigger<DealDamageAction>((DealDamageAction dealDamage) => dealDamage.Target == characterCard, new Func<DealDamageAction, IEnumerator>(this.ImmuneResponse), TriggerType.ImmuneToDamage, TriggerTiming.After, ActionDescription.DamageTaken, false, true, null, false, null, null, false, false);
			base.AddTrigger<DealDamageAction>((DealDamageAction dealDamage) => characterCard.IsInPlayAndHasGameText && dealDamage.Target == characterCard && dealDamage.DidDealDamage, new Func<DealDamageAction, IEnumerator>(this.WarningMessageResponse), TriggerType.Hidden, TriggerTiming.After, ActionDescription.Unspecified, false, true, null, false, null, null, false, false);
		}

		private IEnumerator WarningMessageResponse(DealDamageAction dealDamage)
		{
			if (base.IsFirstOrOnlyCopyOfThisCardInPlay())
			{
				IEnumerator coroutine = base.GameController.SendMessageAction("Tiamat used Elemental Form to become immune to " + dealDamage.DamageType.ToString() + " damage.", Priority.High, base.GetCardSource(), null, false);
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine);
				}
				Log.Debug("Tiamat used Elemental Form to become immune to " + dealDamage.DamageType.ToString() + " damage. ");
			}
			yield return null;
			yield break;
		}

		private IEnumerator ImmuneResponse(DealDamageAction dealDamage)
		{
			DamageType value = dealDamage.DamageType;
				//this.GetDamageTypeThatHeadIsImmuneTo(dealDamage.Target) ?? default;
			ImmuneToDamageStatusEffect immuneToDamageStatusEffect = new ImmuneToDamageStatusEffect();
			immuneToDamageStatusEffect.DamageTypeCriteria.AddType(value);
			immuneToDamageStatusEffect.TargetCriteria.IsSpecificCard = dealDamage.Target;
			immuneToDamageStatusEffect.UntilStartOfNextTurn(base.TurnTaker);
			immuneToDamageStatusEffect.CardDestroyedExpiryCriteria.Card = dealDamage.Target;
			IEnumerator coroutine2 = base.AddStatusEffect(immuneToDamageStatusEffect, true);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine2);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine2);
			}
			yield break;
		}

		#endregion Methods
	}
}