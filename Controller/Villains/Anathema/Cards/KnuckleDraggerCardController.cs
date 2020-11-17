using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Anathema
{
	public class KnuckleDraggerCardController : ArmCardController
    {
		public KnuckleDraggerCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

		public override void AddTriggers()
		{
			//At the end of the Villain Turn, Anathema deals the Hero character with the highest HP {H+1} melee damage. If that Hero takes damage this way, they must destroy 1 of their ongoing cards."

			base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(this.DealDamageResponse), TriggerType.DealDamage, null, false);
		}

		private IEnumerator DealDamageResponse(PhaseChangeAction phaseChange)
		{
			//Anathema deals the Hero character with the highest HP {H+1} melee damage..
			List<DealDamageAction> storedResults = new List<DealDamageAction>();
			IEnumerator coroutine = base.DealDamageToHighestHP(base.CharacterCard, 1, (Card c) => c.IsHeroCharacterCard, (Card c) => new int?(base.H + 1), DamageType.Melee, false, false, storedResults, null, null, null, false, false);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			DealDamageAction dd = storedResults.FirstOrDefault<DealDamageAction>();
			//If that Hero takes damage this way, they must destroy 1 of their ongoing cards.
			if (dd != null && dd.DidDealDamage)
			{
				coroutine = base.GameController.SelectAndDestroyCard(base.FindHeroTurnTakerController(dd.Target.Owner.ToHero()), new LinqCardCriteria((Card c) => c.IsOngoing && c.Owner == dd.Target.Owner, "ongoing", true, false, null, null, false), false, null, null, base.GetCardSource(null));
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
	}
}
