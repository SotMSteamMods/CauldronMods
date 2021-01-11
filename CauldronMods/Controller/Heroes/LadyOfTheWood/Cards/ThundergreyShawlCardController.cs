using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.LadyOfTheWood
{
	public class ThundergreyShawlCardController : CardController
	{
		public ThundergreyShawlCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}
		public override void AddTriggers()
		{
			AddTrigger((DealDamageAction dd) => GameController.PretendMode && dd.DamageSource.IsSameCard(base.CharacterCard) && dd.Amount <= 2, AddPreviewIrreducible, TriggerType.Other, TriggerTiming.Before);
			AddTrigger((ReduceDamageAction rd) => rd.DealDamageAction.DamageSource.IsCard && rd.DealDamageAction.DamageSource.IsSameCard(CharacterCard) && rd.DealDamageAction.Amount <= 2,
							RetroactiveIrreducibilityResponse,
							new TriggerType[] { TriggerType.MakeDamageIrreducible },
							TriggerTiming.After);
		}

		public override IEnumerator UsePower(int index = 0)
		{
			//LadyOfTheWood deals up to 2 targets 1 lightning damage each.
			int targets = base.GetPowerNumeral(0, 2);
			int damage = base.GetPowerNumeral(1, 1);
			IEnumerator coroutine = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), damage, DamageType.Lightning, new int?(targets), false, new int?(0), cardSource: base.GetCardSource());
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

		private IEnumerator AddPreviewIrreducible(DealDamageAction dd)
        {
			//doesn't actually cancel out the damage reduction in the preview,
			//but does indicate that the Shawl will do something
			if(dd.Amount <= 2)
            {
				IEnumerator coroutine = GameController.MakeDamageIrreducible(dd, GetCardSource());
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

		private IEnumerator RetroactiveIrreducibilityResponse(ReduceDamageAction rd)
        {
			DealDamageAction dd = rd.DealDamageAction;
			IEnumerator coroutine = GameController.MakeDamageIrreducible(dd, GetCardSource());
			if (base.UseUnityCoroutines)
			{
				yield return GameController.StartCoroutine(coroutine);
			}
			else
			{
				GameController.ExhaustCoroutine(coroutine);
			}

			var reduceActions = dd.DamageModifiers.Where((ModifyDealDamageAction mdd) => mdd is ReduceDamageAction).ToList();

			foreach(ReduceDamageAction mod in reduceActions)
            {
				IncreaseDamageAction restoreDamage = new IncreaseDamageAction(mod.CardSource, dd, mod.AmountToReduce, false);

				//we do our best to make it have as little interaction as possible with things that respond to increasing damage
				//since it's supposed to be retroactive undoing of damage decreases
				restoreDamage.AllowTriggersToRespond = false;
				restoreDamage.CanBeCancelled = false;

				var wasUnincreasable = dd.IsUnincreasable;
				dd.IsUnincreasable = false;

				coroutine = GameController.DoAction(restoreDamage);
				if (base.UseUnityCoroutines)
				{
					yield return GameController.StartCoroutine(coroutine);
				}
				else
				{
					GameController.ExhaustCoroutine(coroutine);
				}

				dd.IsUnincreasable = wasUnincreasable;
            }
			yield break;
        }
		
	}
}
