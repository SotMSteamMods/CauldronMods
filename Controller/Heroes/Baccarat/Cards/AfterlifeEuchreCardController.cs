using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Baccarat
{
    public class AfterlifeEuchreCardController : CardController
    {
        public AfterlifeEuchreCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //Increase the next damage dealt by {Baccarat} by 1, or {Baccarat} deals 1 target 2 toxic damage.
            IEnumerable<Function> functionChoices = new Function[]
            {
				//Increase the next damage dealt by {Baccarat} by 1...
				new Function(base.HeroTurnTakerController, "Increase the next damage dealt by Baccarat by 1", SelectionType.IncreaseNextDamage, () => base.AddStatusEffect(new IncreaseDamageStatusEffect(1){ NumberOfUses = new int?(1) })),

				//...or {Baccarat} deals 1 target 2 toxic damage.
				new Function(base.HeroTurnTakerController, "Baccarat deals 1 target 2 toxic damage", SelectionType.TurnTaker, () => base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), 2, DamageType.Toxic, new int?(1), false, new int?(1),cardSource: base.GetCardSource()))
            };
            SelectFunctionDecision selectFunction = new SelectFunctionDecision(base.GameController, base.HeroTurnTakerController, functionChoices, false);
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