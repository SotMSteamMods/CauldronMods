using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Drift
{
    public class DriftTurnTakerController : HeroTurnTakerController
    {
        public DriftTurnTakerController(TurnTaker turnTaker, GameController gameController) : base(turnTaker, gameController)
        {

        }
        protected const string ShiftTrack = "ShiftTrack";

        public override IEnumerator StartGame()
        {
            List<SelectCardDecision> cardDecisions = new List<SelectCardDecision>();
            IEnumerator coroutine = base.GameController.SelectCardAndStoreResults(this, SelectionType.AddTokens, new LinqCardCriteria((Card c) => c.SharedIdentifier == ShiftTrack, "Shift Track Position"), cardDecisions, false, includeRealCardsOnly: false);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = this.SetupShiftTrack(cardDecisions.FirstOrDefault());
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

        private IEnumerator SetupShiftTrack(SelectCardDecision decision)
        {
            Card selectedTrack = decision.SelectedCard;
            IEnumerator coroutine = base.GameController.PlayCard(this, selectedTrack);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            CardController selectTrackController = base.FindCardController(selectedTrack);
            int tokensToAdd = 0;
            if (selectTrackController is ShiftTrack1CardController)
            {
                tokensToAdd = 1;
            }
            else if (selectTrackController is ShiftTrack2CardController)
            {
                tokensToAdd = 2;
            }
            else if (selectTrackController is ShiftTrack3CardController)
            {
                tokensToAdd = 3;
            }
            else if (selectTrackController is ShiftTrack4CardController)
            {
                tokensToAdd = 4;
            }

            coroutine = base.GameController.AddTokensToPool(selectedTrack.FindTokenPool("ShiftPool"), tokensToAdd, new CardSource(base.CharacterCardController));
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