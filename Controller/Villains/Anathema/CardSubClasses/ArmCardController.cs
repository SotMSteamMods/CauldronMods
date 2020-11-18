using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.Anathema
{
    public class ArmCardController : AnathemaCardController
    {

        public ArmCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

		public override IEnumerator Play()
		{
			if (base.GetNumberOfArmsInPlay() > 2)
			{
				//Determine the arm with the highest HP
				List<Card> highestArm = new List<Card>();
				IEnumerator coroutine = base.GameController.FindTargetWithHighestHitPoints(1, (Card c) => base.IsArm(c) && c.IsInPlay && c != base.Card, highestArm, cardSource: base.GetCardSource());

				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine);
				}

				//Destroy all other arm cards except for the one with the highest HP.

				IEnumerator coroutine2 = base.GameController.DestroyCards(this.DecisionMaker, new LinqCardCriteria((Card c) => base.IsArm(c) && !highestArm.Contains(c) && c != base.Card, "arm"), cardSource: base.GetCardSource());
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine2);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine2);
				}
			}

			yield break;
		}

	}
}