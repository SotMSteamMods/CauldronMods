using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Anathema
{
    public class ArmCardController : AnathemaCardController
    {

        public ArmCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
			SpecialStringMaker.ShowHighestHP(cardCriteria: new LinqCardCriteria((Card c) => IsArm(c) && c.IsInPlayAndHasGameText, "arm"));
        }

		public override IEnumerator Play()
		{
			if (base.GetNumberOfArmsInPlay() > 2)
			{
				IEnumerator coroutine;
				IEnumerable<Card> nonIndestructableOtherArms = GetArmsInPlay().Where(c => c != Card && !GameController.IsCardIndestructible(c));
				if (nonIndestructableOtherArms.Count() > 0)
				{
					//Determine the arm with the highest HP
					List<Card> highestArm = new List<Card>();
					coroutine = base.GameController.FindTargetWithHighestHitPoints(1, (Card c) => nonIndestructableOtherArms.Contains(c), highestArm, cardSource: base.GetCardSource());

					if (base.UseUnityCoroutines)
					{
						yield return base.GameController.StartCoroutine(coroutine);
					}
					else
					{
						base.GameController.ExhaustCoroutine(coroutine);
					}

					//Destroy all other arm cards except for the one with the highest HP.

					coroutine = base.GameController.DestroyCards(this.DecisionMaker, new LinqCardCriteria((Card c) => nonIndestructableOtherArms.Contains(c) && !highestArm.Contains(c), "arm"), cardSource: base.GetCardSource());
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
					coroutine = GameController.SendMessageAction("All other arms in play are indestructible.", Priority.Medium, GetCardSource(), showCardSource: true);
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