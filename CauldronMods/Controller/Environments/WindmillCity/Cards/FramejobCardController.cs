using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.WindmillCity
{
    public class FrameJobCardController : WindmillCityUtilityCardController
    {
        public override bool AllowFastCoroutinesDuringPretend => false;
        public FrameJobCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowListOfCards(new LinqCardCriteria(c => IsResponder(c), "responder")).Condition = () => !Card.IsInPlayAndHasGameText;
            SpecialStringMaker.ShowHeroTargetWithHighestHP();
        }

        public override IEnumerator Play()
        {
            //When this card enters play, reveal cards from the top of the environment deck until a Responder is revealed, put it into play, and discard the other revealed cards.
            IEnumerator coroutine = RevealCards_PutSomeIntoPlay_DiscardRemaining(TurnTakerController, TurnTaker.Deck, null, new LinqCardCriteria(c => IsResponder(c), "responder"), revealUntilNumberOfMatchingCards: 1);
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

        public override void AddTriggers()
        {
            //Redirect all damage dealt by Responders to the hero target with the highest HP.
            AddTrigger((DealDamageAction dd) => dd.DamageSource != null && dd.DamageSource.Card != null && IsResponder(dd.DamageSource.Card), RedirectResponderResponse, TriggerType.RedirectDamage, TriggerTiming.Before);
            //At the start of the environment turn, destroy this card.
            AddStartOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, DestroyThisCardResponse, TriggerType.DestroySelf);
        }

        private IEnumerator RedirectResponderResponse(DealDamageAction dd)
        {
            List<Card> storedResults = new List<Card>();
            IEnumerator coroutine = GameController.FindTargetWithHighestHitPoints(1, (Card c) => IsHero(c), storedResults, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            Card card = storedResults.FirstOrDefault();
            if (card != null)
            {
                coroutine = GameController.RedirectDamage(dd, card, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
        }
    }
}
