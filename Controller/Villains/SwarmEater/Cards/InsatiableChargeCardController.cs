using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.SwarmEater
{
    public class InsatiableChargeCardController : CardController
    {
        public InsatiableChargeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //{SwarmEater} deals each other target 2 melee damage...
            IEnumerator coroutine = base.DealDamage(base.CharacterCard, (Card c) => c != base.CharacterCard, 2, DamageType.Melee);
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
            //For each target destroyed by {SwarmEater} this way...
            base.AddTrigger<DestroyCardAction>((DestroyCardAction d) => d.CardToDestroy.Card.IsTarget && d.WasCardDestroyed && d.ActionSource is DealDamageAction && ((DealDamageAction)d.ActionSource).DamageSource.IsSameCard(base.CharacterCard), new Func<DestroyCardAction, IEnumerator>(this.DestroyTargetResponse), TriggerType.DestroyCard, TriggerTiming.After, ActionDescription.Unspecified, false, true, null, false, null, null, false, false);
        }

        private IEnumerator DestroyTargetResponse(DestroyCardAction action)
        {
            //...destroy 1 hero ongoing or equipment card.
            IEnumerator coroutine = base.GameController.SelectAndDestroyCard(this.DecisionMaker, new LinqCardCriteria((Card c) => c.IsHero && (c.IsOngoing || c.DoKeywordsContain("equipment"))), false);
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