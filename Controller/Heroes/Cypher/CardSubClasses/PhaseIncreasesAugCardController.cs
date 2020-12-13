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
            IEnumerator coroutine = IncreasePhaseActionCountIfInPhase((TurnTaker tt) => tt == base.TurnTaker, _phaseToIncrease, 1);
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

            base.AddTriggers();
        }

        private bool ShouldIncreasePhaseActionCount(TurnTaker tt)
        {
            return tt == base.GetCardThisCardIsNextTo(true).Owner;
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