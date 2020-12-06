using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.TheCybersphere
{
    public class SystemCrashCardController : TheCybersphereCardController
    {

        public SystemCrashCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
			SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria((Card c) => IsGridVirus(c)));
			SpecialStringMaker.ShowSpecialString(() => "[b]You are in danger of being deleted![/b]").Condition = () => GetNumberOfGridVirusesInPlay() >= 4;
		}

		public override void AddTriggers()
        {
			//At the start of the environment turn, if there are at least 4 Grid Virus cards in play, everyone is deleted. [b]Game Over.[/b]
			AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, CheckIfGameOverResponse, TriggerType.GameOver);
		}

		private IEnumerator CheckIfGameOverResponse(PhaseChangeAction phaseChange)
		{
			//if there are at least 4 Grid Virus cards in play, everyone is deleted. [b]Game Over.[/b]
			if (GetNumberOfGridVirusesInPlay() >= 4)
			{
				IEnumerator coroutine = base.GameController.GameOver(EndingResult.EnvironmentDefeat, "Everyone is deleted.", cardSource: GetCardSource());
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

	}
}