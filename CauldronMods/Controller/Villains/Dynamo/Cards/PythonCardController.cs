using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Dynamo
{
    public class PythonCardController : DynamoUtilityCardController
    {
        public PythonCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHasBeenUsedThisTurn(OncePerTurn, trueFormat: $"{Card.Title} has been dealt damage by a hero target this turn.", falseFormat: $"{Card.Title} has not been dealt damage by a hero target this turn.").Condition = () => Card.IsInPlayAndHasGameText; ;
            SpecialStringMaker.ShowHeroTargetWithLowestHP(numberOfTargets: 2);
        }

        protected const string OncePerTurn = "OncePerTurn";

        public override void AddTriggers()
        {
            //The first time a hero target deals damage to this card each turn, reduce damage dealt by that target by 1 until the start of the next villain turn.
            base.AddTrigger<DealDamageAction>((DealDamageAction action) => !base.HasBeenSetToTrueThisTurn(OncePerTurn) && action.Target == this.Card && action.DamageSource.IsHero && action.DamageSource.IsTarget && action.Amount > 0, this.ReduceDamageResponse, TriggerType.ReduceDamage, TriggerTiming.After);

            //Whenever a One-shot enters the villain trash, this card deals the 2 hero targets with the lowest HP {H - 2} toxic damage each.
            base.AddTrigger<MoveCardAction>((MoveCardAction action) => action.Destination.IsTrash && action.Destination.IsVillain && action.CardToMove.IsOneShot, this.DealDamageResponse, TriggerType.DealDamage, TriggerTiming.After);
        }

        private IEnumerator ReduceDamageResponse(DealDamageAction action)
        {
            SetCardPropertyToTrueIfRealAction(OncePerTurn);

            //...reduce damage dealt by that target by 1 until the start of the next villain turn.
            ReduceDamageStatusEffect statusEffect = new ReduceDamageStatusEffect(1);
            statusEffect.SourceCriteria.IsSpecificCard = action.DamageSource.Card;
            statusEffect.UntilStartOfNextTurn(base.TurnTaker);
            statusEffect.UntilCardLeavesPlay(action.DamageSource.Card);

            IEnumerator coroutine = base.AddStatusEffect(statusEffect);
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

        private IEnumerator DealDamageResponse(MoveCardAction action)
        {
            //...this card deals the 2 hero targets with the lowest HP {H - 2} toxic damage each.
            IEnumerator coroutine = base.DealDamageToLowestHP(this.Card, 1, (Card c) => IsHeroTarget(c), (Card c) => base.Game.H - 2, DamageType.Toxic, numberOfTargets: 2);
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
