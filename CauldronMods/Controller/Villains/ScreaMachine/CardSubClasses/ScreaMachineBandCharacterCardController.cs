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
            _memberAbilityKey = member.GetAbilityKey();
            _memberKeyword = member.GetKeyword();
        }

        protected abstract string AbilityDescription { get; }

        protected abstract IEnumerator ActivateBandAbility();

        public override IEnumerator ActivateAbility(string abilityKey)
        {
            if (abilityKey == _memberAbilityKey)
                return ActivateBandAbility();

            return base.ActivateAbility(abilityKey);
        }

        public override IEnumerable<ActivatableAbility> GetActivatableAbilities(string key = null, TurnTakerController activatingTurnTaker = null)
        {
            if (key is null || key == _memberAbilityKey)
            {
                yield return new ActivatableAbility(TurnTakerController, this, _memberAbilityKey, AbilityDescription, ActivateAbility(_memberAbilityKey), 0, null, activatingTurnTaker, GetCardSource());
            }
        }

        public override void AddSideTriggers()
        {
            if (!Card.IsFlipped)
            {
                AddSideTrigger(AddTrigger<CardEntersPlayAction>(FlipCriteria, ca => base.FlipThisCharacterCardResponse(ca), TriggerType.FlipCard, TriggerTiming.After));
            }
            else
            {
                AddFlippedSideTriggers();
            }
        }

        private bool FlipCriteria(CardEntersPlayAction ga)
        {
            int count = base.FindCardsWhere(new LinqCardCriteria(c => c.IsInPlayAndNotUnderCard && GameController.DoesCardContainKeyword(c, _memberKeyword)), GetCardSource()).Count();

            return count >= 3;
        }

        protected abstract string UltimateFormMessage { get; }

        protected IEnumerator FlipBandmateResponse(GameAction action)
        {
            var coroutine = GameController.FlipCard(CharacterCardController, cardSource: GetCardSource());
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
