using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Northspar
{
    public class FrozenSolidCardController : NorthsparCardController
    {

        public FrozenSolidCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroCharacterCardWithLowestHP();
        }

        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            //When this card enters play, place it next to the hero with the lowest HP. 
            IEnumerator coroutine = base.SelectCardThisCardWillMoveNextTo(new LinqCardCriteria((Card c) => base.CanCardBeConsideredLowestHitPoints(c, (Card card) => IsHeroCharacterCard(card) && !card.IsIncapacitatedOrOutOfGame && GameController.IsCardVisibleToCardSource(card, GetCardSource())) && (additionalTurnTakerCriteria == null || additionalTurnTakerCriteria.Criteria(c.Owner)), "hero with the lowest hp"), storedResults, isPutIntoPlay, decisionSources);

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
            //That hero must skip either their play or power phase during their turn.
            AddTrigger((PhaseChangeAction p) => p.ToPhase.IsUsePower && DidHeroPlayCardDuringCardPhaseThisTurn(p.ToPhase.TurnTaker), SkipPhaseResponse, TriggerType.SkipPhase, TriggerTiming.After);
            AddTrigger((PhaseChangeAction p) => p.ToPhase.IsPlayCard && DidHeroUsePowerDuringPowerPhaseThisTurn(p.ToPhase.TurnTaker), SkipPhaseResponse, TriggerType.SkipPhase, TriggerTiming.After);

            // Increase cold damage dealt to that hero by 1
            AddIncreaseDamageTrigger((DealDamageAction dd) => dd.DamageType == DamageType.Cold && dd.Target == base.GetCardThisCardIsNextTo(), (DealDamageAction dd) => 1);

            // If that hero is dealt fire damaage, destroy this card
            AddTrigger<DealDamageAction>((DealDamageAction dd) => dd.DamageType == DamageType.Fire && dd.Target == GetCardThisCardIsNextTo() && dd.DidDealDamage, DestroyThisCardResponse, TriggerType.DestroySelf, TriggerTiming.After);
        }

        private IEnumerator SkipPhaseResponse(PhaseChangeAction p)
        {
            IEnumerator coroutine = base.GameController.PreventPhaseAction(p.ToPhase, showMessage: false, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        private bool DidHeroPlayCardDuringCardPhaseThisTurn(TurnTaker tt)
        {
            if (tt == base.GetCardThisCardIsNextTo().Owner)
            {
                return base.Journal.PlayCardEntriesThisTurn().Any((PlayCardJournalEntry p) => p.CardPlayed.Owner == tt && p.TurnPhase.TurnTaker == tt && p.TurnPhase.IsPlayCard);
            }
            return false;
        }

        private bool DidHeroUsePowerDuringPowerPhaseThisTurn(TurnTaker tt)
        {
            if (tt == base.GetCardThisCardIsNextTo().Owner)
            {
                return base.Journal.UsePowerEntriesThisTurn().Any((UsePowerJournalEntry p) => p.PowerUser == tt && p.TurnPhase.TurnTaker == tt && p.TurnPhase.IsUsePower);
            }
            return false;
        }

    }
}