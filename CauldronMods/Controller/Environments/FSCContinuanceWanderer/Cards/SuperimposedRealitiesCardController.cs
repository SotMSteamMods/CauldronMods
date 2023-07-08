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

        public SuperimposedRealitiesCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        private Card cardThisIsNextTo => GetCardThisCardIsNextTo();
        private HeroTurnTaker superimposedTurnTaker => cardThisIsNextTo?.Owner.ToHero();

        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            //Play this card next to a hero.
            IEnumerator coroutine = base.SelectCardThisCardWillMoveNextTo(new LinqCardCriteria((Card c) =>  IsHeroCharacterCard(c) && !c.IsIncapacitatedOrOutOfGame && GameController.IsCardVisibleToCardSource(c, GetCardSource())), storedResults, true, decisionSources);
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

        public override void AddTriggers()
        {
            //Whenever another hero would play a card, use a power, or draw a card, instead the hero next to this card does that respective action.
            //When a Play Card/Use Power/Draw Card phase is entered then give the Superimposed target those actions instead
            //base.AddPhaseChangeTrigger((TurnTaker turnTaker) => IsHero(turnTaker) && turnTaker != cardThisIsNextTo.NativeDeck.OwnerTurnTaker, (Phase phase) => new Phase[] { Phase.PlayCard, Phase.UsePower, Phase.DrawCard }.Contains(phase), (PhaseChangeAction action) => new Phase[] { Phase.PlayCard, Phase.UsePower, Phase.DrawCard }.Contains(action.ToPhase.Phase), this.SuperimposedPhaseResponse, new TriggerType[] { TriggerType.SetPhaseActionCount, TriggerType.PreventPhaseAction }, TriggerTiming.After);
            //If a hero were to hero play. Instead the Superimposed plays.

            base.AddTrigger<PlayCardAction>((PlayCardAction action) => !action.IsPutIntoPlay && action.TurnTakerController.TurnTaker != superimposedTurnTaker && IsHero(action.TurnTakerController.TurnTaker) && GameController.IsTurnTakerVisibleToCardSource(action.TurnTakerController.TurnTaker, GetCardSource()), SuperimposePlayResponse, TriggerType.PlayCard, TriggerTiming.Before);

            //If a hero were to use a power. Instead the Superimposed plays.
            base.AddTrigger<UsePowerAction>((UsePowerAction action) => action.HeroUsingPower.TurnTaker != superimposedTurnTaker && GameController.IsTurnTakerVisibleToCardSource(action.HeroUsingPower.TurnTaker, GetCardSource()), SuperimposePowerResponse, TriggerType.UsePower, TriggerTiming.Before);
            //If a hero were todraw a card. Instead the Superimposed does.
            base.AddTrigger<DrawCardAction>((DrawCardAction action) => action.HeroTurnTaker != superimposedTurnTaker && GameController.IsTurnTakerVisibleToCardSource(action.HeroTurnTaker, GetCardSource()), SuperimposeDrawResponse, TriggerType.DrawCard, TriggerTiming.Before);

            //At the start of the environment turn, destroy this card.
            base.AddStartOfTurnTrigger((TurnTaker turnTaker) => turnTaker == base.TurnTaker, base.DestroyThisCardResponse, TriggerType.DestroySelf);
        }

        /*
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
        */


        //TODO - issue triggers from prevented card still exist for unknown reasons.
        private IEnumerator SuperimposePlayResponse(PlayCardAction action)
        {
            var coroutine = CancelAction(action, isPreventEffect: true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            System.Console.WriteLine($"### Canceled - {action.CardToPlay}");
                        
            var superImposed = FindHeroTurnTakerController(superimposedTurnTaker);

            //Test Behaves differently, Game doesn't return card to players hand.
            coroutine = GameController.MoveCard(null, action.CardToPlay, action.Origin, action.FromBottom, false, evenIfIndestructible: true, doesNotEnterPlay: true, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }


            coroutine = base.SelectAndPlayCardFromHand(superImposed, true);
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

        private IEnumerator SuperimposePowerResponse(UsePowerAction action)
        {
            IEnumerator coroutine = CancelAction(action);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            System.Console.WriteLine($"### Canceled - {action.Power}");

            coroutine = base.SelectAndUsePower(base.FindCardController(cardThisIsNextTo), true);
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

        private IEnumerator SuperimposeDrawResponse(DrawCardAction action)
        {
            var superImposed = FindHeroTurnTakerController(superimposedTurnTaker);
            IEnumerator coroutine = CancelAction(action);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            System.Console.WriteLine($"### Canceled - {action.CardToDraw}");

            coroutine = base.DrawCards(superImposed, 1, false);
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