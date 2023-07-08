using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Echelon
{
    public class RemoteObservationCardController : TacticBaseCardController
    {
        //==============================================================
        //"At the start of your turn, you may discard a card. If you do not, draw a card and destroy this card.",
        //"During their draw Phase, heroes draw 1 additional card, then discard 1 card."
        //==============================================================

        public static string Identifier = "RemoteObservation";

        public RemoteObservationCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            AddThisCardControllerToList(CardControllerListType.IncreasePhaseActionCount);
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine = IncreasePhaseActionCountIfInPhase((TurnTaker tt) => IsHero(tt), Phase.DrawCard, 1);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        protected override void AddTacticEffectTrigger()
        {
            //"During their draw Phase, heroes draw 1 additional card, then discard 1 card."
            AddAdditionalPhaseActionTrigger((TurnTaker tt) => IsHero(tt), Phase.DrawCard, 1);
            AddPhaseChangeTrigger(tt => IsHero(tt), phase => true, pca => pca.FromPhase.IsDrawCard, ThenDiscard, new TriggerType[] { TriggerType.DiscardCard }, TriggerTiming.Before);
        }

        private IEnumerator ThenDiscard(PhaseChangeAction pca)
        {
            var hero = GameController.ActiveTurnTaker;
            var player = FindHeroTurnTakerController(hero?.ToHero());
            if (player != null)
            {
                IEnumerator coroutine = SelectAndDiscardCards(player, 1, false, 1, responsibleTurnTaker: TurnTaker);
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
        public override bool AskIfIncreasingCurrentPhaseActionCount()
        {
            return GameController.ActiveTurnPhase.IsDrawCard;
        }
    }
}