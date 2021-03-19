using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Gargoyle
{
    /*                
     *  Draw 2 cards.
     *  Reduce the next damage dealt by {Gargoyle} by X, where X is up to 3.
     *  {Gargoyle} deals 1 target 1 toxic damage. If that target takes damage this way, {Gargoyle} deals X other targets 2 toxic damage each."
     */
    public class DreamcatcherCardController : GargoyleUtilityCardController
    {
        public DreamcatcherCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowSpecialString(TotalNextDamageBoostString);
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;
            IEnumerable<Function> functionChoices;
            SelectFunctionDecision selectFunction;
            List<SelectFunctionDecision> storedResults = new List<SelectFunctionDecision>();
            List<SelectCardDecision> storedCardResults = new List<SelectCardDecision>();
            List<DealDamageAction> storedResultsDamage = new List<DealDamageAction>();
            int valueOfX = 0;

            // Draw 2 cards.
            coroutine = base.DrawCards(DecisionMaker, 2);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            // Reduce the next damage dealt by {Gargoyle} by X, where X is up to 3.
            functionChoices = new Function[]
            {
                new Function(DecisionMaker, "Do not reduce damage", SelectionType.None,()=> DoNothing()),
                new Function(DecisionMaker, "Reduce the next damage dealt by Gargoyle by 1", SelectionType.None,()=>ReduceDamage(1)),
                new Function(DecisionMaker, "Reduce the next damage dealt by Gargoyle by 2", SelectionType.None,()=>ReduceDamage(2)),
                new Function(DecisionMaker, "Reduce the next damage dealt by Gargoyle by 3", SelectionType.None,()=>ReduceDamage(3))
            };

            selectFunction = new SelectFunctionDecision(base.GameController, base.HeroTurnTakerController, functionChoices, false);
            coroutine = base.GameController.SelectAndPerformFunction(selectFunction, storedResults);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (storedResults != null)
            {
                valueOfX = storedResults.FirstOrDefault().Index.Value;
            }

            // {Gargoyle} deals 1 target 1 toxic damage. 
            coroutine = base.GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), 1, DamageType.Toxic, 1, false, 1, storedResultsDecisions: storedCardResults, storedResultsDamage: storedResultsDamage, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (valueOfX > 0 && storedCardResults != null && storedCardResults.Any() && storedResultsDamage != null && storedResultsDamage.Any((dda) => dda.DidDealDamage))
            {
                var selectedCard = storedCardResults.FirstOrDefault().SelectedCard;

                // If that target takes damage this way, 
                if (storedResultsDamage.FirstOrDefault().Target == selectedCard)
                {
                    // {Gargoyle} deals X other targets 2 toxic damage each.
                    coroutine = base.GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), 2, DamageType.Toxic, valueOfX, false, valueOfX, additionalCriteria: (card)=> card != selectedCard, storedResultsDecisions: storedCardResults, storedResultsDamage: storedResultsDamage, cardSource: base.GetCardSource());
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

        private IEnumerator ReduceDamage(int amount)
        {
            ReduceDamageStatusEffect reduceDamageStatusEffect = new ReduceDamageStatusEffect(amount);

            reduceDamageStatusEffect.SourceCriteria.IsSpecificCard = base.CharacterCard;
            reduceDamageStatusEffect.NumberOfUses = 1;

            return base.AddStatusEffect(reduceDamageStatusEffect);
        }
    }

    ////Increase the next damage dealt by {Baccarat} by 1, or {Baccarat} deals 1 target 2 toxic damage.
    //IEnumerable<Function> functionChoices = new Function[]
    //{
				////Increase the next damage dealt by {Baccarat} by 1...
				//new Function(base.HeroTurnTakerController, "Increase the next damage dealt by Baccarat by 1", SelectionType.IncreaseNextDamage, () => base.AddStatusEffect(new IncreaseDamageStatusEffect(1){ SourceCriteria = { IsSpecificCard = base.CharacterCard }, NumberOfUses = new int?(1) })),

				////...or {Baccarat} deals 1 target 2 toxic damage.
				//new Function(base.HeroTurnTakerController, "Baccarat deals 1 target 2 toxic damage", SelectionType.TurnTaker, () => base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), 2, DamageType.Toxic, new int?(1), false, new int?(1),cardSource: base.GetCardSource()))
    //};
    //SelectFunctionDecision selectFunction = new SelectFunctionDecision(base.GameController, base.HeroTurnTakerController, functionChoices, false);
    //IEnumerator coroutine = base.GameController.SelectAndPerformFunction(selectFunction);

}
