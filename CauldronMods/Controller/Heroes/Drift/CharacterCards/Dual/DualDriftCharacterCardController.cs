using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Drift
{
    public class DualDriftCharacterCardController : DriftSubCharacterCardController
    {
        public DualDriftCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        protected override bool shouldRunSetUp => true;

        public override IEnumerator PerformEnteringGameResponse()
        {
            IEnumerator coroutine = base.PerformEnteringGameResponse();
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            //Then place 1 of your 2 character cards (1929 or 2199) next to that same space, inactive. Place your other character card into play, active.
            if (CharacterCardController is DualDriftCharacterCardController)
            {
                List<SelectCardDecision> selectDriftDecision = new List<SelectCardDecision>();
                customMode = CustomMode.StartOfGameChooseDrift;
                coroutine = GameController.SelectCardAndStoreResults(HeroTurnTakerController, SelectionType.Custom, new LinqCardCriteria((Card c) => FindCardController(c) is DualDriftSubCharacterCardController), selectDriftDecision, false, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }

                if(!DidSelectCard(selectDriftDecision))
                {
                    yield break;
                }

                //Place your other character card into play, active.
                Card driftToPlay = GetSelectedCard(selectDriftDecision);
                Card driftToNotUse = GameController.FindCardsWhere((Card c) => FindCardController(c) is DualDriftSubCharacterCardController && driftToPlay != c).FirstOrDefault();
                coroutine = GameController.SwitchCards(CharacterCard, driftToPlay);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }

                MoveCardAction moveCardAction = new MoveCardAction(GetCardSource(), driftToNotUse, TurnTaker.OffToTheSide, false, 0, null, TurnTaker, false, null, false, false, false, false);
                moveCardAction.AllowTriggersToRespond = false;
                moveCardAction.CanBeCancelled = false;
                coroutine = GameController.DoAction(moveCardAction);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
                //Then place 1 of your 2 character cards (1929 or 2199) next to that same space
                GameController.AddCardPropertyJournalEntry(GetShiftTrack(), "DriftPosition" + GetShiftPool().CurrentValue, true);

                yield break;
            }
        }

        public override CustomDecisionText GetCustomDecisionText(IDecision decision)
        {
            if (customMode == CustomMode.StartOfGameChooseDrift)
            {
                return new CustomDecisionText("Which Drift do you want to start in play", "Which Drift should they start in play?", "Vote for which Drift should start in play?", "Drift to start in play");
            }

            return base.GetCustomDecisionText(decision);
        }
    }
}
