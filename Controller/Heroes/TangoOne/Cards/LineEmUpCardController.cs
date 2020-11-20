using System;
using System.Collections;
using System.Collections.Generic;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TangoOne
{
    public class LineEmUpCardController : TangoOneBaseCardController
    {
        //==============================================================
        // Whenever {TangoOne} destroys a card,
        // she may deal 1 target 1 projectile damage.
        //==============================================================

        public static string Identifier = "LineEmUp";

        private const int DamageAmount = 1;

        public LineEmUpCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            base.AddTrigger<DestroyCardAction>(destroyCard => destroyCard.WasCardDestroyed
                && destroyCard.ResponsibleCard != null 
                && destroyCard.ResponsibleCard.Equals(this.Card.Owner.CharacterCard)
                && base.GameController.IsCardVisibleToCardSource(destroyCard.CardToDestroy.Card,
                    base.GetCardSource()),
                this.DealDamageResponse,
                TriggerType.DealDamage, TriggerTiming.After);

            base.AddTriggers();
        }

        private IEnumerator DealDamageResponse(DestroyCardAction destroyCard)
        {
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

            // You may deal 1 target 1 projectile damage
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