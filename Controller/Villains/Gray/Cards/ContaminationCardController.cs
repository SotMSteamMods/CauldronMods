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
            this.GameActionCriteria = (DestroyCardAction action) => action.CardSource != null && action.CardSource.Card.ResponsibleTarget != null && (from e in this.GetHeroCardsDestroyedThisRound() where e.CardSource != null && e.ResponsibleCard == base.Card select e).Count<DestroyCardJournalEntry>() >= Game.H;
        }

        private Func<DestroyCardAction, bool> GameActionCriteria;

        public override void AddTriggers()
        {
            //Whenever a hero deals damage to a villain target, that hero must destroy 1 of their ongoing or equipment cards.
            base.AddTrigger<DealDamageAction>((DealDamageAction action) => action.DamageSource.IsHero && action.Target.IsVillain && action.IsSuccessful, new Func<DealDamageAction, IEnumerator>(this.DestroyHeroCardResponse), TriggerType.DestroyCard, TriggerTiming.After);
            //Destroy this card when {H} hero cards are destroyed this way in one round.
            base.AddTrigger<DestroyCardAction>(this.GameActionCriteria, base.DestroyThisCardResponse, TriggerType.DestroySelf, TriggerTiming.After);
        }

        private IEnumerator DestroyHeroCardResponse(DealDamageAction action)
        {
            IEnumerator coroutine = base.GameController.SelectAndDestroyCard(base.FindHeroTurnTakerController(action.DamageSource.TurnTaker.ToHero()), new LinqCardCriteria((Card c) => c.IsHero && (c.IsOngoing || IsEquipment(c))), false, cardSource: base.GetCardSource());
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
                    where e.Card.IsHero && (e.Card.IsOngoing || IsEquipment(e.Card))
                    select e).Where(base.Journal.SinceCardWasMoved<DestroyCardJournalEntry>(base.Card, (MoveCardJournalEntry e) => e.ToLocation == base.TurnTaker.Deck));
        }
    }
}