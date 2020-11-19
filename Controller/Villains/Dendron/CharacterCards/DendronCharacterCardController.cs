using System.Collections;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace Cauldron.Dendron
{
    public class DendronCharacterCardController : VillainCharacterCardController
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


        public DendronCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            // Show the number of tattoos in play
            base.SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria(IsTattoo, "tattoo")).Condition = (() => true);
        }



        public override void AddSideTriggers()
        {
            // Front side
            if (!base.Card.IsFlipped)
            {
                // At the start and end of the villain turn, if there are fewer than {H - 2} tattoos in play,
                // play the top card of the villain deck
                base.SideTriggers.Add(base.AddEndOfTurnTrigger(taker => taker == base.TurnTaker, 
                    StartOfTurnResponse, new TriggerType[]
                {
                    TriggerType.PlayCard
                }));

                if (this.IsGameAdvanced)
                {
                    // Increase damage dealt by tattoos by 1.
                    base.AddIncreaseDamageTrigger(dda => dda.DamageSource != null && IsTattoo(dda.DamageSource.Card),
                        AdvancedTattooDamageIncrease);
                }
            }
            else
            {
                
            }

            base.AddDefeatedIfDestroyedTriggers();
        }

        public override IEnumerator DestroyAttempted(DestroyCardAction destroyCard)
        {
            if (base.Card.IsFlipped)
            {
                yield break;
            }

            // Flip Dendron
            IEnumerator flipRoutine = base.GameController.FlipCard(this, treatAsPlayed: false, treatAsPutIntoPlay: 
                false, destroyCard.ActionSource, null, GetCardSource());

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
            IEnumerator restoreHpRoutine =
                base.GameController.SetHP(this.Card, this.CharacterCard.MaximumHitPoints.Value, this.GetCardSource());

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

        private IEnumerator StartOfTurnResponse(PhaseChangeAction pca)
        {
            yield break;
        }

        private bool IsTattoo(Card card)
        {
            return card != null && base.GameController.DoesCardContainKeyword(card, "tattoo");
        }
    }
}