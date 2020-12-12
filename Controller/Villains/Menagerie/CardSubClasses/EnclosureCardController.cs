using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.Menagerie
{
    public class EnclosureCardController : CardController
    {
        public EnclosureCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public IEnumerator EncloseTopCardResponse()
        {
            //When this card enters play, place the top card of the villain deck beneath it face down.
            IEnumerator coroutine = base.GameController.MoveCard(base.TurnTakerController, base.TurnTaker.Deck.TopCard, base.Card.UnderLocation, flipFaceDown: true, cardSource: base.GetCardSource());
            coroutine = base.GameController.GainHP(base.Card, 6, cardSource: base.GetCardSource());
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