using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace Cauldron.Necro
{
    public class ChaoticSummonCardController : NecroCardController
    {
        public ChaoticSummonCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }
        public override IEnumerator Play()
        {
            //Put the top 2 cards of your deck into play.
            IEnumerator coroutine = base.GameController.PlayTopCard(this.DecisionMaker, base.TurnTakerController,
                optional: false,
                numberOfCards: 2,
                upTo: false,
                isPutIntoPlay: true,
                cardSource: base.GetCardSource());
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
