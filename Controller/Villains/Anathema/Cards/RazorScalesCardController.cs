using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Linq;

namespace Cauldron.Anathema
{
	public class RazorScalesCardController : CardController
    {
		public RazorScalesCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

		public override void AddTriggers()
		{
			//The first time a Villain target is dealt damage each turn, this card deals the source of that damage 2 melee damage.
			base.AddTrigger<DealDamageAction>((DealDamageAction dd) => !base.IsPropertyTrue("FirstDamageToVillainTargetThisTurn", null) && dd.DidDealDamage && dd.DamageSource.IsTarget && base.IsVillainTarget(dd.Target), new Func<DealDamageAction, IEnumerator>(this.FirstDamageDealtResponse), TriggerType.DealDamage, TriggerTiming.After, ActionDescription.DamageTaken, false, true, null, false, null, null, false, false);
		}

		private IEnumerator FirstDamageDealtResponse(DealDamageAction dd)
		{
			//this card deals the source of that damage 2 melee damage.
			base.SetCardPropertyToTrueIfRealAction("FirstDamageToVillainTargetThisTurn", null);
			IEnumerator coroutine = base.DealDamage(base.Card, dd.DamageSource.Card, 2, DamageType.Melee, false, false, true, null, null, null, false, null);
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

		private const string FirstDamageToVillainTargetThisTurn = "FirstDamageToVillainTargetThisTurn";

		public override IEnumerator Play()
		{
			//When this card enters play, destroy all other body cards.
			if(GetNumberOfBodyInPlay() > 1)
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
