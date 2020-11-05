using System;
using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Baccarat
{
    public class CheapTrickCardController : CardController
    {
        #region Constructors

        public CheapTrickCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController) { }

        #endregion Constructors

        #region Methods

        public override IEnumerator Play()
        {
            //Discard the top card of your deck.
            IEnumerator coroutine = base.GameController.DiscardTopCard(this.TurnTaker.Deck, null, null, this.TurnTaker, base.GetCardSource(null));
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //Reveal cards from the top of your deck until you reveal a trick. Shuffle the other cards back into your deck and put the trick into play.
            coroutine = base.RevealCards_MoveMatching_ReturnNonMatchingCards(base.TurnTakerController, base.TurnTaker.Deck, false, true, false, new LinqCardCriteria((Card c) => c.DoKeywordsContain("trick")), new int?(1), null, true, false, RevealedCardDisplay.ShowMatchingCards, true, false, null, false, false);
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

        #endregion Methods
    }
}