using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.FSCContinuanceWanderer
{
    public class TimeFreezeCardController : CardController
    {
        //This card is very fragile, test changes carefully.
        public TimeFreezeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            AddThisCardControllerToList(CardControllerListType.ChangesTurnTakerOrder);
        }
        private TurnTaker FrozenTurnTaker
        {
            get
            {
                if (this.Card.IsInPlayAndHasGameText)
                {
                    var frozenTurnTaker = GetCardThisCardIsNextTo()?.Location.HighestRecursiveLocation.OwnerTurnTaker;
                    if (frozenTurnTaker != null && frozenTurnTaker.IsHero)
                    {
                        return frozenTurnTaker;
                    }
                }
                return null;
            }
        }
        private bool AreTurnsReversed
        {
            get 
            {
                return GameController.FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && c.Identifier == "PlayingDiceWithTheCosmos").Any();
            }
        }
        public override TurnTaker AskIfTurnTakerOrderShouldBeChanged(TurnTaker fromTurnTaker, TurnTaker toTurnTaker)
        {
            var frozen = FrozenTurnTaker;
            if (frozen != null && !AreTurnsReversed)
            {
                if (toTurnTaker == frozen)
                {
                    var turnTakersInOrder = GameController.AllTurnTakers.ToList();
                    int? frozenIndex = turnTakersInOrder.IndexOf(frozen);
                    if(frozenIndex.HasValue)
                    {
                        return turnTakersInOrder[frozenIndex.Value + 1];
                    }
                }
            }
            return null;
        }

        public override void AddTriggers()
        {//base.GameController.Game.OverrideNextTurnPhase = lastTurnPhase;
            //That hero skips their turns...
            //mostly accomplished with AskIfTurnTakerOrderShouldBeChanged,
            //this is here to handle conflicts with Wager Master's 'Playing Dice With The Cosmos'
            AddPhaseChangeTrigger(tt => true, p => true, IsEnteringTurnReversedFrozenPhase, SkipToTurnReversedFollower, new TriggerType[] { TriggerType.SkipTurn, TriggerType.HiddenLast }, TriggerTiming.Before);

            //...and targets in their play are are immune to damage.
            base.AddImmuneToDamageTrigger((DealDamageAction action) => action.Target.Location.HighestRecursiveLocation == GetCardThisCardIsNextTo()?.Location.HighestRecursiveLocation);
            //At the start of the environment turn, destroy this card.
            base.AddStartOfTurnTrigger((TurnTaker turnTaker) => turnTaker == base.TurnTaker, base.DestroyThisCardResponse, TriggerType.DestroySelf);
        }

        private bool IsEnteringTurnReversedFrozenPhase(PhaseChangeAction pca)
        {
            var frozen = FrozenTurnTaker;
            if (pca.ToPhase.TurnTaker == frozen && pca.FromPhase.TurnTaker != frozen && AreTurnsReversed)
            {
                return true;
            }
            return false;
        }

        private IEnumerator SkipToTurnReversedFollower(PhaseChangeAction pca)
        {
            var frozen = FrozenTurnTaker;
            if (frozen == null)
            {
                yield break;
            }
            var frozenIndex = GameController.AllTurnTakers.IndexOf(frozen);
            var allHeroTurnTakers = GameController.AllTurnTakers.Where((TurnTaker tt) => tt.IsHero);

            //previous hero in normal turn order, or environment if it is the first
            var nextTurnTakerIndex = (frozenIndex ?? 0) - 1;
            if (frozen == allHeroTurnTakers.FirstOrDefault())
            {
                nextTurnTakerIndex = (GameController.AllTurnTakers.IndexOf(FindEnvironment().TurnTaker) ?? -1);
            }

            if (nextTurnTakerIndex == -1)
            {
                Log.Warning("Failed to find next turn taker for Time Freeze");
                yield break;
            }
            var nextTurnTaker = GameController.AllTurnTakers.ToList()[nextTurnTakerIndex];
            //Log.Debug($"Should skip to TurnTaker at index {nextTurnTakerIndex}, which is {nextTurnTaker.Name}");

            //make sure we skip to the right phase if the imp also has Breaking The Rules
            var nextTTStart = nextTurnTaker.TurnPhases.First();
            if(nextTurnTaker.IsHero && GameController.GetAllCards().Where((Card c) => c.IsInPlayAndHasGameText && c.Identifier == "BreakingTheRules").Any())
            {
                nextTTStart = nextTurnTaker.TurnPhases.Last();
            }

            IEnumerator coroutine = GameController.SkipToTurnPhase(nextTTStart, interruptActions: true, cardSource: GetCardSource());
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
        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            //Play this card next to a hero.
            IEnumerator coroutine = base.SelectCardThisCardWillMoveNextTo(new LinqCardCriteria((Card c) => c.IsHeroCharacterCard), storedResults, true, decisionSources);
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