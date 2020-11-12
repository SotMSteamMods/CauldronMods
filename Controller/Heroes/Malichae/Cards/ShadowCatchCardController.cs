using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Malichae
{
    public class ShadowCatchCardController : MalichaeCardController
    {
        public ShadowCatchCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
        public override IEnumerator Play()
        {
            IEnumerator e1 = base.GameController.SelectAndMoveCard(DecisionMaker, c => IsDjinn(c) && c.Location == DecisionMaker.TurnTaker.Deck, DecisionMaker.HeroTurnTaker.Hand, cardSource: GetCardSource());
            IEnumerator e2 = base.ShuffleDeck(DecisionMaker, DecisionMaker.HeroTurnTaker.Deck);
            IEnumerator e3 = base.DrawCard(HeroTurnTaker, true);
            IEnumerator e4 = base.SelectAndPlayCardFromHand(DecisionMaker);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(e1);
                yield return base.GameController.StartCoroutine(e2);
                yield return base.GameController.StartCoroutine(e3);
                yield return base.GameController.StartCoroutine(e4);
            }
            else
            {
                base.GameController.ExhaustCoroutine(e1);
                base.GameController.ExhaustCoroutine(e2);
                base.GameController.ExhaustCoroutine(e3);
                base.GameController.ExhaustCoroutine(e4);
            }
            yield break;
        }
    }
}
