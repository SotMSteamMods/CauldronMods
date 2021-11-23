using System;
using System.Collections;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Cypher
{
    public abstract class PhaseIncreasesAugCardController : AugBaseCardController
    {
        private readonly Phase _phaseToIncrease;

        protected PhaseIncreasesAugCardController(Card card, TurnTakerController turnTakerController, Phase phase) : base(card, turnTakerController)
        {
            _phaseToIncrease = phase;
            AddThisCardControllerToList(CardControllerListType.IncreasePhaseActionCount);
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine = IncreasePhaseActionCountIfInPhase((TurnTaker tt) => tt == base.GetCardThisCardIsNextTo(true).Owner, _phaseToIncrease, 1);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        public override void AddTriggers()
        {
            //They may play an additional card during their play phase.
            base.AddAdditionalPhaseActionTrigger(this.ShouldIncreasePhaseActionCount, _phaseToIncrease, 1);

            base.AddTrigger<MoveCardAction>(mca => mca.CardToMove == Card && mca.Origin.IsNextToCard && mca.Destination.IsNextToCard, MoveCardResponse, TriggerType.IncreasePhaseActionCount, TriggerTiming.After);

            base.AddTriggers();
        }

        private IEnumerator MoveCardResponse(MoveCardAction mca)
        {
            //target we are moving away from, reduce his phase count
            IEnumerator coroutine = ReducePhaseActionCountIfInPhase((TurnTaker tt) => mca.Origin.IsHero && tt == mca.Origin.OwnerTurnTaker, _phaseToIncrease, 1);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //target we are moving to, increase his phase count
            coroutine = IncreasePhaseActionCountIfInPhase((TurnTaker tt) => mca.Destination.IsHero && tt == mca.Destination.OwnerTurnTaker, _phaseToIncrease, 1);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private bool ShouldIncreasePhaseActionCount(TurnTaker tt)
        {
            return base.GetCardThisCardIsNextTo(true) != null && tt == base.GetCardThisCardIsNextTo(true).Owner;
        }

        public override bool AskIfIncreasingCurrentPhaseActionCount()
        {
            if (base.GameController.ActiveTurnPhase.Phase == _phaseToIncrease)
            {
                return ShouldIncreasePhaseActionCount(base.GameController.ActiveTurnTaker);
            }
            return false;
        }
    }
}