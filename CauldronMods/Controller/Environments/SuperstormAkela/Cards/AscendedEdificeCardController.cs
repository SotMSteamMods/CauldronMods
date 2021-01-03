using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.SuperstormAkela
{
    public class AscendedEdificeCardController : SuperstormAkelaCardController
    {

        public AscendedEdificeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //When this card enters play, play the top card of the environment deck.",
            Log.Debug(Card.Title + " plays the top card of the environment deck...");
            IEnumerator coroutine = GameController.SendMessageAction(Card.Title + " plays the top card of the environment deck...", Priority.High, GetCardSource(), showCardSource: true);
            IEnumerator coroutine2 = GameController.PlayTopCard(DecisionMaker, base.TurnTakerController, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
                yield return GameController.StartCoroutine(coroutine2);

            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
                GameController.ExhaustCoroutine(coroutine2);
            }
        }

        public override void AddTriggers()
        {
            //Reduce damage dealt to villain cards by 1.
            AddReduceDamageTrigger((Card c) => IsVillain(c), 1);
        }

    }
}