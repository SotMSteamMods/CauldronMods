using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.Quicksilver
{
    public class LiquidMetalCardController : ComboCardController
    {
        public LiquidMetalCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //Reveal cards from the top of your deck until you reveal a Combo...
            IEnumerator coroutine = base.RevealCards_MoveMatching_ReturnNonMatchingCards(base.TurnTakerController, base.TurnTaker.Deck, false, false, true, new LinqCardCriteria((Card c) => c.DoKeywordsContain("combo")), 1, shuffleSourceAfterwards: false);
            //...and a Finisher and put them into your hand. Shuffle the other revealed cards back into your deck.
            IEnumerator coroutine2 = base.RevealCards_MoveMatching_ReturnNonMatchingCards(base.TurnTakerController, base.TurnTaker.Deck, false, false, true, new LinqCardCriteria((Card c) => c.DoKeywordsContain("finisher")), 1);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
                yield return base.GameController.StartCoroutine(coroutine2);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
                base.GameController.ExhaustCoroutine(coroutine2);
            }
            //{Quicksilver} may deal herself 2 melee damage and play a Combo now.
            base.Play();
            yield break;
        }
    }
}