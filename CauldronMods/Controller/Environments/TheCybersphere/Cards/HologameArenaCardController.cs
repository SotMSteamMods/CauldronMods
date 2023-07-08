using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.TheCybersphere
{
    public class HologameArenaCardController : TheCybersphereCardController
    {

        public HologameArenaCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //At the start of the Environment turn, you may destroy this card.
            AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, YouMayDestroyThisCardResponse, TriggerType.DestroySelf);

            //Whenever a hero card destroys an environment target, that hero draws a card.
            AddTrigger<DestroyCardAction>((DestroyCardAction dca) => dca.CardToDestroy != null && dca.CardToDestroy.Card.IsEnvironmentTarget && dca.WasCardDestroyed && GameController.IsCardVisibleToCardSource(dca.CardToDestroy.Card, GetCardSource()) &&
                dca.CardSource != null && IsHero(dca.CardSource.Card) && GameController.IsCardVisibleToCardSource(dca.CardSource.Card, GetCardSource()),
                DrawCardResponse, TriggerType.DrawCard, TriggerTiming.After);

            //At the end of the Environment turn, play the top card of the Environment deck.
            AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, PlayTheTopCardOfTheEnvironmentDeckWithMessageResponse, TriggerType.PlayCard);
        }

        private IEnumerator DrawCardResponse(DestroyCardAction dca)
        {
            //that hero draws a card
            HeroTurnTaker htt = dca.CardSource.Card.Owner.ToHero();
            IEnumerator coroutine = base.DrawCard(htt);
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