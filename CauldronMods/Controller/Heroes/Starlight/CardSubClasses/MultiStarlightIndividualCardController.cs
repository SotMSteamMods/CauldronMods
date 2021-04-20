using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.Starlight
{
    public class MultiStarlightIndividualCardController : StarlightSubCharacterCardController
	{

        public MultiStarlightIndividualCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }


		public override IEnumerator BeforeFlipCardImmediateResponse(FlipCardAction flip)
		{
			if (!base.CardWithoutReplacements.IsFlipped)
			{
				RemoveTargetAction action = new RemoveTargetAction(base.GameController, base.CardWithoutReplacements);
				IEnumerator coroutine = DoAction(action);
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(coroutine);
				}
				else
				{
					base.GameController.ExhaustCoroutine(coroutine);
				}
				RemoveAllTriggers();
			}
		}
	}
}