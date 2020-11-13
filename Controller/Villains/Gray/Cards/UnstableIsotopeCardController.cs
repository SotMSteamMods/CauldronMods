using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron
{
    public class UnstableIsotopeCardController : CardController
    {
        public UnstableIsotopeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //Reveal cards from the top of the villain deck until 2 Radiation cards are revealed. Put those cards into play and discard the rest.
            IEnumerator coroutine = base.RevealCards_PutSomeIntoPlay_DiscardRemaining(this.DecisionMaker, base.TurnTaker.Deck, null, new LinqCardCriteria((Card c) => c.DoKeywordsContain("radiation")), revealUntilNumberOfMatchingCards: new int?(2));
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