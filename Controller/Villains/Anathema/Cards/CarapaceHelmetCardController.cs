using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Linq;

namespace Cauldron.Anathema
{
	public class CarapaceHelmetCardController : CardController
    {
		public CarapaceHelmetCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

		public override void AddTriggers()
		{
			//Reduce damage dealt to this card by 1.
			base.AddReduceDamageTrigger((Card c) => c == base.Card, 1);
			//Villain targets are immune to damage dealt by environment cards. 
			base.AddImmuneToDamageTrigger((DealDamageAction dd) => dd.DamageSource.IsEnvironmentCard && base.IsVillainTarget(dd.Target), false);
		}



		public override IEnumerator Play()
		{
			//When this card enters play, destroy all other head cards.
			if (GetNumberOfHeadInPlay() > 1)
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
