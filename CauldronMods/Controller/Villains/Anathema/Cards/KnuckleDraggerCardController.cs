using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using Handelabra;
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
			SpecialStringMaker.ShowHeroCharacterCardWithHighestHP();
		}

		public override void AddTriggers()
		{
			//At the end of the Villain Turn, Anathema deals the Hero character with the highest HP {H+1} melee damage. If that Hero takes damage this way, they must destroy 1 of their ongoing cards."

			base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.DealDamageResponse, new TriggerType[]
				{
					TriggerType.DealDamage,
					TriggerType.DestroyCard
				});
		}

		private IEnumerator DealDamageResponse(PhaseChangeAction phaseChange)
		{
			//Anathema deals the Hero character with the highest HP {H+1} melee damage..
			List<DealDamageAction> storedResults = new List<DealDamageAction>();
			IEnumerator coroutine = base.DealDamageToHighestHP(base.CharacterCard, 1, (Card c) =>  IsHeroCharacterCard(c), (Card c) => new int?(base.H + 1), DamageType.Melee, storedResults: storedResults);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			DealDamageAction dd = storedResults.FirstOrDefault();
			//If that Hero takes damage this way, they must destroy 1 of their ongoing cards.
			if (dd != null && dd.DidDealDamage && DidIntendedTargetTakeDamage(new DealDamageAction[] { dd }, dd.OriginalTarget))
			{
				HeroTurnTakerController httc = base.FindHeroTurnTakerController(dd.Target.Owner.ToHero());
				coroutine = base.GameController.SelectAndDestroyCard(httc, new LinqCardCriteria((Card c) => IsOngoing(c) && c.Owner == dd.Target.Owner, "ongoing"), false, cardSource: base.GetCardSource());
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
