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

        public TimeFreezeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        private TurnPhase triggerPhase;
        private TurnPhase skipToTurnPhase;

        public override IEnumerator Play()
        {
            //The turn taker of the card this is next to
            var frozenTurnTaker = base.GetCardThisCardIsNextTo().Owner;
            //The index of the turn taker this card is next to
            int indexFrozenTurnTaker = Game.TurnTakers.IndexOf(frozenTurnTaker) ?? default;
            //The index of the turn taker after the frozen turn taker
            int indexNextTurnTaker = indexFrozenTurnTaker + 1;
            if (indexNextTurnTaker >= Game.TurnTakers.Count())
            {
                //If the new index is outside of the list then take the first one
                indexNextTurnTaker -= Game.TurnTakers.Count() + 1;
            }
            //The index of the turn taker before the frozen turn taker
            int indexPrevTurnTaker = indexFrozenTurnTaker - 1;
            if (indexNextTurnTaker < 0)
            {
                //If the new index is outside of the list then take the last one
                indexNextTurnTaker += Game.TurnTakers.Count() + 1;
            }
            //If we're playing OblivAeon then we need to grab the actual phasess of the game
            Phase startPhase = Game.IsOblivAeonMode ? Phase.BeforeStart : Phase.Start;
            Phase endPhase = Game.IsOblivAeonMode ? Phase.AfterEnd : Phase.End;

            //Turn taker after the frozen turn taker
            TurnTaker nextTurnTaker = Game.TurnTakers.ElementAt(indexNextTurnTaker);

            //Turn taker before the frozen turn taker
            TurnTaker prevTurnTaker = Game.TurnTakers.ElementAt(indexPrevTurnTaker);

            //The phase where the Skip is applied
            triggerPhase = base.Game.FindTurnPhases((TurnPhase turnPhase) => turnPhase.Phase == endPhase && turnPhase.TurnTaker == prevTurnTaker).FirstOrDefault();

            Log.Debug($"## TimeFreeze.Play: triggerPhase = {triggerPhase.TurnTaker.Name}-{triggerPhase.FriendlyPhaseName}");

            //The phase we will skip to
            skipToTurnPhase = base.Game.FindTurnPhases((TurnPhase turnPhase) => turnPhase.Phase == startPhase && turnPhase.TurnTaker == nextTurnTaker).FirstOrDefault();

            //Console.WriteLine($"## alt trigger = {frozenLast}");
            Log.Debug($"## TimeFreeze.Play: skipToTurnPhase = {skipToTurnPhase.TurnTaker.Name}-{skipToTurnPhase.FriendlyPhaseName}");

            //If we are already in the phase that would cause the trigger to fire then manually fire the override
            if (Game.ActiveTurnPhase == triggerPhase)
            {
                Log.Debug($"## TimeFreeze.Play: ActiveGamePhase is trigger phase, skipping to skipTo phase.");

                base.GameController.Game.OverrideNextTurnPhase = skipToTurnPhase;
            }
            yield break;
        }

        public override void AddTriggers()
        {//base.GameController.Game.OverrideNextTurnPhase = lastTurnPhase;
            //That hero skips their turns...
            base.AddEndOfTurnTrigger((TurnTaker turnTaker) => turnTaker == triggerPhase.TurnTaker, this.SkipTurnResponse, new TriggerType[] { TriggerType.SkipTurn });
            //...and targets in their play are are immune to damage.
            base.AddImmuneToDamageTrigger((DealDamageAction action) => action.Target.Location == GetCardThisCardIsNextTo().Location);
            //At the start of the environment turn, destroy this card.
            base.AddStartOfTurnTrigger((TurnTaker turnTaker) => turnTaker == base.TurnTaker, base.DestroyThisCardResponse, TriggerType.DestroySelf);
        }

        private IEnumerator SkipTurnResponse(PhaseChangeAction action)
        {
            Log.Debug($"## TimeFreeze.SkipTurnResponse triggered");

            base.GameController.Game.OverrideNextTurnPhase = skipToTurnPhase;
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