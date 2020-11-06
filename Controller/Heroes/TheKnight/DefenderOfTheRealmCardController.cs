using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.TheKnight
{
    public class DefenderOfTheRealmCardController : TheKnightCardController
    {
        public DefenderOfTheRealmCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"Search your deck for a copy of “Plate Mail” or “Plate Helm” and put it into play. Shuffle your deck.",
            //"Select a hero target. Until the start of your next turn, reduce damage dealt to that target by 1."
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
