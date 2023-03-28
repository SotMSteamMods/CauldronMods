using System;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TangoOne
{
    public class DisablingShotCardController : TangoOneBaseCardController
    {
        //==============================================================
        // You may destroy 1 ongoing card.
        // {TangoOne} may deal 1 target 2 projectile damage.
        //==============================================================

        public static readonly string Identifier = "DisablingShot";

        private const int DamageAmount = 2;

        public DisablingShotCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            // Destroy ongoing card option
            IEnumerator destroyOngoingRoutine = this.GameController.SelectAndDestroyCard(this.HeroTurnTakerController,
                new LinqCardCriteria(card => IsOngoing(card) && card.IsInPlay, "ongoing"),
                true, cardSource: this.GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(destroyOngoingRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(destroyOngoingRoutine);
            }

            List<SelectCardDecision> selectCardResults = new List<SelectCardDecision>();
            IEnumerator selectOwnCharacterRoutine = base.SelectOwnCharacterCard(selectCardResults, SelectionType.HeroToDealDamage);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(selectOwnCharacterRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(selectOwnCharacterRoutine);
            }

            Card characterCard = GetSelectedCard(selectCardResults);
            IEnumerator dealDamageRoutine = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker,
                new DamageSource(base.GameController, characterCard), DamageAmount,
                DamageType.Projectile, 1, false, 0,
                additionalCriteria: c => c.IsTarget && c.IsInPlayAndHasGameText,
                cardSource: base.GetCardSource());

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(dealDamageRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(dealDamageRoutine);
            }
        }
    }
}