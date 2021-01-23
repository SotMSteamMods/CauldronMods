using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

//DECKLIST EDIT: third damage now "damage dealt to that hero + 1"
namespace Cauldron.Gargoyle
{
    // Power
    // {Gargoyle} deals up to 2 targets 2 toxic damage each.
    // If a hero was damaged this way, {Gargoyle} deals a third target 2 melee damage.
    public class AgileTechniqueCardController : GargoyleUtilityCardController
    {
        private int MaxTargets => GetPowerNumeral(0, 2);
        private int ToxicDamageAmount => GetPowerNumeral(1, 2);
        private int MeleeDamageAmount => GetPowerNumeral(2, 2);

        public AgileTechniqueCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UsePower(int index = 0)
        {
            List<DealDamageAction> storedResultsDamage = new List<DealDamageAction>();
            List<string> selectedIdentifiers;
            IEnumerator coroutine;

            // {Gargoyle} deals up to 2 targets 2 toxic damage each.
            coroutine = base.GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), ToxicDamageAmount, DamageType.Toxic, MaxTargets, false, null, storedResultsDamage: storedResultsDamage, cardSource: base.GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            // If a hero was damaged this way, {Gargoyle} deals a third target 2 melee damage.
            if (storedResultsDamage != null && storedResultsDamage.Count((dda)=>dda.Target.IsHero == true && dda.Amount > 0) > 0)
            {
                selectedIdentifiers = storedResultsDamage.Select(item => item.Target.Identifier).ToList();

                coroutine = base.GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), MeleeDamageAmount, DamageType.Melee, 1, false, 1, additionalCriteria: (Card c) => !selectedIdentifiers.Contains(c.Identifier), cardSource: base.GetCardSource());

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
