using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Cricket
{
    public class InfrasonicCollapseCardController : CardController
    {
        public InfrasonicCollapseCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            List<DestroyCardAction> storedResults = new List<DestroyCardAction>();
            //Destroy 1 ongoing or environment card.
            IEnumerator coroutine = base.GameController.SelectAndDestroyCard(base.HeroTurnTakerController, new LinqCardCriteria((Card c) => IsOngoing(c) || c.IsEnvironment), false, storedResults, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //based on the card type do something
            if (DidDestroyCard(storedResults))
            {
                Card destroyedCard = GetDestroyedCards(storedResults).FirstOrDefault();
                if(IsOngoing(destroyedCard))
                {
                    //If you destroyed an ongoing card this way, {Cricket} deals 1 target 2 sonic damage.
                    coroutine = base.GameController.SelectTargetsAndDealDamage(base.HeroTurnTakerController, new DamageSource(base.GameController, base.CharacterCard), 2, DamageType.Sonic, 1, false, 1, cardSource: base.GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                }
                if (destroyedCard.IsEnvironment)
                {
                    //If you destroyed an environment card this way, {Cricket} deals each non-hero target 1 sonic damage.
                    coroutine = base.DealDamage(base.CharacterCard, (Card c) => !IsHeroTarget(c), 1, DamageType.Sonic);
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
            yield break;
        }
    }
}