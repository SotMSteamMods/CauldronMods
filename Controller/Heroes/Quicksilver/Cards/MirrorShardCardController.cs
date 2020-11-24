using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;

namespace Cauldron.Quicksilver
{
    public class MirrorShardCardController : CardController
    {
        public MirrorShardCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        private bool redirectPrimed = false;

        public override IEnumerator Play()
        {
            //When this card enters play, draw a card.
            IEnumerator coroutine = base.DrawCard();
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
            //You may redirect any damage dealt by other hero targets to {Quicksilver}.
            base.AddTrigger<DealDamageAction>((DealDamageAction action) => action.DamageSource.IsHero && action.DamageSource.Card != base.CharacterCard && action.IsRedirectable, this.MaybeRedirectResponse, TriggerType.RedirectDamage, TriggerTiming.Before, isActionOptional: true);
            //Whenever {Quicksilver} takes damage this way, she deals 1 non-hero target X damage of the same type, where X is the damage that was dealt to {Quicksilver} plus 1.
            base.AddTrigger<DealDamageAction>((DealDamageAction action) => redirectPrimed, (DealDamageAction action) => this.DealDamageResponse(action), TriggerType.DealDamage, TriggerTiming.After);
        }

        private IEnumerator MaybeRedirectResponse(DealDamageAction action)
        {
            if (action.IsRedirectable == false)
            {
                yield break;
            }

            List<YesNoCardDecision> didRedirect = new List<YesNoCardDecision> { };
            IEnumerator askForRedirect = GameController.MakeYesNoCardDecision(HeroTurnTakerController, SelectionType.RedirectDamage, this.Card, action: action, storedResults: didRedirect, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(askForRedirect);
            }
            else
            {
                base.GameController.ExhaustCoroutine(askForRedirect);
            }

            if (DidPlayerAnswerYes(didRedirect))
            {
                IEnumerator coroutine = base.GameController.RedirectDamage(action, base.CharacterCard, false, base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                redirectPrimed = true;
                yield return null;
            }
            yield break;
        }

        private IEnumerator DealDamageResponse(DealDamageAction action)
        {
            //there is possibly a chance of false positive here, but it's what I can do for now            
            if (action.NumberOfTimesRedirected > 0 && action.AllTargets.Contains(this.CharacterCard))
            {
                redirectPrimed = false;
                if (action.Target == base.CharacterCard && action.DidDealDamage)
                {
                    IEnumerator coroutine = base.GameController.SelectTargetsAndDealDamage(base.HeroTurnTakerController, new DamageSource(base.GameController, base.CharacterCard), action.Amount + 1, action.DamageType, 1, false, 1, cardSource: base.GetCardSource());
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