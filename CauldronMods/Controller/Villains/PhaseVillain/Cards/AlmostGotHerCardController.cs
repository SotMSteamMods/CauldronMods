using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.PhaseVillain
{
    public class AlmostGotHerCardController : PhaseVillainCardController
    {
        public AlmostGotHerCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            var ss = base.SpecialStringMaker.ShowIfElseSpecialString(() => base.HasBeenSetToTrueThisRound(FirstTimeDealDamage),
                () => $"{CharacterCard.Title} has dealt damage to a target this round.",
                () => $"{CharacterCard.Title} has not dealt damage to a target this round.");
            ss.Condition = () => Card.IsInPlay;
        }

        private const string FirstTimeDealDamage = "FirstTimeDealDamage";

        public override void AddTriggers()
        {
            //Increase damage dealt to Obstacles by 1 by the first hero target damaged by {Phase} each round.
            base.AddTrigger<DealDamageAction>((DealDamageAction action) => !base.HasBeenSetToTrueThisRound(FirstTimeDealDamage) && action.DamageSource != null && action.DamageSource.Card != null && action.DamageSource.Card == base.CharacterCard && IsHero(action.Target), this.FirstTimeDealDamageResponse, TriggerType.IncreaseDamage, TriggerTiming.After);
            //Damage dealt by {Phase} is irreducible.
            base.AddMakeDamageIrreducibleTrigger((DealDamageAction action) => action.DamageSource != null && action.DamageSource.Card != null && action.DamageSource.Card == base.CharacterCard);
            //At the start of the villain turn, if there are 1 or 0 Obstacles in play, each player must discard a card. Then, this card is destroyed.
            base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker && base.FindCardsWhere(new LinqCardCriteria((Card c) => base.IsObstacle(c) && c.IsInPlayAndHasGameText)).Count() <= 1, this.DiscardCardsAndDestroyThisCardResponse, TriggerType.DestroySelf);

            base.AddAfterLeavesPlayAction(() => ResetFlagAfterLeavesPlay(FirstTimeDealDamage));
        }

        private IEnumerator FirstTimeDealDamageResponse(DealDamageAction action)
        {
            base.SetCardPropertyToTrueIfRealAction(FirstTimeDealDamage);
            //Increase damage dealt to Obstacles by 1 by the first hero target damaged by {Phase} each round.
            //...by 1...
            IncreaseDamageStatusEffect statusEffect = new IncreaseDamageStatusEffect(1);
            //...by the first hero target damaged by {Phase}...
            statusEffect.SourceCriteria.IsSpecificCard = action.Target;
            //...dealt to Obstacles...
            statusEffect.TargetCriteria.HasAnyOfTheseKeywords = new List<string>() { "obstacle" };
            //...each round.
            statusEffect.UntilStartOfNextTurn(base.TurnTaker);
            IEnumerator coroutine = base.AddStatusEffect(statusEffect);
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

        private IEnumerator DiscardCardsAndDestroyThisCardResponse(PhaseChangeAction action)
        {
            //...each player must discard a card. 
            IEnumerator coroutine = base.GameController.EachPlayerDiscardsCards(1, 1, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //Then, this card is destroyed.
            coroutine = base.GameController.DestroyCard(this.DecisionMaker, base.Card, cardSource: base.GetCardSource());
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
    }
}