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
            List<Card> foundTarget = new List<Card>();
            IEnumerator coroutine = base.GameController.FindTargetWithHighestHitPoints(2, (Card c) => c.IsHero && (overridePlayArea == null || c.IsAtLocationRecursive(overridePlayArea)), foundTarget, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            Card secondHighest = foundTarget.FirstOrDefault<Card>();
            if (secondHighest != null && storedResults != null)
            {
                //Play this card next to the hero with the second highest HP.
                storedResults.Add(new MoveCardDestination(secondHighest.NextToLocation, false, false, false));
            }
            yield break;
        }

        #endregion Methods
    }
}