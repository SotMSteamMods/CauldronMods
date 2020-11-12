using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.LadyOfTheWood
{
	public class FrostOnThePetalsCardController : CardController
    {
		public FrostOnThePetalsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}
		public override IEnumerator Play()
		{
			//LadyOfTheWood deals 1 target 3 toxic damage or up to 3 targets 1 cold damage each.
			List<Function> list = new List<Function>();

			string option1Message = "Deal 1 target 3 toxic damage";
			IEnumerator option1Effect = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), 3, DamageType.Toxic, new int?(1), false, new int?(1), cardSource: base.GetCardSource());
			list.Add(new Function(this.DecisionMaker, option1Message, SelectionType.DealDamage, () => option1Effect, repeatDecisionText: option1Message));

			string option2Message = "Deal up to 3 targets 1 cold damage each";
			IEnumerator option2Effect = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), 1, DamageType.Cold, new int?(3), false, new int?(0), cardSource: base.GetCardSource());
			list.Add(new Function(this.DecisionMaker, option2Message, SelectionType.DealDamage, () => option2Effect, repeatDecisionText: option2Message));

			SelectFunctionDecision selectFunction = new SelectFunctionDecision(base.GameController, this.DecisionMaker, list, false, cardSource: base.GetCardSource());
			IEnumerator coroutine = base.GameController.SelectAndPerformFunction(selectFunction);
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
	}
}
