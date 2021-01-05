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

        public override void AddTriggers()
        {
            base.AddRedirectDamageTrigger(dd => IsEquipmentEffectingCard(dd.Target), c => base.Card, true);

            base.AddMaintainTargetTriggers((Card c) => c.Owner == base.Card.Owner && c.Identifier == Card.Identifier, 3, new List<string> { "equipment" });

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
