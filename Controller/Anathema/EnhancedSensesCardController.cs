using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Linq;

namespace SotMWorkshop.Controller.Anathema
{
	public class EnhancedSensesCardController : CardController
    {
		public EnhancedSensesCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

		public override void AddTriggers()
		{
			//Change the type of all damage to sonic.
			base.AddChangeDamageTypeTrigger((DealDamageAction dd) => true, DamageType.Sonic);
			//Whenever a Hero card enters play, this card deals that Hero Character 1 sonic damage.
			base.AddTargetEntersPlayTrigger((Card c) => c.IsHero, 
				(Card c) => base.DealDamage(base.Card, c.Owner.CharacterCard,1,DamageType.Sonic,false,false,false,null,null,null,false,base.GetCardSource(null)),
				TriggerType.DealDamage, TriggerTiming.After, false, false);			
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
