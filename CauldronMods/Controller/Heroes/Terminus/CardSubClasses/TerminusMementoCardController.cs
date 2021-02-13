using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Terminus
{
    public abstract class TerminusMementoCardController : TerminusBaseCardController
    {
        protected TerminusMementoCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            ShowIndestructibleString();
            base.AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
        }

        protected virtual void ShowIndestructibleString()
        {
            SpecialStringMaker.ShowSpecialString(() => "This card is indestructible.");
        }

        public override bool AskIfCardIsIndestructible(Card card)
        {
            return card == base.Card;
        }

        public override void AddTriggers()
        {
            // If another Memento would enter play, instead remove it from the game and ...
            base.AddTrigger<CardEntersPlayAction>((cep) => cep.CardEnteringPlay != this.Card && cep.CardEnteringPlay.DoKeywordsContain("memento"), MementoEntersPlayResponse, TriggerType.CancelAction, TriggerTiming.Before);
            base.AddTriggers();
        }

        private IEnumerator MementoEntersPlayResponse(CardEntersPlayAction cardEntersPlay)
        {
            IEnumerator coroutine;

            coroutine = CancelAction(cardEntersPlay);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = base.GameController.MoveCard(base.TurnTakerController, cardEntersPlay.CardEnteringPlay, base.TurnTaker.OutOfGame, toBottom: false, isPutIntoPlay: false, playCardIfMovingToPlayArea: true, null, showMessage: false, null, null, null, evenIfIndestructible: true, flipFaceDown: false, null, isDiscard: false, evenIfPretendGameOver: false, shuffledTrashIntoDeck: false, doesNotEnterPlay: false, GetCardSource());
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
