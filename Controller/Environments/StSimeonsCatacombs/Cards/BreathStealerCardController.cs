using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.StSimeonsCatacombs
{
    public class BreathStealerCardController : CardController
    {
        #region Constructors

        public BreathStealerCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            //TODO: Add a conditional special string that says something like "Aqueduct is in play so it is affected by Hero cards" and the reverse
            base.AddThisCardControllerToList(CardControllerListType.ChangesVisibility);
        }

        #endregion Constructors

        #region Methods
        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            //Play this card next to the hero with the lowest HP
            IEnumerator coroutine = base.SelectCardThisCardWillMoveNextTo(new LinqCardCriteria((Card c) => base.CanCardBeConsideredLowestHitPoints(c, (Card card) => card.IsHero && card.IsTarget) && (additionalTurnTakerCriteria == null || additionalTurnTakerCriteria.Criteria(c.Owner)), "hero target with the lowest HP"), storedResults, isPutIntoPlay, decisionSources);
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

        public override IEnumerator Play()
        {
            if (base.Card.Location.IsNextToCard)
            {
                //That hero cannot regain HP.
                CannotGainHPStatusEffect cannotGainHPStatusEffect = new CannotGainHPStatusEffect();
                cannotGainHPStatusEffect.TargetCriteria.IsSpecificCard = base.GetCardThisCardIsNextTo();
                cannotGainHPStatusEffect.UntilTargetLeavesPlay(base.Card);
                IEnumerator coroutine = base.AddStatusEffect(cannotGainHPStatusEffect);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            yield break;
        }

        public override void AddTriggers()
        {
            //At the end of the environment turn, this card deals that hero 1 toxic damage
            IEnumerator dealDamage = base.DealDamage(base.Card, base.GetCardThisCardIsNextTo(), 1, DamageType.Toxic, cardSource: base.GetCardSource());
            base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, (PhaseChangeAction pca) => dealDamage, TriggerType.DealDamage);

            //This card may not be affected by hero cards unless Aqueducts is in play.
            base.AddTrigger<MakeDecisionsAction>((MakeDecisionsAction md) => !this.IsAqueductsInPlay() && md.CardSource != null && md.CardSource.Card.Owner.IsHero, new Func<MakeDecisionsAction, IEnumerator>(this.RemoveDecisionsFromMakeDecisionsResponse), TriggerType.RemoveDecision, TriggerTiming.Before);

        }

        private IEnumerator RemoveDecisionsFromMakeDecisionsResponse(MakeDecisionsAction md)
        {
            //remove this card as an option to make decisions
            md.RemoveDecisions((IDecision d) =>d.SelectedCard == base.Card);
            yield return base.DoNothing();
            yield break;
        }

        public override bool? AskIfCardIsVisibleToCardSource(Card card, CardSource cardSource)
        {
            //check if card is from a hero and if aqueducts is in play
            if (!this.IsAqueductsInPlay() && card == base.Card)
            {
                return new bool?(false);
            }
            return new bool?(true);
        }


        public override bool AskIfActionCanBePerformed(GameAction g)
        {
            //if aqueducts is not in play, actions from hero cards cannot affect this card
            if (!this.IsAqueductsInPlay())
            {
                bool? flag = g.DoesFirstCardAffectSecondCard((Card c) => c.IsHero, (Card c) => c == base.Card);

                if (flag != null && flag.Value)
                {
                    return false;
                }
            }
            return true;
        }

        private bool IsAqueductsInPlay()
        {
            return base.FindCardsWhere(c => c.IsInPlayAndHasGameText && c.Identifier == "Aqueducts").Count() > 0;
        }

        #endregion Methods
    }
}