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

        public override void PrepareToUsePower(Power power)
        {
            base.PrepareToUsePower(power);
            if(power.IsInnatePower)
            {
                var partnerCard = TurnTaker.GetCardsWhere((Card c) => c.SharedIdentifier == this.CardWithoutReplacements.SharedIdentifier && c != this.CardWithoutReplacements).FirstOrDefault();
                HeroTurnTaker powerUser = null;
                if (power.TurnTakerController != null && power.TurnTakerController.TurnTaker.IsHero)
                {
                    powerUser = power.TurnTakerController.TurnTaker.ToHero();
                }
                if (partnerCard != null)
                {
                    //this adds an extra power-use record to the journal, of using the OTHER card's power
                    //WITHOUT actually ever 'using' a power, so it shouldn't cause extra triggers
                    //may cause problems with card that want to count how many powers a player has used in a turn, though
                    GameController.Game.Journal.RecordUsePower(partnerCard, power.Index, power.NumberOfUses, power.CardSource.Card, powerUser, false, power.CardController.CardWithoutReplacements.PlayIndex, power.CardSource.Card.PlayIndex, null, this.CardWithoutReplacements);
                }
            }
        }

        protected override IEnumerator RemoveCardsFromGame(IEnumerable<Card> cards)
        {
            if (!Card.IsInPlayAndHasGameText)
            {
                yield break;
            }
            IEnumerable<Card> enumerable = FindCardsWhere((Card c) => c != Card && c.SharedIdentifier != null && c.SharedIdentifier == Card.SharedIdentifier);
            foreach (Card item in enumerable)
            {
                if (!item.IsIncapacitated)
                {
                    IEnumerator coroutine = base.GameController.FlipCard(FindCardController(item), cardSource: GetCardSource());
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
            IEnumerator coroutine2 = base.RemoveCardsFromGame(cards);
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
}