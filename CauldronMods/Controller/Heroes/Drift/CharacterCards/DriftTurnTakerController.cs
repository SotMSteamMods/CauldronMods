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
        protected const string Base = "Base";
        protected const string Dual = "Dual";
        protected const string ThroughTheBreach = "ThroughTheBreach";

        public override IEnumerator StartGame()
        {
            string promoIdentifier = Base;
            if (base.CharacterCardController is DualDriftCharacterCardController)
            {
                promoIdentifier = Dual;
            }
            if (base.CharacterCardController is ThroughTheBreachDriftCharacterCardController)
            {
                promoIdentifier = ThroughTheBreach;
            }

            string[] tracks = new string[] {
                promoIdentifier + ShiftTrack + 1,
                promoIdentifier + ShiftTrack + 2,
                promoIdentifier + ShiftTrack + 3,
                promoIdentifier + ShiftTrack + 4,
            };

            //At the start of the game, after drawing your cards, place a token on 1 of the 4 spaces of the shift track.
            List<SelectCardDecision> cardDecisions = new List<SelectCardDecision>();
            IEnumerator coroutine = base.GameController.SelectCardAndStoreResults(this, SelectionType.AddTokens, new LinqCardCriteria((Card c) => c.SharedIdentifier == ShiftTrack && tracks.Contains(c.Identifier), "Shift Track Position"), cardDecisions, false, includeRealCardsOnly: false);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }


            Card selectedTrack = cardDecisions.FirstOrDefault().SelectedCard;
            coroutine = base.GameController.PlayCard(this, selectedTrack);
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
            if (selectTrackController is BaseShiftTrack1CardController || selectTrackController is DualShiftTrack1CardController || selectTrackController is ThroughTheBreachShiftTrack1CardController)
            {
                tokensToAdd = 1;
            }
            else if (selectTrackController is BaseShiftTrack2CardController || selectTrackController is DualShiftTrack2CardController || selectTrackController is ThroughTheBreachShiftTrack2CardController)
            {
                tokensToAdd = 2;
            }
            else if (selectTrackController is BaseShiftTrack3CardController || selectTrackController is DualShiftTrack3CardController || selectTrackController is ThroughTheBreachShiftTrack3CardController)
            {
                tokensToAdd = 3;
            }
            else if (selectTrackController is BaseShiftTrack4CardController || selectTrackController is DualShiftTrack4CardController || selectTrackController is ThroughTheBreachShiftTrack4CardController)
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

            //Then place 1 of your 2 character cards (1929 or 2199) next to that same space, inactive. Place your other character card into play, active.
            if (base.CharacterCardController is DualDriftCharacterCardController)
            {
                List<SelectCardDecision> selectDriftDecision = new List<SelectCardDecision>();
                coroutine = base.GameController.SelectCardAndStoreResults(this, SelectionType.RemoveCardFromGame, new LinqCardCriteria((Card c) => base.FindCardController(c) is DualDriftSubCharacterCardController), selectDriftDecision, false);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                //Place your other character card into play, active.
                Card selectedDrift = selectDriftDecision.FirstOrDefault().SelectedCard;
                Card driftToPlay = base.GameController.FindCardsWhere((Card c) => base.FindCardController(c) is DualDriftSubCharacterCardController && selectedDrift != c).FirstOrDefault();
                coroutine = base.GameController.SwitchCards(base.CharacterCard, driftToPlay);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                //Then place 1 of your 2 character cards (1929 or 2199) next to that same space
                base.GameController.AddCardPropertyJournalEntry(selectedTrack, "DriftPosition" + tokensToAdd, true);
            }
            yield break;
        }
    }
}