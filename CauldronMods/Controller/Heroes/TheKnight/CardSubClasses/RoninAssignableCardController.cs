using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.TheKnight
{
    public abstract class RoninAssignableCardController : TheKnightCardController
    {
        protected bool _useSpecialAssignment = false;
        protected RoninAssignableCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            IEnumerator coroutine;
            if(this.TurnTakerControllerWithoutReplacements.HasMultipleCharacterCards && CharacterCardController is WastelandRoninTheKnightCharacterCardController && !_useSpecialAssignment)
            {
                coroutine = SelectCardThisCardWillMoveNextTo(new LinqCardCriteria((Card c) => IsOwnCharacterCard(c) && c.IsInPlayAndHasGameText && !c.IsIncapacitatedOrOutOfGame, "Knight character"), storedResults, isPutIntoPlay, decisionSources);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                if(storedResults.Any() && storedResults.FirstOrDefault().Location.IsNextToCard)
                {
                    AddCardPropertyJournalEntry(RoninKey, storedResults.FirstOrDefault().Location.OwnerCard);
                }
            }
            else
            {
                coroutine = base.DeterminePlayLocation(storedResults, isPutIntoPlay, decisionSources, overridePlayArea, additionalTurnTakerCriteria);
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
            AddAfterLeavesPlayAction(ClearRoninOwner, TriggerType.Hidden);
        }

        private IEnumerator ClearRoninOwner(GameAction ga)
        {
            if (this.TurnTakerControllerWithoutReplacements.HasMultipleCharacterCards)
            {
                AddCardPropertyJournalEntry(RoninKey, (Card)null);
            }
            yield return null;
            yield break;
        }
    }
}
