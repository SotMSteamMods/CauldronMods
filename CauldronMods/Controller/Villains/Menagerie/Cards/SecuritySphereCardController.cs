using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Linq;

namespace Cauldron.Menagerie
{
    public class SecuritySphereCardController : EnclosureCardController
    {
        public SecuritySphereCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowListOfCards(new LinqCardCriteria((Card c) => base.IsCaptured(c.Owner), "captured"));
        }

        public override IEnumerator Play()
        {
            //When this card enters play, place the top card of the villain deck beneath it face down...
            IEnumerator coroutine = base.EncloseTopCardResponse(TurnTaker.Deck);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //...and destroy {H - 2} hero ongoing cards.
            coroutine = base.GameController.SelectAndDestroyCards(this.DecisionMaker, new LinqCardCriteria((Card c) => c.IsHero && (c.IsOngoing || base.IsEquipment(c)), "hero ongoing or equipment"), Game.H - 2, cardSource: base.GetCardSource());
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

        public override void AddTriggers()
        {
            base.AddTriggers();
            //The Captured hero and their cards cannot affect or be affected by cards or effects from other hero decks
            base.AddTrigger<MakeDecisionsAction>((MakeDecisionsAction md) => md.CardSource != null && md.CardSource.Card.Owner.IsHero, new Func<MakeDecisionsAction, IEnumerator>(this.RemoveDecisionsFromMakeDecisionsResponse), TriggerType.RemoveDecision, TriggerTiming.Before);
        }

        public override bool AskIfActionCanBePerformed(GameAction g)
        {
            //The Captured hero and their cards cannot affect or be affected by cards or effects from other hero decks
            if (this.GetCapturedHero() != null)
            {
                bool? flag = g.DoesFirstCardAffectSecondCard((Card c) => c.Owner == this.GetCapturedHero(), (Card c) => c.Owner != this.GetCapturedHero() && c.Owner.IsHero);
                bool? flag2 = g.DoesFirstCardAffectSecondCard((Card c) => c.Owner != this.GetCapturedHero() && c.Owner.IsHero, (Card c) => c.Owner == this.GetCapturedHero());
                bool? flag3 = g.DoesFirstTurnTakerAffectSecondTurnTaker((TurnTaker tt) => tt == this.GetCapturedHero(), (TurnTaker tt) => tt != this.GetCapturedHero() && tt.IsHero);
                bool? flag4 = g.DoesFirstTurnTakerAffectSecondTurnTaker((TurnTaker tt) => tt != this.GetCapturedHero() && tt.IsHero, (TurnTaker tt) => tt == this.GetCapturedHero());
                if ((flag != null && flag.Value) || (flag2 != null && flag2.Value) || (flag3 != null && flag3.Value) || (flag4 != null && flag4.Value))
                {
                    return false;
                }
            }
            return true;
        }

        private IEnumerator RemoveDecisionsFromMakeDecisionsResponse(MakeDecisionsAction md)
        {
            //The Captured hero and their cards cannot affect or be affected by cards or effects from other hero decks
            md.RemoveDecisions((IDecision d) => d.CardSource.Card.Owner != this.GetCapturedHero() && d.HeroTurnTakerController.TurnTaker == this.GetCapturedHero());
            md.RemoveDecisions((IDecision d) => d.CardSource.Card.Owner == this.GetCapturedHero() && d.HeroTurnTakerController.TurnTaker != this.GetCapturedHero());
            yield return base.DoNothing();
            yield break;
        }

        private TurnTaker GetCapturedHero()
        {
            Card prize = FindCardsWhere(new LinqCardCriteria((Card c) => base.FindCardController(c) is PrizedCatchCardController)).FirstOrDefault();
            if (prize.IsInPlayAndHasGameText && prize.Location.IsNextToCard)
            {
                return prize.Location.OwnerCard.Owner;
            }
            return null;
        }

        public override bool? AskIfCardIsVisibleToCardSource(Card card, CardSource cardSource)
        {
            //The Captured hero and their cards cannot affect or be affected by cards or effects from other hero decks
            return this.AskIfTurnTakerIsVisibleToCardSource(card.Owner, cardSource);
        }

        public override bool? AskIfTurnTakerIsVisibleToCardSource(TurnTaker tt, CardSource cardSource)
        {
            //The Captured hero and their cards cannot affect or be affected by cards or effects from other hero decks
            if (cardSource == null || !cardSource.Card.IsHero || !tt.IsHero)
            {
                return true;
            }
            if (cardSource.Card.Owner == this.GetCapturedHero())
            {
                return tt == this.GetCapturedHero();
            }
            return tt != this.GetCapturedHero();
        }
    }
}