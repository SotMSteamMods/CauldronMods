using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Anathema
{
	public class WhipTendrilCardController : ArmCardController
    {
		public WhipTendrilCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

		public override void AddTriggers()
		{
			//At the end of the Villain Turn, Anathema deals each Hero target 2 projectile damage. Heroes that take damage this way must destroy 1 of their equipment cards.
			base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(this.DealDamageResponse), TriggerType.DealDamage, null, false);
		}

		private IEnumerator DealDamageResponse(PhaseChangeAction phaseChange)
		{
			//Anathema deals each Hero target 2 projectile damage. 
			List<DealDamageAction> storedResults = new List<DealDamageAction>();
			IEnumerator coroutine = base.DealDamage(base.Card, (Card card) => card.IsHero, 2, DamageType.Projectile, false, false, storedResults, null, null, false, null, null, false, false);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			//Heroes that take damage this way must destroy 1 of their equipment cards.
			foreach(DealDamageAction dd in storedResults)
			{
				if(dd.DidDealDamage && dd.Target.IsHeroCharacterCard)
				{
					coroutine = base.GameController.SelectAndDestroyCard(base.FindHeroTurnTakerController(dd.Target.Owner.ToHero()), new LinqCardCriteria((Card c) => base.IsEquipment(c, false) && c.Owner == dd.Target.Owner, "equipment", true, false, null, null, false), false, null, null, base.GetCardSource(null));
					if (base.UseUnityCoroutines)
					{
						yield return base.GameController.StartCoroutine(coroutine);
					}
					else
					{
						base.GameController.ExhaustCoroutine(coroutine);
					}
				}
			}

			yield break;
		}
	}
}
