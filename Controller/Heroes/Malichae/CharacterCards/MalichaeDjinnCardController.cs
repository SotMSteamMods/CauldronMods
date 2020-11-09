using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;
using NUnit.Framework;

namespace Cauldron.Malichae
{
    public abstract class MalichaeDjinnCardController : MalichaeCardController
    {
        private readonly LinqCardCriteria _djinnOngoingCriteria;

        protected MalichaeDjinnCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            _djinnOngoingCriteria = new LinqCardCriteria(c => c.IsInPlay && c.IsOngoing && IsDjinn(c) && IsThisCardNextToCard(c), "djinn ongoings");

            SpecialStringMaker.ShowNumberOfCards(_djinnOngoingCriteria);
        }

        protected void AddDjinnTargetTrigger()
        {
            base.AddTrigger<DealDamageAction>(dda => dda.Target == this.Card && dda.Amount >= this.Card.HitPoints.Value, DjinnDeadlyDamageResponse, TriggerType.DealDamage, TriggerTiming.Before,
                ignoreBattleZone: true);
        }

        public override bool CanBeDestroyed => false;

        public override IEnumerator DestroyAttempted(DestroyCardAction destroyCard)
        {
            var coroutine = DjinnDestroyInsteadReponse(destroyCard);
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

        private IEnumerator DjinnDeadlyDamageResponse(DealDamageAction dda)
        {
            var coroutine = base.CancelAction(dda);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            coroutine = DjinnDestroyInsteadReponse(dda);
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

        private IEnumerator DjinnDestroyInsteadReponse(GameAction action)
        {
            var storedResults = new List<DestroyCardAction>();
            var coroutine = base.GameController.SelectAndDestroyCard(this.DecisionMaker, _djinnOngoingCriteria, false, storedResults, action.CardSource.Card, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if (DidDestroyCard(storedResults))
            {
                coroutine = base.GameController.SetHP(this.Card, this.Card.MaximumHitPoints.Value, GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            else
            {
                coroutine = base.GameController.MoveCard(this.DecisionMaker, this.Card, this.HeroTurnTaker.Hand,
                                cardSource: GetCardSource());
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
