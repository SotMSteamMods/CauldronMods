using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron
{
    public class SuperimposedRealitiesCardController : CardController
    {
        #region Constructors

        public SuperimposedRealitiesCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        #endregion Constructors

        #region Properties

        private Card cardThisIsNextTo;
        private HeroTurnTaker superimposedTurnTaker;
        private HeroTurnTakerController superimposedTurnTakerController;

        #endregion Properties

        #region Methods

        public override IEnumerator Play()
        {
            cardThisIsNextTo = base.GetCardThisCardIsNextTo();
            superimposedTurnTaker = cardThisIsNextTo.Owner.ToHero();
            superimposedTurnTakerController = base.FindHeroTurnTakerController(superimposedTurnTaker);
            yield break;
        }

        public override void AddTriggers()
        {
            //Whenever another hero would play a card, use a power, or draw a card, instead the hero next to this card does that respective action.
            base.AddPhaseChangeTrigger((TurnTaker turnTaker) => turnTaker.IsHero && turnTaker != cardThisIsNextTo.NativeDeck.OwnerTurnTaker, (Phase phase) => new Phase[] { Phase.PlayCard, Phase.UsePower, Phase.DrawCard }.Contains(phase), null, this.SuperimposedPhaseResponse, new TriggerType[] { TriggerType.SetPhaseActionCount, TriggerType.PreventPhaseAction }, TriggerTiming.After);
            base.AddTrigger<PlayCardAction>((PlayCardAction action) => action.DecisionMaker != superimposedTurnTakerController && action.DecisionMaker.IsHero, SuperimposePlayResponse, TriggerType.PlayCard, TriggerTiming.Before);
            base.AddTrigger<UsePowerAction>((UsePowerAction action) => action.DecisionMaker != superimposedTurnTakerController && action.DecisionMaker.IsHero, SuperimposePowerResponse, TriggerType.UsePower, TriggerTiming.Before);
            base.AddTrigger<DrawCardAction>((DrawCardAction action) => action.DecisionMaker != superimposedTurnTakerController && action.DecisionMaker.IsHero, SuperimposeDrawResponse, TriggerType.DrawCard, TriggerTiming.Before);
            //At the start of the environment turn, destroy this card.
            base.AddStartOfTurnTrigger((TurnTaker turnTaker) => turnTaker == base.TurnTaker, new Func<PhaseChangeAction, IEnumerator>(base.DestroyThisCardResponse), TriggerType.DestroySelf);
        }

        private IEnumerator SuperimposedPhaseResponse(PhaseChangeAction action)
        {
            TurnPhase turnPhase = base.Game.ActiveTurnPhase;
            int actionCount = turnPhase.GetPhaseActionCount() ?? default;
            IEnumerator coroutine = null;
            IEnumerator coroutine2 = null;
            if (turnPhase.Phase == Phase.PlayCard)
            {
                coroutine = base.SelectAndPlayCardsFromHand(superimposedTurnTakerController, actionCount, true);
                CannotPlayCardsStatusEffect cannotPlayCardsStatusEffect = new CannotPlayCardsStatusEffect();
                cannotPlayCardsStatusEffect.TurnTakerCriteria.IsSpecificTurnTaker = superimposedTurnTaker;
                cannotPlayCardsStatusEffect.UntilEndOfPhase(base.TurnTaker, turnPhase.Phase);
                coroutine2 = base.AddStatusEffect(cannotPlayCardsStatusEffect);
            }
            else if (turnPhase.Phase == Phase.UsePower)
            {
                for (int i = 0; i <= actionCount; i++)
                {
                    coroutine = base.SelectAndUsePower(base.FindCardController(cardThisIsNextTo), true);
                }
                CannotUsePowersStatusEffect cannotUsePowersStatusEffect = new CannotUsePowersStatusEffect();
                cannotUsePowersStatusEffect.TurnTakerCriteria.IsSpecificTurnTaker = superimposedTurnTaker;
                cannotUsePowersStatusEffect.UntilEndOfPhase(base.TurnTaker, turnPhase.Phase);
                coroutine2 = base.AddStatusEffect(cannotUsePowersStatusEffect);
            }
            else if (turnPhase.Phase == Phase.DrawCard)
            {
                coroutine = base.DrawCards(superimposedTurnTakerController, actionCount, true);
                PreventPhaseActionStatusEffect preventPhaseActionStatusEffect = new PreventPhaseActionStatusEffect();
                preventPhaseActionStatusEffect.ToTurnPhaseCriteria.Phase = new Phase?(Phase.DrawCard);
                preventPhaseActionStatusEffect.ToTurnPhaseCriteria.TurnTaker = superimposedTurnTaker;
                preventPhaseActionStatusEffect.UntilEndOfPhase(base.TurnTaker, turnPhase.Phase);
                coroutine2 = base.AddStatusEffect(preventPhaseActionStatusEffect);
            }
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
                yield return base.GameController.StartCoroutine(coroutine2);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
                base.GameController.ExhaustCoroutine(coroutine2);
            }
            yield break;
        }

        private IEnumerator SuperimposePlayResponse(PlayCardAction action)
        {
            IEnumerator coroutine = CancelAction(action);
            IEnumerator coroutine2 = base.SelectAndPlayCardFromHand(superimposedTurnTakerController);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
                yield return base.GameController.StartCoroutine(coroutine2);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
                base.GameController.ExhaustCoroutine(coroutine2);
            }
            yield break;
        }

        private IEnumerator SuperimposePowerResponse(UsePowerAction action)
        {
            IEnumerator coroutine = CancelAction(action);
            IEnumerator coroutine2 = base.SelectAndUsePower(base.FindCardController(cardThisIsNextTo), true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
                yield return base.GameController.StartCoroutine(coroutine2);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
                base.GameController.ExhaustCoroutine(coroutine2);
            }
            yield break;
        }

        private IEnumerator SuperimposeDrawResponse(DrawCardAction action)
        {
            IEnumerator coroutine = CancelAction(action);
            IEnumerator coroutine2 = base.DrawCards(superimposedTurnTakerController, actionCount, true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
                yield return base.GameController.StartCoroutine(coroutine2);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
                base.GameController.ExhaustCoroutine(coroutine2);
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

        #endregion Methods
    }
}