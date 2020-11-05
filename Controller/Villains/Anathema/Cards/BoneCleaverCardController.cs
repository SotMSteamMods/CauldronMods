using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Anathema
{
	public class BoneCleaverCardController : CardController
    {
		public BoneCleaverCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

		public override void AddTriggers()
		{
			//At the end of the Villain Turn, Anathema deals the Hero target with the lowest HP {H-2} melee damage. If that target took damage this way, it cannot deal damage until the start of the next Villain Turn.

			base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(this.DealDamageResponse), TriggerType.DealDamage, null, false);
		}

		private IEnumerator DealDamageResponse(PhaseChangeAction phaseChange)
		{
			//Anathema deals the Hero target with the lowest HP {H-2} melee damage.
			//If that target took damage this way, it cannot deal damage until the start of the next Villain Turn.
			IEnumerator coroutine = base.DealDamageToLowestHP(base.CharacterCard, 1, (Card c) => c.IsHero, (Card c) => new int?(base.H - 2), DamageType.Melee, false, false, null, 1, null, new Func<DealDamageAction, IEnumerator>(base.TargetsDealtDamageCannotDealDamageUntilTheStartOfNextTurnResponse), false);
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

		public override IEnumerator Play()
		{
			if(GetNumberOfArmsInPlay() > 2)
			{
				//Determine the arm with the highest HP
				List<Card> highestArm = new List<Card>();
				IEnumerator coroutine = base.GameController.FindTargetWithHighestHitPoints(1, (Card c) => this.IsArm(c) && c.IsInPlay && c != base.Card, highestArm, null, null, false, false, null, false, base.GetCardSource(null));

				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine);
				}
				//Destroy all other arm cards except for the one with the highest HP.

				IEnumerator coroutine2 = base.GameController.DestroyCards(this.DecisionMaker, new LinqCardCriteria((Card c) => this.IsArm(c) && !highestArm.Contains(c) && c != base.Card, "arm", true, false, null, null, false), false, null, null, null, SelectionType.DestroyCard, base.GetCardSource(null));
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine2);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine2);
				}
			}
			
			yield break;
		}

		private bool IsArm(Card card)
		{
			return card != null && base.GameController.DoesCardContainKeyword(card, "arm", false, false);
		}

		private int GetNumberOfArmsInPlay()
		{
			return base.FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && this.IsArm(c), false, null, false).Count<Card>();
		}

	}
}
