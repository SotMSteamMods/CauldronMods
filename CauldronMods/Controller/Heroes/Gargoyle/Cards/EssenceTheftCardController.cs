using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Gargoyle
{  
    //"{Gargoyle} may deal 3 targets 1 toxic damage each or 1 target 3 melee damage.",
    //"If any targets were dealt damage this way, {Gargoyle} regains 1HP."
    public class EssenceTheftCardController : GargoyleUtilityCardController
    {
        public EssenceTheftCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowSpecialString(TotalNextDamageBoostString);
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;
            Function[] functions;
            List<DealDamageAction> dealDamageActions = new List<DealDamageAction>();

            //"{Gargoyle} may deal 3 targets 1 toxic damage each or 1 target 3 melee damage.",
            functions = new Function[]
            {
                new Function(DecisionMaker, "Deal 3 targets 1 toxic damage", SelectionType.DealDamage, () => DealTargetsDamage(3, DamageType.Toxic, 1, dealDamageActions)),
                new Function(DecisionMaker, "Deal 1 target 3 melee damage", SelectionType.DealDamage, () => DealTargetsDamage(1, DamageType.Melee, 3, dealDamageActions))
            };
            coroutine = SelectAndPerformFunction(DecisionMaker, functions, optional: true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (dealDamageActions != null && dealDamageActions.Count(dda=>dda.Amount > 0) > 0)
            {
                //"If any targets were dealt damage this way, {Gargoyle} regains 1HP."
                coroutine = base.GameController.GainHP(this.CharacterCard, 1, cardSource: GetCardSource());
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

        private IEnumerator DealTargetsDamage(int totalTargets, DamageType damageType, int damageAmount, List<DealDamageAction> dealDamageActions)
        {
            IEnumerator coroutine;

            coroutine = base.GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), damageAmount, damageType, totalTargets, false, totalTargets, storedResultsDamage: dealDamageActions, cardSource: GetCardSource());
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
