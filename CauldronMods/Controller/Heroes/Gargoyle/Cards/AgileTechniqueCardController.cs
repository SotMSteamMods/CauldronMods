using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Gargoyle
{
    // Power
    // {Gargoyle} deals up to 2 targets 2 toxic damage each.
    // If a hero was damaged this way, {Gargoyle} deals a third target X+1 melee damage, where X is the damage dealt to that hero.
    public class AgileTechniqueCardController : GargoyleUtilityCardController
    {
        private int MaxTargets => GetPowerNumeral(0, 2);
        private int ToxicDamageAmount => GetPowerNumeral(1, 2);
        private int AddedMeleeDamageAmount => GetPowerNumeral(2, 1);

        public AgileTechniqueCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowSpecialString(TotalNextDamageBoostString);
        }

        public override IEnumerator UsePower(int index = 0)
        {
            List<DealDamageAction> storedResultsDamage = new List<DealDamageAction>();
            IEnumerator coroutine;

            // {Gargoyle} deals up to 2 targets 2 toxic damage each.
            coroutine = base.GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), ToxicDamageAmount, DamageType.Toxic, MaxTargets, false, 0, storedResultsDamage: storedResultsDamage, cardSource: base.GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            // If a hero was damaged this way, {Gargoyle} deals a third target 2 melee damage.
            if (storedResultsDamage != null && storedResultsDamage.Count((dda)=> IsHeroCharacterCard(dda.Target) == true && dda.Amount > 0) > 0)
            {
                var alreadySelected = storedResultsDamage.Select((DealDamageAction dd) => dd.Target);

                Func<int, IEnumerator> dealFinalDamage = delegate (int damageToDeal)
                {
                    return base.GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), damageToDeal, DamageType.Melee, 1, false, 1, additionalCriteria: (Card c) => !alreadySelected.Contains(c), cardSource: base.GetCardSource());
                };

                List<int> damageAmounts = new List<int>();
                var functions = new List<Function>();
                foreach(DealDamageAction dd in storedResultsDamage)
                {
                    if(dd.DidDealDamage && IsHeroCharacterCard(dd.Target))
                    {
                        int damage = dd.Amount + AddedMeleeDamageAmount;
                        if (!damageAmounts.Contains(damage))
                        {
                            damageAmounts.Add(damage);
                            functions.Add(new Function(DecisionMaker, $"Deal {damage} melee damage", SelectionType.DealDamage, () => dealFinalDamage(damage)));
                        }
                    }
                }
 
                coroutine = SelectAndPerformFunction(DecisionMaker, functions, false, noSelectableFunctionMessage: "No heroes were dealt damage.");

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
    }
}
