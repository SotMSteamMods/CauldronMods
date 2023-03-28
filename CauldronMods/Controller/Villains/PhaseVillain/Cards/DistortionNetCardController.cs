using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.PhaseVillain
{
    public class DistortionNetCardController : PhaseVillainCardController
    {
        public DistortionNetCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> destination, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            List<Card> storedResults = new List<Card>();
            //When this card enters play, place it next to the hero with the highest HP.
            IEnumerator coroutine = base.GameController.FindTargetWithHighestHitPoints(1, (Card c) =>  IsHeroCharacterCard(c), storedResults, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            Card card = storedResults.FirstOrDefault<Card>();
            if (card != null && destination != null)
            {
                destination.Add(new MoveCardDestination(card.NextToLocation, false, false, false));
            }
            yield break;
        }

        public override void AddTriggers()
        {
            //Reduce damage dealt by that hero by 2.
            base.AddReduceDamageTrigger((DealDamageAction action) => action.DamageSource != null && action.DamageSource.Card != null && action.DamageSource.Card == base.GetCardThisCardIsNextTo(), (DealDamageAction action) => 2);
            //At the start of that hero's turn, this card deals them {H} toxic damage.
            base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.GetCardThisCardIsNextTo().Owner, this.DealDamageResponse, TriggerType.DealDamage);
        }

        private IEnumerator DealDamageResponse(PhaseChangeAction action)
        {
            //...this card deals them {H} toxic damage.
            IEnumerator coroutine = base.DealDamage(base.Card, base.GetCardThisCardIsNextTo(), Game.H, DamageType.Toxic, cardSource: base.GetCardSource());
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