using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace Cauldron.Dendron
{
    public class ChokingInscriptionCardController : CardController
    {
        //==============================================================
        // The hero with the most cards in hand cannot draw cards during their next turn.
        // The hero with the most cards in play cannot play cards during their next turn.
        // All other heroes shuffle their trash into their decks.
        //==============================================================

        public static readonly string Identifier = "ChokingInscription";

        public ChokingInscriptionCardController(Card card, TurnTakerController turnTakerController) : base(card,
            turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            // Find hero with the most cards in hand
            List<TurnTaker> mostCardsInHandResults = new List<TurnTaker>();
            IEnumerator heroWithMostCardsInHandRoutine = base.FindHeroWithMostCardsInHand(mostCardsInHandResults);

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(heroWithMostCardsInHandRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(heroWithMostCardsInHandRoutine);
            }

            if (mostCardsInHandResults.Any())
            {
                // This hero may not draw cards during their next turn.
                //PreventPhaseActionStatusEffect preventPhaseActionStatusEffect = new PreventPhaseActionStatusEffect();
                //preventPhaseActionStatusEffect.ToTurnPhaseCriteria.Phase = Phase.DrawCard;
                //preventPhaseActionStatusEffect.ToTurnPhaseCriteria.TurnTaker = mostCardsInHandResults.First();
                //preventPhaseActionStatusEffect.UntilEndOfNextTurn(mostCardsInHandResults.First());
                //IEnumerator preventDrawPhaseRoutine = base.AddStatusEffect(preventPhaseActionStatusEffect);
                //if (base.UseUnityCoroutines)
                //{
                //    yield return base.GameController.StartCoroutine(preventDrawPhaseRoutine);
                //}
                //else
                //{
                //    base.GameController.ExhaustCoroutine(preventDrawPhaseRoutine);
                //}

                OnPhaseChangeStatusEffect onPhaseChangeStatusEffect = new OnPhaseChangeStatusEffect(base.CardWithoutReplacements, "PreventDrawsThisTurnEffect", mostCardsInHandResults.First().CharacterCard.Title + " cannot draw cards on their turn.", new TriggerType[] { TriggerType.CreateStatusEffect }, base.Card);
                onPhaseChangeStatusEffect.TurnTakerCriteria.IsSpecificTurnTaker = mostCardsInHandResults.First();
                onPhaseChangeStatusEffect.TurnPhaseCriteria.Phase = Phase.Start;
                onPhaseChangeStatusEffect.UntilEndOfNextTurn(mostCardsInHandResults.First());
                IEnumerator onPhaseChangeRoutine = base.AddStatusEffect(onPhaseChangeStatusEffect);

                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(onPhaseChangeRoutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(onPhaseChangeRoutine);
                }
            }

            // Find hero with most cards in play
            List<TurnTaker> mostCardsInPlayResults = new List<TurnTaker>();
            IEnumerator heroWithMostCardsInPlayRoutine = base.FindHeroWithMostCardsInPlay(mostCardsInPlayResults);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(heroWithMostCardsInPlayRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(heroWithMostCardsInPlayRoutine);
            }

            if (mostCardsInPlayResults.Any())
            {
                // This hero may not play cards during their next turn.
                OnPhaseChangeStatusEffect onPhaseChangeStatusEffect = new OnPhaseChangeStatusEffect(base.CardWithoutReplacements, "PreventPlaysThisTurnEffect", mostCardsInPlayResults.First().CharacterCard.Title + " cannot play cards on their turn.", new TriggerType[] { TriggerType.CreateStatusEffect }, base.Card);
                onPhaseChangeStatusEffect.TurnTakerCriteria.IsSpecificTurnTaker = mostCardsInPlayResults.First();
                onPhaseChangeStatusEffect.TurnPhaseCriteria.Phase = Phase.Start;
                onPhaseChangeStatusEffect.UntilEndOfNextTurn(mostCardsInPlayResults.First());
                IEnumerator onPhaseChangeRoutine = base.AddStatusEffect(onPhaseChangeStatusEffect);

                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(onPhaseChangeRoutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(onPhaseChangeRoutine);
                }
            }

            // All other heroes shuffle their trash into their decks
            IEnumerator shuffleRoutine
                    = base.DoActionToEachTurnTakerInTurnOrder(
                        ttc => ttc.IsHero
                                   && !mostCardsInHandResults.Any(tt => tt.Equals(ttc.TurnTaker))
                                   && !mostCardsInPlayResults.Any(tt => tt.Equals(ttc.TurnTaker)),
                        ShuffleTrashResponse);

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(shuffleRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(shuffleRoutine);
            }
        }

        public IEnumerator PreventPlaysThisTurnEffect(PhaseChangeAction p, OnPhaseChangeStatusEffect effect)
        {
            CannotPlayCardsStatusEffect cannotPlayCardsStatusEffect = new CannotPlayCardsStatusEffect();
            cannotPlayCardsStatusEffect.TurnTakerCriteria.IsSpecificTurnTaker = base.Game.ActiveTurnTaker;
            cannotPlayCardsStatusEffect.UntilThisTurnIsOver(base.Game);
            IEnumerator cannotPlayCardsRoutine = base.AddStatusEffect(cannotPlayCardsStatusEffect);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(cannotPlayCardsRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(cannotPlayCardsRoutine);
            }
            yield break;
        }

        public IEnumerator PreventDrawsThisTurnEffect(PhaseChangeAction p, OnPhaseChangeStatusEffect effect)
        {
            //prevent drawing cards in some way

            HeroTurnTaker htt = base.Game.ActiveTurnTaker.ToHero();

            OnDrawCardStatusEffect onDrawCardStatusEffect = new OnDrawCardStatusEffect(base.CardWithoutReplacements, "CancelDraws", "", new TriggerType[] { TriggerType.CancelAction }, htt, false, base.Card);
            onDrawCardStatusEffect.UntilThisTurnIsOver(base.Game);
            onDrawCardStatusEffect.BeforeOrAfter = BeforeOrAfter.Before;
            IEnumerator coroutine = base.AddStatusEffect(onDrawCardStatusEffect);
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

        public IEnumerator CancelDraws(DrawCardAction dc, HeroTurnTaker htt, OnDrawCardStatusEffect effect)
        {
            if (dc.HeroTurnTaker != htt)
            {
                yield break;
            }

            IEnumerator coroutine = base.CancelAction(dc, isPreventEffect: true);
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

        private IEnumerator ShuffleTrashResponse(TurnTakerController turnTakerController)
        {
            // Shuffle trash to deck
            IEnumerator shuffleTrashIntoDeckRoutine = base.GameController.ShuffleTrashIntoDeck(turnTakerController);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(shuffleTrashIntoDeckRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(shuffleTrashIntoDeckRoutine);
            }
        }
    }
}