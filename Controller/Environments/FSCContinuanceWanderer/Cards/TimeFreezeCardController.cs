using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron
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

        #endregion Properties

        #region Methods

        public override IEnumerator Play()
        {
            cardThisIsNextTo = base.GetCardThisCardIsNextTo();
            frozenTurnTaker = cardThisIsNextTo.Owner.ToHero();
            yield break;
        }

        public override void AddTriggers()
        {
            //That hero skips their turns...
            base.AddPhaseChangeTrigger((TurnTaker turnTaker) => turnTaker == frozenTurnTaker, (Phase phase) => new Phase[] { Phase.Start, Phase.BeforeStart }.Contains(phase), null, this.SkipTurnResponse, new TriggerType[] { TriggerType.SkipTurn }, TriggerTiming.Before);
            //...and targets in their play are are immune to damage.
            base.AddImmuneToDamageTrigger((DealDamageAction action) => action.Target.Location == cardThisIsNextTo.Location);
            //At the start of the environment turn, destroy this card.
            base.AddStartOfTurnTrigger((TurnTaker turnTaker) => turnTaker == base.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(base.DestroyThisCardResponse), TriggerType.DestroySelf);
        }

        private IEnumerator SkipTurnResponse(PhaseChangeAction action)
        {
            IEnumerator coroutine = this.GameController.SkipToNextTurn();
            if (this.UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(coroutine);
            }
            else
            {
                this.GameController.ExhaustCoroutine(coroutine);
            }
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