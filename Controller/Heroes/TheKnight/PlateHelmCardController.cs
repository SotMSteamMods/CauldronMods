using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Markup;

namespace Cauldron.TheKnight
{
    public class PlateHelmCardController : TheKnightCardController
    {
        public PlateHelmCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //TODO - Promo Support
            base.AddRedirectDamageTrigger(dd => dd.Target == base.CharacterCard, c => base.Card, true);
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
