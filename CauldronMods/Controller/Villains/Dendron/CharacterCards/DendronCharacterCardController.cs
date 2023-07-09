using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace Cauldron.Dendron
{
    public class DendronCharacterCardController : DendronBaseCharacterCardController
    {
        /*
         *
         * Setup:
         *
         * At the start of the game, put {Dendron}'s villain character cards into play, 'Mural of the Forest' side up.
         * Search the deck for 1 copy of Stained Wolf and 1 copy of Painted Viper and put them into play.
         *
         * Gameplay:
         *
         * At the start and end of the villain turn, if there are fewer than {H - 2} tattoos in play, play the top card of the villain deck.
         * When Dendron would be destroyed, flip her villain character cards instead.
         *
         * Advanced:
         *
         * Increase damage dealt by tattoos by 1.
         *
         * Flipped Gameplay:
         *
         * When {Dendron} is flipped to this side, restore her to 50 HP and shuffle the villain trash into the deck.
         * At the start and end of the villain turn, play the top card of the villain deck.
         *
         * Flipped Advanced:
         *
         * At the start of the villain turn, {Dendron} regains 5 HP.
         *
         *
         */

        private const int AdvancedTattooDamageIncrease = 1;
        private const int AdvancedHpGain = 5;

        public DendronCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            // Show the number of tattoos in play
            base.SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria(IsTattoo, "tattoo"));
            //challenge mode show highest hero target
            base.SpecialStringMaker.ShowHeroTargetWithHighestHP().Condition = () => Game.IsChallenge;
        }

        public override void AddSideTriggers()
        {
            // Front side (Mural of the Forest)
            if (!base.Card.IsFlipped)
            {
                // At the start and end of the villain turn, if there are fewer than {H - 2} tattoos in play,
                // play the top card of the villain deck

                base.SideTriggers.Add(base.AddStartOfTurnTrigger(taker => taker == base.TurnTaker, CheckTattooCountResponse, TriggerType.PlayCard));

                base.SideTriggers.Add(base.AddEndOfTurnTrigger(taker => taker == base.TurnTaker, CheckTattooCountResponse, TriggerType.PlayCard));

                if (this.IsGameAdvanced)
                {
                    // Increase damage dealt by tattoos by 1.
                    base.AddIncreaseDamageTrigger(dda => dda.DamageSource != null && dda.DamageSource.Card != null && IsTattoo(dda.DamageSource.Card), AdvancedTattooDamageIncrease);
                }

                base.AddDefeatedIfMovedOutOfGameTriggers();
            }
            // Flipped side (Unmarked Dryad)
            else
            {
                // At the start and end of the villain turn, play the top card of the villain deck.
                base.SideTriggers.Add(base.AddStartOfTurnTrigger(taker => taker == base.TurnTaker, PlayCardResponse, TriggerType.PlayCard));

                base.SideTriggers.Add(base.AddEndOfTurnTrigger(taker => taker == base.TurnTaker, PlayCardResponse, TriggerType.PlayCard));

                if (this.IsGameAdvanced)
                {
                    // At the start of the villain turn, {Dendron} regains 5 HP.
                    base.AddStartOfTurnTrigger(tt => tt.Equals(this.TurnTaker), GainHpResponse, TriggerType.GainHP);
                }

                base.AddDefeatedIfDestroyedTriggers();
            }

            if(Game.IsChallenge)
            {
                base.SideTriggers.Add(base.AddTrigger((CardEntersPlayAction cpa) => cpa.CardEnteringPlay != null && IsTattoo(cpa.CardEnteringPlay), ChallengeTattooEntersPlayResponse, TriggerType.DealDamage, TriggerTiming.After));
            }
        }

        private IEnumerator ChallengeTattooEntersPlayResponse(CardEntersPlayAction cpa)
        {
            Card newTattoo = cpa.CardEnteringPlay;
            IEnumerator coroutine = DealDamageToHighestHP(newTattoo, 1, (Card c) => IsHeroTarget(c) && GameController.IsCardVisibleToCardSource(c, FindCardController(newTattoo).GetCardSource()), (Card c) => Game.H - 2, DamageType.Infernal);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        public override bool CanBeDestroyed => base.CharacterCard.IsFlipped;

        public override IEnumerator DestroyAttempted(DestroyCardAction destroyCard)
        {
            if (base.Card.IsFlipped)
            {
                yield break;
            }

            // Flip Dendron
            IEnumerator flipRoutine = base.GameController.FlipCard(this, treatAsPlayed: false, treatAsPutIntoPlay: false, actionSource: destroyCard.ActionSource, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(flipRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(flipRoutine);
            }
        }

        public override IEnumerator AfterFlipCardImmediateResponse()
        {
            IEnumerator afterFlipRoutine = base.AfterFlipCardImmediateResponse();
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(afterFlipRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(afterFlipRoutine);
            }

            // Restore Dendron back to 50 HP
            IEnumerator restoreHpRoutine = base.GameController.SetHP(this.Card, this.CharacterCard.MaximumHitPoints.Value, this.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(restoreHpRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(restoreHpRoutine);
            }

            // Shuffle the villain trash into the deck
            IEnumerator shuffleTrashIntoDeckRoutine = this.GameController.ShuffleTrashIntoDeck(this.TurnTakerController);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(shuffleTrashIntoDeckRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(shuffleTrashIntoDeckRoutine);
            }

            yield break;
        }

        private IEnumerator CheckTattooCountResponse(PhaseChangeAction pca)
        {
            // If there are fewer than {H - 2} tattoos in play, play the top card of the villain deck

            int tattooThreshold = this.Game.H - 2;

            IEnumerable<Card> tattoosInPlay = base.GameController.FindCardsWhere(card => IsTattoo(card) && card.IsInPlay);
            if (tattoosInPlay.Count() >= tattooThreshold)
            {
                yield break;
            }

            // Tattoos in play don't meet the cutoff, play the top card of the villain deck
            IEnumerator playCardRoutine = base.GameController.PlayTopCardOfLocation(this.TurnTakerController, this.TurnTaker.Deck, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(playCardRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(playCardRoutine);
            }
        }

        private IEnumerator PlayCardResponse(PhaseChangeAction pca)
        {
            IEnumerator playCardRoutine = base.GameController.PlayTopCardOfLocation(this.TurnTakerController, this.TurnTaker.Deck, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(playCardRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(playCardRoutine);
            }
        }

        private IEnumerator GainHpResponse(PhaseChangeAction pca)
        {
            IEnumerator gainHpRoutine = this.GameController.GainHP(this.Card, AdvancedHpGain, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(gainHpRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(gainHpRoutine);
            }
        }
    }
}