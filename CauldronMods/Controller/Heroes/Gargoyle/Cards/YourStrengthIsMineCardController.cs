using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Gargoyle
{
    public class YourStrengthIsMineCardController : GargoyleUtilityCardController
    {
        /*
         * Play this card next to a target. You may destroy this card at any time.
         * When this card is destroyed, reduce the next damage dealt by that target by 2 and increase the next damage dealt by {Gargoyle} by 2.
        */
        private LinqCardCriteria NextToCardCriteria { get; set; }
        private SelfDestructTrigger SelfDestructTrigger { get; set; }

        public YourStrengthIsMineCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            this.NextToCardCriteria = new LinqCardCriteria((Card c) => c.IsTarget && GameController.IsCardVisibleToCardSource(c, GetCardSource()), "", useCardsSuffix: false, singular: "target", plural: "targets");
            SelfDestructTrigger = null;
        }

        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            IEnumerator coroutine;

            //  Play this card next to a target.
            coroutine = SelectCardThisCardWillMoveNextTo(NextToCardCriteria, storedResults, isPutIntoPlay, decisionSources);
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

        public override bool AllowFastCoroutinesDuringPretend
        {
            get
            {
                // This allows us to make decisions during damage actions.
                return false;
            }
        }

        public override void AddTriggers()
        {
            // You may destroy this card at any time.
            base.AddTrigger<DealDamageAction>((dda) => base.IsThisCardNextToCard(dda.DamageSource.Card) || dda.DamageSource.Card == base.CharacterCard, DealDamageActionResponse, new TriggerType[] { TriggerType.ModifyDamageAmount }, TriggerTiming.Before, isConditional: true, isActionOptional:true);

            // When this card is destroyed, reduce the next damage dealt by that target by 2 and increase the next damage dealt by {Gargoyle} by 2.
            SelfDestructTrigger =  base.AddWhenDestroyedTrigger(CardDestroyedResponse, TriggerType.CreateStatusEffect);

            AddIfTheCardThatThisCardIsNextToLeavesPlayMoveItToTheirPlayAreaTrigger(alsoRemoveTriggersFromThisCard: false);
        }

        private IEnumerator DealDamageActionResponse(DealDamageAction dealDamageAction)
        {
            IEnumerator coroutine;
            List<YesNoCardDecision> storedResults = new List<YesNoCardDecision>();
            List<DestroyCardAction> destroyCardActions = new List<DestroyCardAction>();
            Card target;

            // You may destroy this card at any time
            coroutine = base.GameController.MakeYesNoCardDecision(DecisionMaker, SelectionType.DestroySelf, base.Card, action: dealDamageAction, storedResults: storedResults, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (base.DidPlayerAnswerYes(storedResults))
            {
                target = base.GetCardThisCardIsNextTo();

                if (dealDamageAction.DamageSource.Card == target || dealDamageAction.DamageSource.Card == base.CharacterCard)
                {
                    // We don't want the trigger firing and applying it's effect. We are handling that here because either Gargoyle,
                    // or the target of this card is about to do damage.
                    RemoveTrigger(SelfDestructTrigger);
                }

                coroutine = base.GameController.DestroyCard(DecisionMaker, this.Card, storedResults: destroyCardActions, postDestroyAction: () => DoPostDamageDestroy(dealDamageAction, target), cardSource: base.GetCardSource());
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

        private IEnumerator DoPostDamageDestroy(DealDamageAction dealDamageAction, Card target)
        {
            IEnumerator coroutine;
            ReduceDamageStatusEffect reduceDamageStatusEffect;

            // When this card is destroyed, reduce the next damage dealt by that target by 2
            if (target != null)
            {
                reduceDamageStatusEffect = new ReduceDamageStatusEffect(2);
                reduceDamageStatusEffect.UntilTargetLeavesPlay(target);
                reduceDamageStatusEffect.SourceCriteria.IsSpecificCard = target;
                reduceDamageStatusEffect.NumberOfUses = 1;

                coroutine = base.AddStatusEffect(reduceDamageStatusEffect);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }

            // and increase the next damage dealt by {Gargoyle} by 2.
            if (dealDamageAction.DamageSource.IsSameCard(CharacterCard))
            {
                coroutine = GameController.IncreaseDamage(dealDamageAction, 2, cardSource: GetCardSource());
            }
            else
            {
                coroutine = base.IncreaseGargoyleNextDamage(2);
            }
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

        private IEnumerator CardDestroyedResponse(DestroyCardAction destroyCardAction)
        {
            ReduceDamageStatusEffect reduceDamageStatusEffect;
            Card target;
            IEnumerator coroutine;

            if (base.DidDestroyCard(destroyCardAction))
            {
                target = base.GetCardThisCardIsNextTo();

                // When this card is destroyed, reduce the next damage dealt by that target by 2
                if (target != null && target.IsTarget)
                {
                    reduceDamageStatusEffect = new ReduceDamageStatusEffect(2);
                    reduceDamageStatusEffect.UntilTargetLeavesPlay(target);
                    reduceDamageStatusEffect.SourceCriteria.IsSpecificCard = target;
                    reduceDamageStatusEffect.NumberOfUses = 1;

                    coroutine = base.AddStatusEffect(reduceDamageStatusEffect);
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                }

                // and increase the next damage dealt by {Gargoyle} by 2.
                if (base.CharacterCard.IsActive)
                {
                    coroutine = base.IncreaseGargoyleNextDamage(2);
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
