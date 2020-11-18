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

        public static string Identifier = "ChokingInscription";

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
                // This hero may not draw cards until the start of their next turn.
                PreventPhaseActionStatusEffect preventPhaseActionStatusEffect = new PreventPhaseActionStatusEffect();
                preventPhaseActionStatusEffect.ToTurnPhaseCriteria.Phase = Phase.DrawCard;
                preventPhaseActionStatusEffect.ToTurnPhaseCriteria.TurnTaker = mostCardsInHandResults.First();
                preventPhaseActionStatusEffect.UntilStartOfNextTurn(mostCardsInHandResults.First());
                IEnumerator preventDrawPhaseRoutine = base.AddStatusEffect(preventPhaseActionStatusEffect);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(preventDrawPhaseRoutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(preventDrawPhaseRoutine);
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
                // This hero may not draw cards until the start of their next turn.
                PreventPhaseActionStatusEffect preventPhaseActionStatusEffect = new PreventPhaseActionStatusEffect();
                preventPhaseActionStatusEffect.ToTurnPhaseCriteria.Phase = Phase.PlayCard;
                preventPhaseActionStatusEffect.ToTurnPhaseCriteria.TurnTaker = mostCardsInPlayResults.First();
                preventPhaseActionStatusEffect.UntilStartOfNextTurn(mostCardsInPlayResults.First());
                IEnumerator preventDrawPhaseRoutine = base.AddStatusEffect(preventPhaseActionStatusEffect);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(preventDrawPhaseRoutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(preventDrawPhaseRoutine);
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