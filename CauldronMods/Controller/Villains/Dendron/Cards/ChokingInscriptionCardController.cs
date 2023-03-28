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
        public static readonly string PreventDrawPropertyKey = Identifier + "TurnTakerCannotDraw";

        public ChokingInscriptionCardController(Card card, TurnTakerController turnTakerController) : base(card,
            turnTakerController)
        {
            SpecialStringMaker.ShowHeroWithMostCards(true);
            SpecialStringMaker.ShowHeroWithMostCards(false);
        }

        public override IEnumerator Play()
        {
            // Find hero with the most cards in hand
            List<TurnTaker> excludedTurnTakers = new List<TurnTaker>();
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
                var biggestHandTurnTaker = mostCardsInHandResults.First();
                excludedTurnTakers.Add(biggestHandTurnTaker);

                //The status effect must last slightly longer than the triggering phase action, or the effect will not fire.
                OnPhaseChangeStatusEffect effect = new OnPhaseChangeStatusEffect(CardWithoutReplacements, nameof(PreventDrawsThisTurnEffect), $"{biggestHandTurnTaker.Name} cannot draw cards on their turn.", new TriggerType[] { TriggerType.CreateStatusEffect }, base.Card);
                effect.TurnTakerCriteria.IsSpecificTurnTaker = biggestHandTurnTaker;
                effect.TurnPhaseCriteria.Phase = Phase.Start;
                effect.UntilEndOfNextTurn(biggestHandTurnTaker);
                effect.CanEffectStack = true;
                IEnumerator onPhaseChangeRoutine = base.AddStatusEffect(effect);

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
                var mostCardsTurnTaker = mostCardsInPlayResults.First();
                excludedTurnTakers.Add(mostCardsTurnTaker);

                //The status effect must last slightly longer than the triggering phase action, or the effect will not fire.
                // This hero may not play cards during their next turn.
                OnPhaseChangeStatusEffect effect = new OnPhaseChangeStatusEffect(CardWithoutReplacements, nameof(PreventPlaysThisTurnEffect), $"{mostCardsTurnTaker.Name} cannot play cards on their turn.", new TriggerType[] { TriggerType.CreateStatusEffect }, base.Card);
                effect.TurnTakerCriteria.IsSpecificTurnTaker = mostCardsTurnTaker;
                effect.TurnPhaseCriteria.Phase = Phase.Start;
                effect.UntilEndOfNextTurn(mostCardsTurnTaker);
                effect.CanEffectStack = true;
                IEnumerator onPhaseChangeRoutine = base.AddStatusEffect(effect);

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
            IEnumerator shuffleRoutine = base.DoActionToEachTurnTakerInTurnOrder(ttc => IsHero(ttc.TurnTaker) && !ttc.IsIncapacitatedOrOutOfGame && !excludedTurnTakers.Contains(ttc.TurnTaker), ShuffleTrashResponse);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(shuffleRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(shuffleRoutine);
            }
        }

        public IEnumerator PreventPlaysThisTurnEffect(PhaseChangeAction _, OnPhaseChangeStatusEffect sourceEffect)
        {
            System.Console.WriteLine("### DEBUG ### - ChokingInspiration.PreventPlaysThisTurnEffect triggered");

            CannotPlayCardsStatusEffect effect = new CannotPlayCardsStatusEffect
            {
                CardSource = sourceEffect.CardSource
            };
            effect.TurnTakerCriteria.IsSpecificTurnTaker = base.Game.ActiveTurnTaker;
            effect.UntilThisTurnIsOver(base.Game);
            IEnumerator coroutine = base.AddStatusEffect(effect);
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

        public IEnumerator PreventDrawsThisTurnEffect(PhaseChangeAction _, OnPhaseChangeStatusEffect sourceEffect)
        {
            System.Console.WriteLine("### DEBUG ### - ChokingInspiration.PreventDrawsThisTurnEffect triggered");

            //The status effect must last slightly longer than the triggering phase action, or the effect will not fire.
            HeroTurnTaker htt = sourceEffect.TurnTakerCriteria.IsSpecificTurnTaker.ToHero();
            OnPhaseChangeStatusEffect effect = new OnPhaseChangeStatusEffect(CardWithoutReplacements, nameof(ResumeDrawEffect), $"{htt.Name} cannot draw cards.", new TriggerType[] { TriggerType.Hidden }, base.Card);
            effect.TurnTakerCriteria.IsSpecificTurnTaker = htt;
            effect.TurnPhaseCriteria.Phase = Phase.End;
            effect.CanEffectStack = true;
            effect.UntilThisTurnIsOver(Game);

            //We secretly set a property on the victim's character card to indicate that they can't draw cards.
            //A CannotDrawCards query on DendronCharacterCardController actually makes this happen
            GameController.AddCardPropertyJournalEntry(htt.CharacterCards.First(), PreventDrawPropertyKey, true);

            IEnumerator coroutine = base.AddStatusEffect(effect);
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

        public IEnumerator ResumeDrawEffect(PhaseChangeAction _, OnPhaseChangeStatusEffect sourceEffect)
        {
            System.Console.WriteLine("### DEBUG ### - ChokingInspiration.ResumeDrawEffect triggered");

            HeroTurnTaker htt = sourceEffect.TurnTakerCriteria.IsSpecificTurnTaker.ToHero();

            //Clear the secret property from all Character Cards of the victim, just to be sure
            foreach (var ch in htt.CharacterCards)
            {
                GameController.AddCardPropertyJournalEntry(ch, PreventDrawPropertyKey, (bool?)null);
            }

            return DoNothing();
        }

        private IEnumerator ShuffleTrashResponse(TurnTakerController turnTakerController)
        {
            // Shuffle trash to deck
            IEnumerator coroutine = base.GameController.ShuffleTrashIntoDeck(turnTakerController, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }
    }
}