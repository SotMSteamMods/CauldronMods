using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Terminus
{
    public abstract class TerminusMementoCardController : TerminusUtilityCardController
    {
        protected TerminusMementoCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
        }

        public override bool AskIfCardIsIndestructible(Card card)
        {
            return card == base.Card;
        }

        public override void AddTriggers()
        {
            // If another Memento would enter play, instead remove it from the game and ...
            base.AddTrigger<PlayCardAction>((pca) => pca.CardToPlay.DoKeywordsContain("memento"), PlayCardActionResponse, TriggerType.RemoveFromGame, TriggerTiming.Before);
            base.AddTriggers();
        }

        private IEnumerator PlayCardActionResponse(PlayCardAction playCardAction)
        {
            IEnumerator coroutine;

            coroutine = CancelAction(playCardAction);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = base.GameController.MoveCard(base.TurnTakerController, playCardAction.CardToPlay, base.TurnTaker.OutOfGame, toBottom: false, isPutIntoPlay: false, playCardIfMovingToPlayArea: true, null, showMessage: false, null, null, null, evenIfIndestructible: false, flipFaceDown: false, null, isDiscard: false, evenIfPretendGameOver: false, shuffledTrashIntoDeck: false, doesNotEnterPlay: false, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = OnOtherMementoRemoved();
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

        protected abstract IEnumerator OnOtherMementoRemoved();
    }
}
