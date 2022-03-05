using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
				IEnumerator coroutine;
				IEnumerable<Card> nonIndestructableOtherBodies = GetBodiesInPlay().Where(c => c != Card && !GameController.IsCardIndestructible(c));
				if (nonIndestructableOtherBodies.Count() > 0)
				{
					coroutine = base.GameController.DestroyCards(this.DecisionMaker, new LinqCardCriteria((Card c) => nonIndestructableOtherBodies.Contains(c), "body"), cardSource: base.GetCardSource());
					if (base.UseUnityCoroutines)
					{
						yield return base.GameController.StartCoroutine(coroutine);
					}
					else
					{
						base.GameController.ExhaustCoroutine(coroutine);
					}
				}
				else
				{
					coroutine = GameController.SendMessageAction("All other bodies in play are indestructible.", Priority.Medium, GetCardSource(), showCardSource: true);
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