using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.TheKnight
{
    public class PlateHelmCardController : RoninAssignableCardController
    {
        public PlateHelmCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddStartOfGameTriggers()
        {
            AddTrigger(
                (RemoveTargetAction r) => r.CardToRemoveTarget == Card,
                (RemoveTargetAction a) => AdjustTargetnessResponseNotPrivate(a, Card, 3),
                TriggerType.CancelAction,
                TriggerTiming.Before,
                outOfPlayTrigger: true
            );

            AddTrigger(
                (BulkRemoveTargetsAction r) => r.CardsToRemoveTargets.Any((Card c) => c == Card),
                (BulkRemoveTargetsAction a) => AdjustTargetnessResponseNotPrivate(a, Card, 3),
                TriggerType.CancelAction,
                TriggerTiming.Before,
                outOfPlayTrigger: true
            );
        }

        public override void AddTriggers()
        {
            base.AddRedirectDamageTrigger(dd => IsEquipmentEffectingCard(dd.Target), c => base.Card, true);

            base.AddTriggers();
        }

        public override IEnumerator Play()
        {
            var coroutine = base.DrawCard(this.HeroTurnTaker, optional: true);
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
