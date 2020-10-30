using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Controller.Guise;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SotMWorkshop.Controller.TheChef
{
	public class TheChefCharacterCardController : HeroCharacterCardController
	{
		public TheChefCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}
		public override IEnumerator UsePower(int index = 0)
		{
			List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
			int powerNumeral = base.GetPowerNumeral(0, 1);
			LinqCardCriteria cardCriteria = new LinqCardCriteria((Card c) => this.IsIngredient(c) && c.IsInPlay, "ingredient", true, false, null, null, false);
			IEnumerator coroutine = base.GameController.SelectCardsAndDoAction(this.DecisionMaker, cardCriteria, SelectionType.ActivateAbility, (Card c) => this.UseGarnishOnCard(c), new int?(powerNumeral), false, null, storedResults, false, null, base.GetCardSource(null), false);
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
		public override IEnumerator UseIncapacitatedAbility(int index)
		{
			switch (index)
			{
				case 0:
					{

						break;
					}
				case 1:
					{
						break;
					}
				case 2:
					{
		
						break;
					}
			}
			yield break;
		}

		private IEnumerator UseGarnishOnCard(Card c)
		{
			IEnumerator coroutine = ((IngredientsCardController)base.FindCardController(c)).UseGarnish(c);
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
		
		private bool IsIngredient(Card card)
		{
			return card != null && base.GameController.DoesCardContainKeyword(card, "ingredient", false, false);
		}


	}
}
