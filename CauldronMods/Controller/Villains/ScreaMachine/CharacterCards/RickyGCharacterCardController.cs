using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.ScreaMachine
{
    public class RickyGCharacterCardController : ScreaMachineBandCharacterCardController
    {
        public RickyGCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, ScreaMachineBandmate.Value.RickyG)
        {
            SpecialStringMaker.ShowVillainTargetWithLowestHP().Condition = () => !Card.IsFlipped;
            SpecialStringMaker.ShowNumberOfCardsAtLocation(TurnTaker.Deck, new LinqCardCriteria(c => FindCardController(c) is ScreaMachineUtilityCardController sc && sc.AbilityIcons.Contains(ScreaMachineBandmate.Value.RickyG), "drum ability"))
                .Condition = () => Card.IsFlipped;
        }

        protected override string AbilityDescription => $"Select the villain target with the lowest HP. Reduce damage dealt to that target by 1 until the start of the next villain turn.";

        protected override string UltimateFormMessage => "The life of music is bigger than all of us."; //Cindy Blackman

        protected override IEnumerator ActivateBandAbility()
        {

            List<Card> lowest = new List<Card>();
            var coroutine = GameController.FindTargetWithLowestHitPoints(1, c => IsVillainTarget(c), lowest, cardSource: GetCardSource(), evenIfCannotDealDamage: true);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            var effect = new ReduceDamageStatusEffect(1);
            effect.TargetCriteria.IsSpecificCard = lowest.First();
            effect.CardSource = Card;
            effect.UntilStartOfNextTurn(TurnTaker);
            effect.UntilTargetLeavesPlay(lowest.First());

            coroutine = AddStatusEffect(effect);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
        }

        protected override void AddFlippedSideTriggers()
        {
            AddSideTrigger(AddImmuneToDamageTrigger(dda => dda.Target != Card && IsVillainTarget(dda.Target)));
            AddThisCardControllerToList(CardControllerListType.MakesIndestructible);

            AddSideTrigger(AddEndOfTurnTrigger(tt => tt == TurnTaker, pca => UltimateEndOfTurn(), new[] { TriggerType.DiscardCard, TriggerType.DealDamage }));
        }

        public override bool AskIfCardIsIndestructible(Card card)
        {
            if (Card.IsFlipped && card != Card && IsVillainTarget(card))
            {
                return true;
            }

            return base.AskIfCardIsIndestructible(card);
        }

        private IEnumerator UltimateEndOfTurn()
        {
            List<MoveCardAction> results = new List<MoveCardAction>();
            var coroutine = DiscardCardsFromTopOfDeck(TurnTakerController, 1, storedResults: results);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            var result = results.FirstOrDefault();
            var card = result?.CardToMove;
            bool wasDrumCard = false;
            if (card != null && FindCardController(card) is ScreaMachineUtilityCardController cc)
            {
                wasDrumCard = cc.AbilityIcons.Contains(ScreaMachineBandmate.Value.RickyG);
            }

            string msg = $"{TurnTaker.Deck.GetFriendlyName()} has no cards to discard.";
            if (card != null)
            {
                if (wasDrumCard)
                {
                    msg = $"[b]{Card.Title}[/b] discards {card.Title} and unleashes a [b]furious[/b] beatdown!";
                }
                else
                {
                    msg = $"{Card.Title} discards {card.Title}.";
                }
            }

            coroutine = GameController.SendMessageAction(msg, Priority.Medium, GetCardSource(), new[] { card });
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            if (wasDrumCard)
            {
                coroutine = DealDamage(Card, c => IsHeroTarget(c) && c.IsInPlayAndNotUnderCard, 3, DamageType.Melee);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }
        }
    }
}