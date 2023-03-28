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
			base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.DealDamageResponse, TriggerType.DealDamage);
		}

		private IEnumerator DealDamageResponse(PhaseChangeAction phaseChange)
		{
			//Anathema deals each Hero target 2 projectile damage. 
			List<DealDamageAction> storedResults = new List<DealDamageAction>();
			IEnumerator coroutine = base.DealDamage(base.CharacterCard, (Card card) => IsHeroTarget(card), 2, DamageType.Projectile, storedResults: storedResults);
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
				if(dd.DidDealDamage && IsHeroCharacterCard(dd.Target))
				{
					HeroTurnTakerController httc = base.FindHeroTurnTakerController(dd.Target.Owner.ToHero());
					coroutine = base.GameController.SelectAndDestroyCard(httc, new LinqCardCriteria((Card c) => base.IsEquipment(c) && c.Owner == dd.Target.Owner, "equipment"), false, cardSource: base.GetCardSource());
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
