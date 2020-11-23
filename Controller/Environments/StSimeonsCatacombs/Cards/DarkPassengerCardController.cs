using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.StSimeonsCatacombs
{
    public class DarkPassengerCardController : GhostCardController
    {
        #region Constructors

        public DarkPassengerCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, new string[] { "CursedVault" })
        {

        }

        #endregion Constructors

        #region Methods

        public override void AddTriggers()
        {
            //Reduce damage dealt by that hero by 1.
            base.AddReduceDamageTrigger((DealDamageAction dd) => base.GetCardThisCardIsNextTo() != null && dd.DamageSource.IsSameCard(base.GetCardThisCardIsNextTo()), new int?(1), null);

            //At the end of the environment turn, this card deals that hero 2 melee damage
            IEnumerator dealDamage = base.DealDamage(base.Card, base.GetCardThisCardIsNextTo(), 2, DamageType.Melee, cardSource: base.GetCardSource());
            base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, (PhaseChangeAction pca) => dealDamage, TriggerType.DealDamage);

            //add unaffected triggers from GhostCardControllers
            base.AddTriggers();
        }

        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            IEnumerable<Card> secondHighestHP = FindCardsWhere(new LinqCardCriteria((Card c) => base.CanCardBeConsideredHighestHitPoints(c, (Card card) => card.IsHero && card.IsTarget, 2)));
            Card secondHighest;
            IEnumerator coroutine = null;
            if (secondHighestHP.Count() > 1)
            {
                List<SelectCardsDecision> storedSecondHero = new List<SelectCardsDecision>();
                coroutine = base.GameController.SelectCardsAndStoreResults(this.DecisionMaker, SelectionType.LowestHP, (Card c) => secondHighestHP.Contains(c), 1, storedSecondHero, false, cardSource: base.GetCardSource());
                secondHighest = storedSecondHero.FirstOrDefault().SelectedCard;
            }
            else
            {
                secondHighest = secondHighestHP.FirstOrDefault();
            }
            //Play this card next to the hero with the second highest HP.
            coroutine = base.SelectCardThisCardWillMoveNextTo(new LinqCardCriteria((Card c) => c == secondHighest && (additionalTurnTakerCriteria == null || additionalTurnTakerCriteria.Criteria(c.Owner)), "hero target with the lowest HP"), storedResults, isPutIntoPlay, decisionSources);
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

        #endregion Methods
    }
}