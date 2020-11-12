using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.FSCContinuanceWanderer
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
            //When a Play Card/Use Power/Draw Card phase is entered then give the Superimposed target those actions instead
            //base.AddPhaseChangeTrigger((TurnTaker turnTaker) => turnTaker.IsHero && turnTaker != cardThisIsNextTo.NativeDeck.OwnerTurnTaker, (Phase phase) => new Phase[] { Phase.PlayCard, Phase.UsePower, Phase.DrawCard }.Contains(phase), (PhaseChangeAction action) => new Phase[] { Phase.PlayCard, Phase.UsePower, Phase.DrawCard }.Contains(action.ToPhase.Phase), this.SuperimposedPhaseResponse, new TriggerType[] { TriggerType.SetPhaseActionCount, TriggerType.PreventPhaseAction }, TriggerTiming.After);
            //If a hero were to hero play. Instead the Superimposed plays.
            base.AddTrigger<PlayCardAction>((PlayCardAction action) => action.TurnTakerController != superimposedTurnTakerController && action.TurnTakerController.IsHero, SuperimposePlayResponse, TriggerType.PlayCard, TriggerTiming.Before);
            //If a hero were to use a power. Instead the Superimposed plays.
            base.AddTrigger<UsePowerAction>((UsePowerAction action) => action.HeroUsingPower.TurnTaker != superimposedTurnTaker, SuperimposePowerResponse, TriggerType.UsePower, TriggerTiming.Before);
            //If a hero were todraw a card. Instead the Superimposed does.
            base.AddTrigger<DrawCardAction>((DrawCardAction action) => action.HeroTurnTaker != superimposedTurnTaker, SuperimposeDrawResponse, TriggerType.DrawCard, TriggerTiming.Before);

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
                cannotPlayCardsStatusEffect.TurnTakerCriteria.IsSpecificTurnTaker = action.FromPhase.TurnTaker;
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
                cannotUsePowersStatusEffect.TurnTakerCriteria.IsSpecificTurnTaker = action.FromPhase.TurnTaker;
                cannotUsePowersStatusEffect.UntilEndOfPhase(base.TurnTaker, turnPhase.Phase);
                coroutine2 = base.AddStatusEffect(cannotUsePowersStatusEffect);
            }
            else if (turnPhase.Phase == Phase.DrawCard)
            {
                coroutine = base.DrawCards(superimposedTurnTakerController, actionCount, true);
                PreventPhaseActionStatusEffect preventPhaseActionStatusEffect = new PreventPhaseActionStatusEffect();
                preventPhaseActionStatusEffect.ToTurnPhaseCriteria.Phase = new Phase?(Phase.DrawCard);
                preventPhaseActionStatusEffect.ToTurnPhaseCriteria.TurnTaker = action.FromPhase.TurnTaker;
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
            IEnumerator coroutine2 = base.SelectAndPlayCardFromHand(superimposedTurnTakerController, false);
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
            IEnumerator coroutine2 = base.SelectAndUsePower(base.FindCardController(cardThisIsNextTo), false);
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
            IEnumerator coroutine2 = base.DrawCards(superimposedTurnTakerController, 1, false);
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