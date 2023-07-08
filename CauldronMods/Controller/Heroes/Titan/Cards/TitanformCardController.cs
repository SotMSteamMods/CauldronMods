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
            base.AddTrigger<DealDamageAction>((DealDamageAction action) => action.DamageSource != null && action.DamageSource.IsTarget && action.Target == base.CharacterCard && !action.DamageSource.IsSameCard(this.CharacterCard) && action.DidDealDamage, this.DealtDamageResponse, TriggerType.CreateStatusEffect, TriggerTiming.After);

            //When {Titan} would deal damage, you may destroy this card to increase that damage by 2.
            base.AddTrigger<DealDamageAction>((DealDamageAction action) => action.DamageSource != null && action.DamageSource.Card != null && action.DamageSource.Card == base.CharacterCard && !this.Card.IsBeingDestroyed && !GameController.IsCardIndestructible(this.Card), this.DestroyThisCardToIncreaseDamageResponse, new TriggerType[] { TriggerType.DestroySelf, TriggerType.IncreaseDamage }, TriggerTiming.Before, isActionOptional: true);
        }

        private IEnumerator DealtDamageResponse(DealDamageAction action)
        {
            var reduceDamageEffect = new ReduceDamageStatusEffect(1);
            reduceDamageEffect.UntilStartOfNextTurn(this.TurnTaker);
            reduceDamageEffect.TargetCriteria.IsSpecificCard = this.CharacterCard;
            reduceDamageEffect.CardSource = this.Card;

            IEnumerator coroutine;
            var maybeExistingEffect = GetExistingEffect(reduceDamageEffect);
            if (maybeExistingEffect != null)
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
            var foundEffect = Game.StatusEffects.OfType<ReduceDamageStatusEffect>().FirstOrDefault(rdse => rdse.IsSameAs(newEffect));
            Log.Debug($"Found Status Effect: {foundEffect}");
            return foundEffect;
        }

        private IEnumerator DestroyThisCardToIncreaseDamageResponse(DealDamageAction action)
        {
            //NB: GameController.MakeYesNoCardDecision seems to give a different UI in game, this gives the UI we want.
            var yesNo = new YesNoCardDecision(GameController, DecisionMaker, SelectionType.DestroyCard, Card, action, null, GetCardSource());
            var coroutine = GameController.MakeDecisionAction(yesNo);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            if (DidPlayerAnswerYes(yesNo))
            {
                //...destroy this card to increase that damage by 2.
                damageToIncrease = action;
                AddWhenDestroyedTrigger(dc => IncreaseDamageResponse(action), TriggerType.IncreaseDamage);
                coroutine = base.GameController.DestroyCard(HeroTurnTakerController, Card, false, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }
        }

        private DealDamageAction damageToIncrease;

        private IEnumerator IncreaseDamageResponse(DealDamageAction action)
        {
            if (IsRealAction() && damageToIncrease == action)
            {
                //for some reason if this doesn't exist Uh Yeah I'm That Guy will double-apply 
                //the copied trigger
                damageToIncrease = null;
                return GameController.IncreaseDamage(action, 2, false, GetCardSource());
            }
            return DoNothing();
        }
    }
}