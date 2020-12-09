using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Phase
{
    public class AlmostGotHerCardController : PhaseCardController
    {
        public AlmostGotHerCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowIfElseSpecialString(() => base.HasBeenSetToTrueThisRound("FirstTimeDamageDeal"), () => "Phase has dealt damage to a target this round.", () => "Phase has not dealt damage to a target this round.");
        }

        private const string FirstTimeDamageDealt = "FirstTimeDamageDeal";

        public override void AddTriggers()
        {
            //Increase damage dealt to Obstacles by 1 by the first hero target damaged by {Phase} each round.
            base.AddTrigger<DealDamageAction>((DealDamageAction action) => !base.HasBeenSetToTrueThisRound("FirstTimeDamageDeal") && action.DamageSource.Card == base.CharacterCard && action.Target.IsHero, this.FirstTimeDealDamageResponse, TriggerType.IncreaseDamage, TriggerTiming.After);
            //Damage dealt by {Phase} is irreducible.
            base.AddMakeDamageIrreducibleTrigger((DealDamageAction action) => action.DamageSource.Card == base.CharacterCard);
            //At the start of the villain turn, if there are 1 or 0 Obstacles in play, each player must discard a card. Then, this card is destroyed.
            base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker && base.FindCardsWhere(new LinqCardCriteria((Card c) => base.IsObstacle(c))).Count() > 1, this.DiscardCardsAndDestroyThisCardResponse, TriggerType.DestroySelf);
        }

        private IEnumerator FirstTimeDealDamageResponse(DealDamageAction action)
        {
            //Increase damage dealt to Obstacles by 1 by the first hero target damaged by {Phase} each round.
            //...by 1...
            IncreaseDamageStatusEffect statusEffect = new IncreaseDamageStatusEffect(1);
            //...by the first hero target damaged by {Phase}...
            statusEffect.SourceCriteria.IsSpecificCard = action.Target;
            //...dealt to Obstacles...
            statusEffect.TargetCriteria.HasAnyOfTheseKeywords = new List<string>() { "Obstacle" };
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