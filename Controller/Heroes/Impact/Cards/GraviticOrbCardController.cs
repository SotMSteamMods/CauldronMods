using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Impact
{
    public class GraviticOrbCardController : CardController
    {
        public GraviticOrbCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            //Play this card next to a target.
            IEnumerator coroutine = SelectCardThisCardWillMoveNextTo(new LinqCardCriteria((Card c) => c.IsTarget && c.IsInPlay, "targets", useCardsSuffix: false), storedResults, isPutIntoPlay, decisionSources);
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
        public override IEnumerator Play()
        {
            //"Play this card next to a target. {Impact} deals that target 2 infernal damage.",
            Card target = GetCardThisCardIsNextTo();
            if (target != null)
            {
                IEnumerator coroutine = DealDamage(this.CharacterCard, target, 2, DamageType.Infernal, cardSource: GetCardSource());
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

        public override void AddTriggers()
        {
            //"When the target next to this card would deal damage, destroy this card and prevent that damage."
            AddTrigger((DealDamageAction dd) => dd.DamageSource.IsSameCard(GetCardThisCardIsNextTo()) && dd.Amount > 0 && !this.IsBeingDestroyed, PreventDamageResponse, TriggerType.WouldBeDealtDamage, TriggerTiming.Before, isActionOptional: false);

            //stays in play if target dies
            AddIfTheCardThatThisCardIsNextToLeavesPlayMoveItToTheirPlayAreaTrigger(false);
        }

        private IEnumerator PreventDamageResponse(DealDamageAction dd)
        {
            IEnumerator coroutine = CancelAction(dd, isPreventEffect: true);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            coroutine = DestroyThisCardResponse(dd);
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
    }
}