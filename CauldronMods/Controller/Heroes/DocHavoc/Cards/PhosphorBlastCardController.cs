using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.DocHavoc
{
    public class PhosphorBlastCardController : CardController
    {
        //==============================================================
        // Deals each target 1 radiant damage. Non-hero targets dealt
        // damage this way may not regain HP until the start of your next turn.
        //==============================================================

        public static readonly string Identifier = "PhosphorBlast";
        private const int DamageAmount = 1;

        public PhosphorBlastCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //{DocHavoc} deals each target 1 radiant damage.
            List<DealDamageAction> storedDamageResults = new List<DealDamageAction>();

            IEnumerator routine = base.DealDamage(this.CharacterCard, c => c.IsTarget && c.IsInPlayAndHasGameText,
                DamageAmount, DamageType.Radiant, storedResults: storedDamageResults);

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(routine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(routine);
            }


            // Check non hero targets to see if they took the intended damage
            foreach (DealDamageAction dd in storedDamageResults.Where(dda => !IsHero(dda.Target) && dda.DidDealDamage))
            {
                // Apply CannotGainHPStatusEffect until start of this hero's next turn
                CannotGainHPStatusEffect cannotGainHpStatusEffect = new CannotGainHPStatusEffect
                {
                    TargetCriteria = { IsSpecificCard = dd.Target }
                };
                cannotGainHpStatusEffect.UntilStartOfNextTurn(this.TurnTaker);

                IEnumerator statusEffectRoutine = base.AddStatusEffect(cannotGainHpStatusEffect);
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
