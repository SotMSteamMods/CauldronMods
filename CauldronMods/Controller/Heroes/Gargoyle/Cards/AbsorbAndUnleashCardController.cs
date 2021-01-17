using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Gargoyle
{
    
    //"{Gargoyle} deals 1 hero target 2 toxic damage.",
    //"{Gargoyle} deals up to X targets 3 toxic damage each, where X is the amount of damage that was dealt to that hero target."
    public class AbsorbAndUnleashCardController : GargoyleUtilityCardController
    {
        private const int HERO_DAMAGE = 2;
        private const int TARGET_DAMAGE = 3;

        public AbsorbAndUnleashCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;
            List<DealDamageAction> storedResults = new List<DealDamageAction>();
            int totalTargets = 0;

            // {Gargoyle} deals 1 hero target 2 toxic damage
            coroutine = base.GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, this.CharacterCard), HERO_DAMAGE, DamageType.Toxic, 1, false, 1, additionalCriteria: (card) => card.IsHero && card.IsTarget, storedResultsDamage: storedResults, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (DidDealDamage(storedResults))
            {
                totalTargets = storedResults.FirstOrDefault().Amount;
                // {Gargoyle} deals up to X targets 3 toxic damage each, where X is the amount of damage that was dealt to that hero target.
                coroutine = base.GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, this.CharacterCard), TARGET_DAMAGE, DamageType.Toxic, totalTargets, true, null, cardSource: GetCardSource());
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
