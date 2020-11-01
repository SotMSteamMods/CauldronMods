using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Linq;

namespace Cauldron.Anathema
{
	public class ReflexBoosterCardController : CardController
    {
		public ReflexBoosterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

		public override void AddTriggers()
		{
			//At the end of the Villain Turn, play the top card of the Villain Deck.
			base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(base.PlayTheTopCardOfTheVillainDeckWithMessageResponse), TriggerType.PlayCard, null, false);
		}



		public override IEnumerator Play()
		{
			//When this card enters play, destroy all other head cards.
			if(GetNumberOfHeadInPlay() > 1)
			{
				IEnumerator coroutine = base.GameController.DestroyCards(this.DecisionMaker, new LinqCardCriteria((Card c) => this.IsHead(c) && c != base.Card, "head", true, false, null, null, false), false, null, null, null, SelectionType.DestroyCard, base.GetCardSource(null));
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

		private bool IsHead(Card card)
		{
			return card != null && base.GameController.DoesCardContainKeyword(card, "head", false, false);
		}

		private int GetNumberOfHeadInPlay()
		{
			return base.FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && this.IsHead(c), false, null, false).Count<Card>();
		}

	}
}
