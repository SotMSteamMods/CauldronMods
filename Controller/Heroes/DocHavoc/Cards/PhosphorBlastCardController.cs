using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.DocHavoc
{
    public class PhosphorBlastCardController : CardController
    {
        public static string Identifier = "PhosphorBlast";
        private const int DamageAmount = 1;

        public PhosphorBlastCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //==============================================================
            // Deals each target 1 radiant damage. Non-hero targets dealt damage this way may not regain HP until the start of your next turn.
            //==============================================================

            List<DealDamageAction> storedDamageResults = new List<DealDamageAction>();

            IEnumerator routine = base.GameController.DealDamage(this.HeroTurnTakerController, this.Card, (Func<Card, bool>) (c => c.IsTarget),
                DamageAmount, DamageType.Radiant, storedResults: storedDamageResults, cardSource: this.GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }


            // Check non hero targets to see if they took the intended damage
            foreach (DealDamageAction dda in storedDamageResults.Where(dda => !dda.Target.IsHero && dda.DidDealDamage))
            {
                // Apply CannotGainHPStatusEffect until start of this hero's next turn
                CannotGainHPStatusEffect cannotGainHpStatusEffect = new CannotGainHPStatusEffect
                {
                    TargetCriteria = {IsSpecificCard = dda.Target}
                };
                cannotGainHpStatusEffect.UntilStartOfNextTurn(this.TurnTaker);

                IEnumerator statusEffectRoutine = base.AddStatusEffect(cannotGainHpStatusEffect, true);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(statusEffectRoutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(statusEffectRoutine);
                }
            }
            
        }
    }
}
