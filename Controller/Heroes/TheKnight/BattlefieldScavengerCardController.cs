using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.TheKnight
{
    public class BattlefieldScavengerCardController : TheKnightCardController
    {
        public BattlefieldScavengerCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"Each player may draw a card, or take an Equipment or Ongoing card from their trash and put it on top of their deck.",
            //TODO - Not Implemented
            var coroutine = base.GameController.SendMessageAction("Not implemented", Priority.Medium, base.GetCardSource());
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
