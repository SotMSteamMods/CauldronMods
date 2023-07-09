using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Gyrosaur
{
    public class HyperspinCardController : GyrosaurUtilityCardController
    {
        public HyperspinCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsAtLocation(TurnTaker.Deck, new LinqCardCriteria((Card c) => IsCrash(c), "crash"));
        }

        public override IEnumerator Play()
        {
            //"When this card enters play, you may play a card.",
            IEnumerator coroutine = SelectAndPlayCardFromHand(DecisionMaker);
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
            //"Increase damage dealt by {Gyrosaur} to non-hero targets by 1.",
            AddIncreaseDamageTrigger((DealDamageAction dd) => dd.DamageSource.IsSameCard(CharacterCard) && !IsHero(dd.Target), 1);
            //"If you would draw a Crash card, play it instead. Then, destroy all copies of Hyperspin."
            AddTrigger((DrawCardAction dc) => dc.HeroTurnTaker == HeroTurnTaker, PlayCrashAndEndHyperspin, new TriggerType[] { TriggerType.PlayCard, TriggerType.DestroyCard }, TriggerTiming.Before);
        }

        private IEnumerator PlayCrashAndEndHyperspin(DrawCardAction dc)
        {
            if(!IsCrash(dc.CardToDraw))
            {
                //If we check for this in the trigger conditions,
                //smart autodraw can leak information about whether the card 
                //that's about to be drawn is a crash
                yield break;
            }
            IEnumerator coroutine = GameController.SendMessageAction($"{Card.Title} goes out of control and plays {dc.CardToDraw.Title}!", Priority.Medium, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            var playStorage = new List<bool>();
            coroutine = GameController.PlayCard(DecisionMaker, dc.CardToDraw, wasCardPlayed: playStorage, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (playStorage.FirstOrDefault() == true)
            {
                coroutine = CancelAction(dc, showOutput: false);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }

            //destroy all other Hyperspins first, so we can't get out of it by blowing this one up early
            coroutine = GameController.DestroyCards(DecisionMaker, new LinqCardCriteria((Card c) => c.IsInPlayAndHasGameText && c.Identifier == "Hyperspin" && c != Card), autoDecide: true, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = DestroyThisCardResponse(dc);
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
