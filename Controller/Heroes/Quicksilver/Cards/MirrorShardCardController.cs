using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron
{
    public class MirrorShardCardController : CardController
    {
        public MirrorShardCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            this.RedirectedDamage = 0;
        }

        private int RedirectedDamage;
        private DamageType RedirectedType;

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
            base.AddTrigger<DealDamageAction>((DealDamageAction action) => action.DamageSource.IsHero && action.DamageSource.Card != base.CharacterCard && action.IsRedirectable, this.MaybeRedirectResponse, TriggerType.RedirectDamage, TriggerTiming.Before);
            //Whenever {Quicksilver} takes damage this way, she deals 1 non-hero target X damage of the same type, where X is the damage that was dealt to {Quicksilver} plus 1.
            base.AddTrigger<DealDamageAction>((DealDamageAction action) => action.Target == base.CharacterCard && this.RedirectedDamage > 0, (DealDamageAction action) => this.DealDamageResponse(action), TriggerType.DealDamage, TriggerTiming.After);
        }

        private IEnumerator MaybeRedirectResponse(DealDamageAction action)
        {
            IEnumerator coroutine = base.GameController.RedirectDamage(action, base.CharacterCard, true, base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if (action.DidDealDamage && action.Target == base.CharacterCard)
            {
                this.RedirectedDamage = action.Amount + 1;
                this.RedirectedType = action.DamageType;
            }
            yield break;
        }

        private IEnumerator DealDamageResponse(DealDamageAction action)
        {
            IEnumerator coroutine = base.GameController.SelectTargetsAndDealDamage(base.HeroTurnTakerController, new DamageSource(base.GameController, base.CharacterCard), this.RedirectedDamage, this.RedirectedType, 1, false, 1, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            this.RedirectedDamage = 0;
            yield break;
        }
    }
}