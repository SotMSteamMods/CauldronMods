using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;

namespace Cauldron.Quicksilver
{
    public class MirrorShardCardController : QuicksilverBaseCardController
    {
        public MirrorShardCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

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
            base.AddTrigger<DealDamageAction>((DealDamageAction action) => action.DamageSource != null && action.DamageSource.Card != null && action.DamageSource.IsHeroTarget && action.DamageSource.Card != base.CharacterCard && action.Target != base.CharacterCard &&  action.IsRedirectable, this.MaybeRedirectResponse, TriggerType.RedirectDamage, TriggerTiming.Before, isActionOptional: true);
            //Whenever {Quicksilver} takes damage this way, she deals 1 non-hero target X damage of the same type, where X is the damage that was dealt to {Quicksilver} plus 1.
            base.AddTrigger<DealDamageAction>((DealDamageAction action) => action.DidDealDamage && action.Target == this.CharacterCard && action.DamageModifiers.Any((ModifyDealDamageAction mdda) => mdda.CardSource != null && mdda.CardSource.Card == this.Card),
                                                (DealDamageAction action) => this.DealDamageResponse(action), 
                                                TriggerType.DealDamage, 
                                                TriggerTiming.After);
        }

        private IEnumerator MaybeRedirectResponse(DealDamageAction action)
        {
            if (action.IsRedirectable == false)
            {
                yield break;
            }

            List<YesNoCardDecision> didRedirect = new List<YesNoCardDecision> { };
            IEnumerator askForRedirect = GameController.MakeYesNoCardDecision(DecisionMaker, SelectionType.RedirectDamage, this.CharacterCard, action: action, storedResults: didRedirect, cardSource: GetCardSource());
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
                yield return null;
            }
            yield break;
        }

        private IEnumerator DealDamageResponse(DealDamageAction action)
        {
            if (action.Target == base.CharacterCard && action.DidDealDamage)
            {
                IEnumerator coroutine = base.GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), action.Amount + 1, action.DamageType, 1, false, 1,additionalCriteria: (Card c) => !IsHeroTarget(c), cardSource: base.GetCardSource());
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
}
