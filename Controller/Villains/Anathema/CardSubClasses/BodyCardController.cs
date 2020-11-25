using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.Anathema
{
    public class BodyCardController : AnathemaCardController
    {

        public BodyCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

		public override IEnumerator Play()
		{
			//When this card enters play, destroy all other body cards.
			if (base.GetNumberOfBodyInPlay() > 1)
			{
				IEnumerator coroutine = base.GameController.DestroyCards(this.DecisionMaker, new LinqCardCriteria((Card c) => base.IsBody(c) && c != base.Card, "body"), cardSource: base.GetCardSource());
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