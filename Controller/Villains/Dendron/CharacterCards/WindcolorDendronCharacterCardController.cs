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
         * Search the deck for all copies of Stained Wolf Painted Viper. Place them beneath this card and shuffle the villain deck.
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
                base.SideTriggers.Add(base.AddStartOfTurnTrigger(tt => tt == TurnTaker, DestroyOngoingToDealDamage, new[] { TriggerType.DestroyCard, TriggerType.DealDamage }));
                base.SideTriggers.Add(base.AddTrigger<DestroyCardAction>(dca => dca.CardToDestroy != null && IsTattoo(dca.CardToDestroy.Card), MoveBeneathThisCard, TriggerType.MoveCard, TriggerTiming.Before));
                base.SideTriggers.Add(base.AddStartOfTurnTrigger(tt => tt == TurnTaker && CharacterCard.UnderLocation.NumberOfCards >= 6, FlipBack, new[] { TriggerType.FlipCard, TriggerType.DealDamage }));
                base.SideTriggers.Add(base.AddEndOfTurnTrigger(tt => tt == TurnTaker, PlayCardResponse, TriggerType.PlayCard));

                if (this.IsGameAdvanced)
                {
                    // Increase damage dealt by {Dendron} to hero targets by {H - 2}.
                    base.SideTriggers.Add(base.AddIncreaseDamageTrigger(dda => dda.DamageSource.IsCard && dda.DamageSource.Card == CharacterCard && dda.Target.IsHero, H - 2));
                }
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
                var coroutine = GameController.SendMessageAction($"{CharacterCard.Title} has no cards to play, so she flips.", Priority.High, GetCardSource());
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
                var coroutine = GameController.SendMessageAction($"{CharacterCard.Title} plays 2 random cards from beneath her.", Priority.High, GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                var cards = CharacterCard.UnderLocation.Cards.ToList().Shuffle(Game.RNG);
                for (int index = 0; index < cards.Count; index++)
                {
                    if (index == 2)
                        break;

                    var card = cards[index];

                    coroutine = GameController.PlayCard(DecisionMaker, card,
                                    isPutIntoPlay: true, optional: false,
                                    reassignPlayIndex: true, actionSource: action,
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
            }
        }

        private IEnumerator DestroyOngoingToDealDamage(PhaseChangeAction action)
        {
            List<DestroyCardAction> results = new List<DestroyCardAction>();
            var coroutine = GameController.SelectAndDestroyCard(DecisionMaker, new LinqCardCriteria(c => IsVillain(c) && c.IsOngoing, "villain ongoing"), false, results, cardSource: GetCardSource());
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
                coroutine = DealDamage(CharacterCard, c => c.IsHero && c.IsTarget, 2, DamageType.Radiant);
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

        private IEnumerator PlayCardResponse(PhaseChangeAction pca)
        {
            IEnumerator playCardRoutine = base.GameController.PlayTopCard(DecisionMaker, TurnTakerController, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(playCardRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(playCardRoutine);
            }
        }
    }
}