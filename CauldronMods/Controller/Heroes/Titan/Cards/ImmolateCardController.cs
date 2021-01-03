using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.Titan
{
    public class ImmolateCardController : CardController
    {
        public ImmolateCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            //base.SpecialStringMaker.ShowCardThisCardIsNextTo(base.GetCardThisCardIsNextTo());
        }

        private const string FirstTimeDealingDamage = "FirstTimeDealingDamage";
        private Location NextToHeroToGoTo = null;

        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            //if we're given a hero to go to, go there
            if (NextToHeroToGoTo != null)
            {
                storedResults.Add(new MoveCardDestination(NextToHeroToGoTo));
                NextToHeroToGoTo = null;
                yield break;
            }

            //Play this card next to a target.
            IEnumerator coroutine = base.SelectCardThisCardWillMoveNextTo(new LinqCardCriteria((Card c) => c.IsTarget, "target"), storedResults, true, decisionSources);
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
            //The first time that target deals damage each turn, it deals itself 1 fire damage.
            base.AddTrigger<DealDamageAction>((DealDamageAction action) => action.DamageSource.Card == base.GetCardThisCardIsNextTo() && !base.IsPropertyTrue(FirstTimeDealingDamage, null), this.DealDamageResponse, TriggerType.DealDamage, TriggerTiming.After);

            //If that target leaves play, destroy this card.
            base.AddIfTheTargetThatThisCardIsNextToLeavesPlayDestroyThisCardTrigger();

            base.AddAfterLeavesPlayAction((GameAction ga) => base.ResetFlagAfterLeavesPlay(FirstTimeDealingDamage), TriggerType.Hidden);
        }

        private IEnumerator DealDamageResponse(DealDamageAction action)
        {
            base.SetCardPropertyToTrueIfRealAction(FirstTimeDealingDamage);
            IEnumerator coroutine = base.DealDamage(base.GetCardThisCardIsNextTo(), base.GetCardThisCardIsNextTo(), 1, DamageType.Fire, cardSource: base.GetCardSource());
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

        public IEnumerator PlayBySpecifiedTarget(Card target, bool isPutIntoPlay, CardSource cardSource)
        {
            if (target != null && !target.IsIncapacitatedOrOutOfGame)
            {
                NextToHeroToGoTo = target.NextToLocation;
            }

            IEnumerator play = GameController.PlayCard(TurnTakerController, this.Card, wasCardPlayed: new List<bool> { !isPutIntoPlay }, isPutIntoPlay: isPutIntoPlay, cardSource: cardSource);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(play);
            }
            else
            {
                GameController.ExhaustCoroutine(play);
            }
            yield break;
        }
    }
}