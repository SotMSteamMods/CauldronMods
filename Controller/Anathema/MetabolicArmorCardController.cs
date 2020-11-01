using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Linq;

namespace SotMWorkshop.Controller.Anathema
{
	public class MetabolicArmorCardController : CardController
    {
		public MetabolicArmorCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

		public override void AddTriggers()
		{
			//Increase damage dealt by Villain targets by 1.
			base.AddIncreaseDamageTrigger((DealDamageAction dd) => dd.DamageSource.IsVillainTarget, (DealDamageAction dd) => 1, false);

			//At the end of the Villain Turn, all Villain targets regain 1HP.
			base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(this.GainHpResponse), TriggerType.GainHP, null, false);
		}

		private IEnumerator GainHpResponse(PhaseChangeAction p)
		{
			//all Villain targets regain 1HP.
			IEnumerator coroutine = base.GameController.GainHP(this.DecisionMaker, (Card c) => base.IsVillainTarget(c), 1, null, false, null, null, null, base.GetCardSource(null));
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
			//When this card enters play, destroy all other body cards.
			if (GetNumberOfBodyInPlay() > 1)
			{
				IEnumerator coroutine = base.GameController.DestroyCards(this.DecisionMaker, new LinqCardCriteria((Card c) => this.IsBody(c) && c != base.Card, "body", true, false, null, null, false), false, null, null, null, SelectionType.DestroyCard, base.GetCardSource(null));
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

		private bool IsBody(Card card)
		{
			return card != null && base.GameController.DoesCardContainKeyword(card, "body", false, false);
		}
		private int GetNumberOfBodyInPlay()
		{
			return base.FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && this.IsBody(c), false, null, false).Count<Card>();
		}

	}
}
