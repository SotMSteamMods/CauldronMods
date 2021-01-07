using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.ScreaMachine
{
    public abstract class ScreaMachineBandCharacterCardController : ScreaMachineUtilityCharacterCardController
    {
        public ScreaMachineBandmate.Value Member { get; }
        private readonly string _memberKeyword;
        private readonly string _memberAbilityKey;

        protected ScreaMachineBandCharacterCardController(Card card, TurnTakerController turnTakerController, ScreaMachineBandmate.Value member) : base(card, turnTakerController)
        {
            Member = member;
            _memberAbilityKey = member.GetAbilityKey();
            _memberKeyword = member.GetKeyword();
        }

        protected abstract string AbilityDescription { get; }

        protected abstract IEnumerator ActivateBandAbility();

        public override IEnumerator ActivateAbility(string abilityKey)
        {
            if (!Card.IsFlipped && abilityKey == _memberAbilityKey)
                return ActivateBandAbility();

            return base.ActivateAbility(abilityKey);
        }

        public override IEnumerable<ActivatableAbility> GetActivatableAbilities(string key = null, TurnTakerController activatingTurnTaker = null)
        {
            if (!Card.IsFlipped && (key is null || key == _memberAbilityKey))
            {
                yield return new ActivatableAbility(TurnTakerController, this, _memberAbilityKey, AbilityDescription, ActivateAbility(_memberAbilityKey), 0, null, activatingTurnTaker, GetCardSource());
            }
        }

        public override void AddSideTriggers()
        {
            if (!Card.IsFlipped)
            {
                AddSideTrigger(AddTrigger<MoveCardAction>(mca => mca.Destination.IsInPlayAndNotUnderCard && mca.WasCardMoved && FlipCriteria(mca.CardToMove), ca => base.FlipThisCharacterCardResponse(ca), TriggerType.FlipCard, TriggerTiming.After));
                AddSideTrigger(AddTrigger<PlayCardAction>(pca => pca.WasCardPlayed && FlipCriteria(pca.CardToPlay), ca => FlipBandmateResponse(ca), TriggerType.FlipCard, TriggerTiming.After));
            }
            else
            {
                AddFlippedSideTriggers();
            }
        }

        private bool FlipCriteria(Card card)
        {
            if (!card.IsVillain)
                return false;

            var cc = FindCardController(card);
            if (cc is ScreaMachineBandCardController bandCC && bandCC.Member == this.Member)
            {
                var cards = GameController.FindCardsWhere(c => c.IsInPlayAndNotUnderCard && c.DoKeywordsContain(_memberKeyword, true, true), true, GetCardSource()).ToList(); ;
                Console.WriteLine($"DEBUG - {Card.Title} has {cards.Count} {_memberKeyword} cards in play.");
                return cards.Count >= 3;
            }

            return false;
        }

        protected abstract string UltimateFormMessage { get; }

        protected IEnumerator FlipBandmateResponse(GameAction action)
        {
            var coroutine = GameController.FlipCard(this, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
        }

        public override IEnumerator AfterFlipCardImmediateResponse()
        {
            var coroutine = base.AfterFlipCardImmediateResponse();
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = GameController.SendMessageAction(UltimateFormMessage, Priority.High, GetCardSource(), showCardSource: true);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
        }

        protected abstract void AddFlippedSideTriggers();

        public override MoveCardDestination GetTrashDestination()
        {
            return new MoveCardDestination(TurnTaker.OutOfGame);
        }
    }
}
