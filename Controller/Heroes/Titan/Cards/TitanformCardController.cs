using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;

namespace Cauldron.Titan
{
    public class TitanformCardController : CardController
    {
        public TitanformCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            AllowFastCoroutinesDuringPretend = false;
        }

        public override void AddTriggers()
        {
            //Whenever {Titan} is dealt damage by another target, reduce damage dealt to {Titan} by 1 until the start of your next turn.
            base.AddTrigger<DealDamageAction>((DealDamageAction action) => action.Target == base.CharacterCard && !action.DamageSource.IsSameCard(this.CharacterCard) && action.DidDealDamage, this.DealtDamageResponse, TriggerType.CreateStatusEffect, TriggerTiming.After);

            //When {Titan} would deal damage, you may destroy this card to increase that damage by 2.
            base.AddTrigger<DealDamageAction>((DealDamageAction action) => action.DamageSource.Card == base.CharacterCard, this.DestroyThisCardToIncreaseDamageResponse, new TriggerType[] { TriggerType.DestroySelf, TriggerType.IncreaseDamage }, TriggerTiming.Before);
        }

        private IEnumerator DealtDamageResponse(DealDamageAction action)
        {
            var reduceDamageEffect = new ReduceDamageStatusEffect(1);
            reduceDamageEffect.UntilStartOfNextTurn(this.TurnTaker);
            reduceDamageEffect.TargetCriteria.IsSpecificCard = this.CharacterCard;
            reduceDamageEffect.CardSource = this.Card;

            IEnumerator coroutine;
            var maybeExistingEffect = GetExistingEffect(reduceDamageEffect);
            if(maybeExistingEffect != null)
            {
                maybeExistingEffect.CombineWithStatusEffect(reduceDamageEffect);
                coroutine = GameController.SendMessageAction($"{this.CharacterCard.Title}'s damage resistance from {this.Card.Title} increases to {maybeExistingEffect.Amount}.", Priority.Medium, GetCardSource());
            }
            else
            {
                coroutine = AddStatusEffect(reduceDamageEffect, true);
            }
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }

        private ReduceDamageStatusEffect GetExistingEffect(ReduceDamageStatusEffect newEffect)
        {
            var foundEffect = Game.StatusEffects.Where((StatusEffect se) => se is ReduceDamageStatusEffect && (se as ReduceDamageStatusEffect).IsSameAs(newEffect)).FirstOrDefault();
            Log.Debug($"Found Status Effect: {foundEffect}");
            return foundEffect != null ? foundEffect as ReduceDamageStatusEffect : null;
        }

        private IEnumerator DestroyThisCardToIncreaseDamageResponse(DealDamageAction action)
        {
            List<DestroyCardAction> destroyList = new List<DestroyCardAction>();
            //...you may destroy this card...
            IEnumerator coroutine = base.GameController.DestroyCard(base.HeroTurnTakerController, base.Card, true, storedResults: destroyList, cardSource: base.GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            if (base.DidDestroyCard(destroyList))
            {
                this.AddInhibitorException((GameAction ga) => ga is IncreaseDamageAction);
                //...to increase that damage by 2.
                coroutine = GameController.IncreaseDamage(action, 2, false, GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
                RemoveInhibitorException();
            }
            yield break;
        }
    }
}