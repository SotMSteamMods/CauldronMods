using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
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
				IEnumerator coroutine = base.GameController.DestroyCards(this.DecisionMaker, new LinqCardCriteria((Card c) => base.IsHead(c) && c != base.Card, "head"), cardSource: base.GetCardSource());
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