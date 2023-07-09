using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Gray
{
    public class ContaminationCardController : CardController
    {

        public ContaminationCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowIfElseSpecialString(() => GetHeroCardsDestroyedThisRound().Where(e => (IsOngoing(e.Card) || IsEquipment(e.Card)) && e.CardSource == this.Card).Count() > 0, () => GetHeroCardsDestroyedThisRound().Where(e => (IsOngoing(e.Card) || IsEquipment(e.Card)) && e.CardSource == this.Card).Count() + " hero card(s) have been destroyed by this card this round.", () => "No hero cards have been destroyed by this card this round.");
        }

        private bool SelfDestructionCriteria(DestroyCardAction action)
        {
            //Destroy this card when {H} hero cards are destroyed this way in one round.
            //prefilter to events that destroyed a hero ongoing before hitting the Journal
            var destroyedCard = action.CardToDestroy?.Card;

            return  destroyedCard != null && IsHero(destroyedCard) && (IsOngoing(destroyedCard) || IsEquipment(destroyedCard)) &&
                    this.GetHeroCardsDestroyedThisRound().Where(e => (IsOngoing(e.Card) || IsEquipment(e.Card)) && e.CardSource == this.Card).Count() >= Game.H;
        }

        public override void AddTriggers()
        {
            //Whenever a hero deals damage to a villain target, that hero must destroy 1 of their ongoing or equipment cards.
            base.AddTrigger<DealDamageAction>((DealDamageAction action) => action.DamageSource != null && action.DamageSource.Card != null && action.DamageSource.IsHero && IsVillain(action.Target) && action.IsSuccessful, new Func<DealDamageAction, IEnumerator>(this.DestroyHeroCardResponse), TriggerType.DestroyCard, TriggerTiming.After);
            //Destroy this card when {H} hero cards are destroyed this way in one round.
            base.AddTrigger<DestroyCardAction>(SelfDestructionCriteria, base.DestroyThisCardResponse, TriggerType.DestroySelf, TriggerTiming.After);
        }

        private IEnumerator DestroyHeroCardResponse(DealDamageAction action)
        {
            IEnumerator coroutine = base.GameController.SelectAndDestroyCard(base.FindHeroTurnTakerController(action.DamageSource.Card.Owner.ToHero()), new LinqCardCriteria((Card c) => (IsOngoing(c) || IsEquipment(c)) && c.Owner == action.DamageSource.Card.Owner), false, cardSource: base.GetCardSource());
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

        private IEnumerable<DestroyCardJournalEntry> GetHeroCardsDestroyedThisRound()
        {
            return (from e in base.Journal.DestroyCardEntriesThisRound()
                    where IsHero(e.Card) && (IsOngoing(e.Card) || IsEquipment(e.Card))
                    select e).Where(base.Journal.SinceCardWasMoved<DestroyCardJournalEntry>(base.Card, (MoveCardJournalEntry e) => e.ToLocation == base.TurnTaker.PlayArea)); ;
        }
    }
}