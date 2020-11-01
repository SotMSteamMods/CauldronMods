using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Linq;

namespace SotMWorkshop.Controller.Anathema
{
	public class HeavyCarapaceCardController : CardController
    {
		public HeavyCarapaceCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

		public override void AddTriggers()
		{
			//Reduce damage dealt to Villain targets by 2.
			base.AddReduceDamageTrigger((Card c) => base.IsVillainTarget(c), 2);
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
