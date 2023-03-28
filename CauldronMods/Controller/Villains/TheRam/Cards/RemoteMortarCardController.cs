using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace Cauldron.TheRam
{
    public class RemoteMortarCardController : TheRamUtilityCardController
    {
        public RemoteMortarCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            AddUpCloseTrackers();
        }

        public override void AddTriggers()
        {
            //"This card is immune to damage from heroes that are Up Close.",
            AddImmuneToDamageTrigger((DealDamageAction dd) => dd.Target == this.Card && dd.DamageSource != null && dd.DamageSource.Card != null && dd.DamageSource.IsHero && dd.DamageSource.IsCard && IsUpClose(dd.DamageSource.Card));

            //"At the end of the villain turn, this card deals each Up Close hero {H - 1} energy damage and those heroes must each discard a card."
            AddDealDamageAtEndOfTurnTrigger(TurnTaker, this.Card, (Card c) => IsHero(c) && IsUpClose(c), TargetType.All, H - 1, DamageType.Energy);
            AddEndOfTurnTrigger((TurnTaker tt) => tt == this.TurnTaker, HeroesDiscardResponse, TriggerType.DiscardCard);
        }

        private IEnumerator HeroesDiscardResponse(PhaseChangeAction pc)
        {
            SelectTurnTakersDecision orderDiscard = new SelectTurnTakersDecision(GameController, DecisionMaker, new LinqTurnTakerCriteria((TurnTaker tt) => IsHero(tt) && !tt.IsIncapacitatedOrOutOfGame && IsUpClose(tt), "Up Close hero"), SelectionType.DiscardCard, allowAutoDecide: true, cardSource: GetCardSource());
            IEnumerator coroutine = GameController.SelectTurnTakersAndDoAction(orderDiscard,
                                                        (TurnTaker tt) => GameController.SelectAndDiscardCard(FindHeroTurnTakerController(tt.ToHero()), cardSource: GetCardSource()),
                                                        cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }
    }
}