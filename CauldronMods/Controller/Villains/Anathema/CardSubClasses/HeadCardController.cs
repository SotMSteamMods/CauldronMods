using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.Anathema
{
    public class HeadCardController : AnathemaCardController
    {

        public HeadCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

		public override IEnumerator Play()
		{
			//When this card enters play, destroy all other head cards.
			if (base.GetNumberOfHeadInPlay() > 1)
			{
				IEnumerator coroutine;
				IEnumerable<Card> nonIndestructableOtherHeads = GetHeadsInPlay().Where(c => c != Card && !GameController.IsCardIndestructible(c));
				if (nonIndestructableOtherHeads.Count() > 0)
				{
					coroutine = base.GameController.DestroyCards(this.DecisionMaker, new LinqCardCriteria((Card c) => nonIndestructableOtherHeads.Contains(c), "head"), cardSource: base.GetCardSource());
					if (base.UseUnityCoroutines)
					{
						yield return base.GameController.StartCoroutine(coroutine);
					}
					else
					{
						base.GameController.ExhaustCoroutine(coroutine);
					}
				} else
                {
					coroutine = GameController.SendMessageAction("All other heads in play are indestructible.", Priority.Medium, GetCardSource(), showCardSource: true);
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