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

        public static readonly string Identifier = "LineEmUp";

        private const int DamageAmount = 1;

        public LineEmUpCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        private bool LineEmUpTriggerCondition(DestroyCardAction destroyCard)
        {
            return destroyCard.WasCardDestroyed &&
                    base.GameController.IsCardVisibleToCardSource(destroyCard.CardToDestroy.Card, base.GetCardSource()) &&
                    (
                        (destroyCard.ResponsibleCard != null && destroyCard.ResponsibleCard.Owner == this.TurnTaker) ||
                        (destroyCard.CardSource != null && destroyCard.CardSource.Card != null && destroyCard.CardSource.Card.Owner == this.TurnTaker)
                    );
        }

        public override void AddTriggers()
        {
            base.AddTrigger<DestroyCardAction>(LineEmUpTriggerCondition, this.DealDamageResponse, TriggerType.DealDamage, TriggerTiming.After);
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