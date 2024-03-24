using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace Cauldron.Dendron
{
    public class WindcolorDendronCharacterCardController : DendronBaseCharacterCardController
    {
        /*
         * Setup:
         *
         * At the start of the game, put {Dendron}'s villain character cards into play, 'Outside The Lines' side up.
         * Search the deck for all copies of Painted Viper and Stained Wolf. Place them beneath this card and shuffle the villain deck.
         *
         * Gameplay:
         *
         * Cards beneath this one have no game text and are not in play.
         * At the end of the villain turn, if there are no cards beneath this one, flip {Dendron}'s villain character cards.
         * Otherwise, put 2 random cards from beneath this one into play
         *
         * Advanced:
         *
         * When {Dendron} flips to this side, she regains 10 HP.
         *
         * Flipped Gameplay:
         *
         * At the start of the villain turn, destroy a villain ongoing card. If you do, {Dendron} deals each hero target 2 radiant damage.
         * Whenever a tattoo would be destroyed, place it beneath this card.
         * At the start of the villain turn, if there are at least 6 tattoos beneath this card, {Dendron} deals herself 10 toxic damage.
         * Then, flip {Dendron}'s villain character cards.
         * At the end of the villain turn, play the top card of the villain deck.
         *
         * Flipped Advanced:
         *
         * Increase damage dealt by {Dendron} to hero targets by {H - 2}.
         */

        public WindcolorDendronCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsAtLocation(CharacterCard.UnderLocation).Condition = () => !Card.IsFlipped;
            SpecialStringMaker.ShowNumberOfCardsAtLocation(CharacterCard.UnderLocation, cardCriteria: new LinqCardCriteria(c => IsTattoo(c), "tatoo")).Condition = () => Card.IsFlipped;
        }

        public override void AddSideTriggers()
        {
            // Front side (Outside the Lines)
            if (!base.Card.IsFlipped)
            {
                /*
                 * Cards beneath this one have no game text and are not in play.
                 * At the end of the villain turn, if there are no cards beneath this one, flip {Dendron}'s villain character cards.
                 * Otherwise, put 2 random cards from beneath this one into play
                 */
                base.Card.UnderLocation.OverrideIsInPlay = false;
                base.SideTriggers.Add(AddEndOfTurnTrigger(tt => tt == TurnTaker, FlipOrPlayFromBeneath, new[] { TriggerType.FlipCard, TriggerType.PutIntoPlay }));
            }
            // Flipped side (Colors of the Wind)
            else
            {
                /*
                 * At the start of the villain turn, destroy a villain ongoing card. If you do, {Dendron} deals each hero target 2 radiant damage.
                 * Whenever a tattoo would be destroyed, place it beneath this card.
                 * At the start of the villain turn, if there are at least 6 tattoos beneath this card, {Dendron} deals herself 10 toxic damage.
                 * Then, flip {Dendron}'s villain character cards.
                 * At the end of the villain turn, play the top card of the villain deck.
                 */
                base.Card.UnderLocation.OverrideIsInPlay = null;
                base.SideTriggers.Add(base.AddStartOfTurnTrigger(tt => tt == TurnTaker, DestroyOngoingToDealDamage, new[] { TriggerType.DestroyCard, TriggerType.DealDamage }));
                base.SideTriggers.Add(base.AddTrigger<DestroyCardAction>(dca => dca.CardToDestroy != null && IsTattoo(dca.CardToDestroy.Card), MoveBeneathThisCard, TriggerType.MoveCard, TriggerTiming.Before));
                base.SideTriggers.Add(base.AddStartOfTurnTrigger(tt => tt == TurnTaker && CharacterCard.UnderLocation.NumberOfCards >= 6, FlipBack, new[] { TriggerType.FlipCard, TriggerType.DealDamage }));
                base.SideTriggers.Add(base.AddEndOfTurnTrigger(tt => tt == TurnTaker, PlayTheTopCardOfTheVillainDeckWithMessageResponse, TriggerType.PlayCard));

                if (this.IsGameAdvanced)
                {
                    // Increase damage dealt by {Dendron} to hero targets by {H - 2}.
                    base.SideTriggers.Add(base.AddIncreaseDamageTrigger(dda => dda.DamageSource != null && dda.DamageSource.Card != null &&  dda.DamageSource.IsCard && dda.DamageSource.Card == CharacterCard && IsHero(dda.Target), H - 2));
                }
            }

            if(Game.IsChallenge)
            {
                base.SideTriggers.Add(base.AddImmuneToDamageTrigger((DealDamageAction dd) => dd.DamageSource != null && dd.DamageSource.Card != null && IsVillainTarget(dd.DamageSource.Card) && dd.Target == CharacterCard));
            }
        }

        public override void AddTriggers()
        {
            base.AddTriggers();
            base.AddDefeatedIfDestroyedTriggers();
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

            if (!Card.IsFlipped && Game.IsAdvanced)
            {
                IEnumerator restoreHpRoutine = base.GameController.GainHP(CharacterCard, 10, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(restoreHpRoutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(restoreHpRoutine);
                }
            }
        }

        private IEnumerator FlipOrPlayFromBeneath(PhaseChangeAction action)
        {
            /*
             * At the end of the villain turn, if there are no cards beneath this one, flip {Dendron}'s villain character cards.
             * Otherwise, put 2 random cards from beneath this one into play
             */

            if (CharacterCard.UnderLocation.NumberOfCards == 0)
            {
                var coroutine = GameController.SendMessageAction($"{CharacterCard.AlternateTitleOrTitle} has no cards to play, so she flips.", Priority.High, GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                coroutine = GameController.FlipCard(CharacterCardController, actionSource: action, cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            else
            {
                //get up to two cards from the underneth location.
                var coroutine = GameController.SendMessageAction($"{CharacterCard.AlternateTitleOrTitle} plays 2 random cards from beneath her.", Priority.High, GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                IEnumerator coroutine2 = base.RevealCards_MoveMatching_ReturnNonMatchingCards(base.TurnTakerController, CharacterCard.UnderLocation, false, true, false,cardCriteria: new LinqCardCriteria(c => true), new int?(2), shuffleBeforehand: true);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine2);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine2);
                }

            }
        }

        private IEnumerator DestroyOngoingToDealDamage(PhaseChangeAction action)
        {
            List<DestroyCardAction> results = new List<DestroyCardAction>();
            var coroutine = GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria(c => IsVillain(c) && IsOngoing(c) && c.IsInPlayAndHasGameText, "villain ongoing"), false, results, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (DidDestroyCard(results))
            {
                coroutine = DealDamage(CharacterCard, c => IsHeroTarget(c), 2, DamageType.Radiant);
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

        private IEnumerator MoveBeneathThisCard(DestroyCardAction action)
        {
            var coroutine = GameController.CancelAction(action, false, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = GameController.MoveCard(TurnTakerController, action.CardToDestroy.Card, CharacterCard.UnderLocation,
                            showMessage: true,
                            flipFaceDown: false,
                            actionSource: action,
                            cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator FlipBack(PhaseChangeAction action)
        {
            var coroutine = DealDamage(CharacterCard, CharacterCard, 10, DamageType.Toxic, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            coroutine = GameController.FlipCard(this, actionSource: action, cardSource: GetCardSource());
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