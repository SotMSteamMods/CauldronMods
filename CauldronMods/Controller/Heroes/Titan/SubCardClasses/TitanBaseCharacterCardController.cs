using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;

namespace Cauldron.Titan
{
    public abstract class TitanBaseCharacterCardController : HeroCharacterCardController
    {
        protected TitanBaseCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            base.AddTriggers();

            base.AddTrigger<DealDamageAction>(action => action.DamageSource != null &&  action.DamageSource.Card != null && action.DamageSource.Card.Owner == TurnTaker && Criteria(action), action => RestoreCharacterCard(), new TriggerType[] { TriggerType.Hidden, TriggerType.FirstTrigger }, TriggerTiming.After);
            base.AddTrigger<DestroyCardAction>(action => action.CardToDestroy.Card.Owner == TurnTaker && Criteria(action), action => RestoreCharacterCard(), new TriggerType[] { TriggerType.Hidden, TriggerType.FirstTrigger }, TriggerTiming.After);
            base.AddTrigger<PhaseChangeAction>(action => Criteria(action), action => RestoreCharacterCard(), new TriggerType[] { TriggerType.Hidden, TriggerType.FirstTrigger }, TriggerTiming.After);
        }

        private bool Criteria(GameAction action)
        {
            return GetCardPropertyJournalEntryBoolean(TitanformCardController.RevertToNormal) == true;
        }

        private IEnumerator RestoreCharacterCard()
        {
            var coroutine = ResetFlagAfterLeavesPlay(TitanformCardController.RevertToNormal);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            var characters = base.TurnTaker.GetAllCards().Where(c => c.IsCharacter).ToList();
            var baseCharacter = characters.First(c => c.Identifier.Contains("TitanCharacter"));
            baseCharacter.SetHitPoints(CharacterCardWithoutReplacements.HitPoints.Value);

            Log.Debug($"Switching to {baseCharacter.Identifier}:{baseCharacter.PromoIdentifierOrIdentifier}");
            Log.Debug($"What should happen is \"SwitchCutoutCard: from {CharacterCardWithoutReplacements.PromoIdentifierOrIdentifier} to {baseCharacter.PromoIdentifierOrIdentifier}\"");

            coroutine = GameController.SwitchCards(CharacterCardWithoutReplacements, baseCharacter, cardSource: GetCardSource());
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