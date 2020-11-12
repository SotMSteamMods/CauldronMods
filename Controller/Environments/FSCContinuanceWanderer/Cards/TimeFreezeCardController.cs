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
        #region Constructors

        public TimeFreezeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        #endregion Constructors

        #region Properties

        private Card cardThisIsNextTo;
        private HeroTurnTaker frozenTurnTaker;
        private TurnPhase triggerPhase;
        private TurnPhase skipToTurnPhase;

        #endregion Properties

        #region Methods

        public override IEnumerator Play()
        {
            //The card this is next to
            cardThisIsNextTo = base.GetCardThisCardIsNextTo();
            //The turn taker of the card this is next to
            frozenTurnTaker = cardThisIsNextTo.Owner.ToHero();
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
            Phase startPhase = Phase.Start;
            Phase endPhase = Phase.End;
            if (Game.IsOblivAeonMode)
            {
                //If we're playing OblivAeon then we need to grab the actual first and last phases of the game
                startPhase = Phase.BeforeStart;
                endPhase = Phase.AfterEnd;
            }
            //Turn taker after the frozen turn taker
            TurnTaker nextTurnTaker = Game.TurnTakers.ToList()[indexNextTurnTaker];
            //The phase where the Skip is applied
            triggerPhase = base.Game.FindTurnPhases((TurnPhase turnPhase) => turnPhase.Phase == endPhase && turnPhase.TurnTaker == Game.TurnTakers.ToList()[indexPrevTurnTaker]).FirstOrDefault();
            //The phase we will skip to
            skipToTurnPhase = base.Game.FindTurnPhases((TurnPhase turnPhase) => turnPhase.Phase == startPhase && turnPhase.TurnTaker == nextTurnTaker).FirstOrDefault();
            //If we are already in the phase that would cause the trigger to fire then manually fire the override
            if (Game.ActiveTurnPhase == triggerPhase)
            {
                base.GameController.Game.OverrideNextTurnPhase = skipToTurnPhase;
            }
            yield break;
        }

        public override void AddTriggers()
        {//base.GameController.Game.OverrideNextTurnPhase = lastTurnPhase;
            //That hero skips their turns...
            base.AddEndOfTurnTrigger((TurnTaker turnTaker) => turnTaker == triggerPhase.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(this.SkipTurnResponse), new TriggerType[] { TriggerType.SkipTurn });
            //...and targets in their play are are immune to damage.
            base.AddImmuneToDamageTrigger((DealDamageAction action) => action.Target.Location == cardThisIsNextTo.Location);
            //At the start of the environment turn, destroy this card.
            base.AddStartOfTurnTrigger((TurnTaker turnTaker) => turnTaker == base.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(base.DestroyThisCardResponse), TriggerType.DestroySelf);
        }

        private IEnumerator SkipTurnResponse(PhaseChangeAction action)
        {
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

        #endregion Methods
    }
}