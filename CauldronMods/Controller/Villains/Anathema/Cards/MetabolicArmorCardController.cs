using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Linq;

namespace Cauldron.Anathema
{
	public class MetabolicArmorCardController : BodyCardController
    {
		public MetabolicArmorCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

		public override void AddTriggers()
		{
			//Increase damage dealt by Villain targets by 1.
			base.AddIncreaseDamageTrigger((DealDamageAction dd) => dd.DamageSource.IsVillainTarget, (DealDamageAction dd) => 1);

			//At the end of the Villain Turn, all Villain targets regain 1HP.
			base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.GainHpResponse, TriggerType.GainHP);
		}

		private IEnumerator GainHpResponse(PhaseChangeAction p)
		{
			//all Villain targets regain 1HP.
			IEnumerator coroutine = base.GameController.GainHP(this.DecisionMaker, (Card c) => base.IsVillainTarget(c), 1, cardSource: base.GetCardSource());
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
