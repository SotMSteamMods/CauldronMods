using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Menagerie
{
    public class PrizedCatchCardController : MenagerieCardController
    {
        public PrizedCatchCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
        }

        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> destination, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            //Play this card next to the hero with the most one-shots in hand. That hero and all their cards are Captured.
            List<TurnTaker> storedResults = new List<TurnTaker>();
            IEnumerator coroutine = base.FindHeroWithMostCardsInHand(storedResults, cardCriteria: new LinqCardCriteria((Card c) => c.IsOneShot, "one-shot"));
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            TurnTaker turnTaker = storedResults.FirstOrDefault<TurnTaker>();
            if (turnTaker != null && destination != null)
            {
                destination.Add(new MoveCardDestination(turnTaker.CharacterCard.NextToLocation, false, true, false));
            }
            yield break;
        }

        public override void AddTriggers()
        {
            //Increase damage dealt by Captured targets to Enclosures by 1. 
            base.AddIncreaseDamageTrigger((DealDamageAction action) => action.DamageSource != null && action.DamageSource.Card != null && base.IsCaptured(action.DamageSource.Card.Owner) && base.IsEnclosure(action.Target), 1);
            //Reduce damage dealt by Captured targets to non-Enclosure targets by 1.
            base.AddReduceDamageTrigger((DealDamageAction action) => action.DamageSource != null && action.DamageSource.Card != null && base.IsCaptured(action.DamageSource.Card.Owner) && !base.IsEnclosure(action.Target), (DealDamageAction action) => 1);
            //Front: The heroes lose if the captured hero is incapacitated.
            base.AddTrigger<FlipCardAction>((FlipCardAction action) => !base.CharacterCard.IsFlipped && base.FindCardsWhere((Card c) =>  IsHeroCharacterCard(c) && !c.IsIncapacitatedOrOutOfGame).Count() > 0 && !base.IsTurnTakerActiveInThisGame(base.Card.Location.OwnerName) && action.CardToFlip.TurnTaker == base.Card.Location.OwnerTurnTaker && action.CardToFlip.Card.IsCharacter && action.CardToFlip.TurnTaker.CharacterCards.Where((Card c) => !c.IsFlipped).Count() == 0, this.LoseTheGameResponse, new TriggerType[] { TriggerType.GameOver }, TriggerTiming.After);
            base.AddTriggers();

        }

        private IEnumerator LoseTheGameResponse(FlipCardAction action)
        {
            IEnumerator coroutine = base.GameController.GameOver(EndingResult.AlternateDefeat, "the captured hero was incapacitated", true, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
                yield break;
            }
        }

        public override bool AskIfCardIsIndestructible(Card card)
        {
            //Front: Prized Catch is Indestructible.
            return !base.CharacterCard.IsFlipped && card == base.Card;
        }
    }
}