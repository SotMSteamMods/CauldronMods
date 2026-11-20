using System.Collections;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Drift;

public class ResourcefulDaydreamerCardController(Card card, TurnTakerController turnTakerController) : DriftBaseCardController(card, turnTakerController)
{
    public override IEnumerator Play()
    {
        //Draw 2 cards. 
        IEnumerator coroutine = base.DrawCards(base.HeroTurnTakerController, 2);
        if (base.UseUnityCoroutines)
        {
            yield return base.GameController.StartCoroutine(coroutine);
        }
        else
        {
            base.GameController.ExhaustCoroutine(coroutine);
        }

        //Discard a card.
        coroutine = base.GameController.SelectAndDiscardCard(base.HeroTurnTakerController, cardSource: base.GetCardSource());
        if (base.UseUnityCoroutines)
        {
            yield return base.GameController.StartCoroutine(coroutine);
        }
        else
        {
            base.GameController.ExhaustCoroutine(coroutine);
        }

        //{DriftPast} You may play a card.
        if (this.IsTimeMatchingPast)
        {
            coroutine = base.GameController.SelectAndPlayCardFromHand(base.HeroTurnTakerController, true, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        //{DriftFuture} You may use a power.
        if (this.IsTimeMatchingFuture)
        {
            coroutine = base.GameController.SelectAndUsePower(base.HeroTurnTakerController, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }
        yield break;
    }
}
