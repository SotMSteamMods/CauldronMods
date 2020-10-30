using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;

namespace SotMWorkshop.Controller.LadyOfTheWood
{
	public class FireInTheCloudsCardController : CardController
    {
		public FireInTheCloudsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}
		public override IEnumerator Play()
		{
			List<Function> list = new List<Function>();
			list.Add(new Function(this.DecisionMaker, "Deal 1 target 3 fire damage", SelectionType.DealDamage, () => base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), 3, DamageType.Fire, new int?(1), false, new int?(1), false, false, false, null, null, null, null, null, false, null, null, false, null, base.GetCardSource(null)), null, null, "Deal 1 target 3 fire damage"));
			list.Add(new Function(this.DecisionMaker, "Deal up to 3 targets 1 lightning damage each", SelectionType.DealDamage, () => base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), 1, DamageType.Lightning, new int?(3), false, new int?(0), false, false, false, null, null, null, null, null, false, null, null, false, null, base.GetCardSource(null)), new bool?(true), null, "Deal up to 3 targets 1 lightning damage each"));
			SelectFunctionDecision selectFunction = new SelectFunctionDecision(base.GameController, this.DecisionMaker, list, false, null, null, null, base.GetCardSource(null));
			IEnumerator coroutine = base.GameController.SelectAndPerformFunction(selectFunction, null, null);
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
