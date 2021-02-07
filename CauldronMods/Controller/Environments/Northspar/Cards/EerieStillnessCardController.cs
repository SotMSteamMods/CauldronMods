using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.Northspar
{
    public class EerieStillnessCardController : NorthsparCardController
    {

        public EerieStillnessCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowListOfCardsAtLocation(TurnTaker.Deck, new LinqCardCriteria(c => IsWaypoint(c), "waypoint"));
            SpecialStringMaker.ShowListOfCardsAtLocation(TurnTaker.Trash, new LinqCardCriteria(c => IsWaypoint(c), "waypoint"));
        }


        public override void AddTriggers()
        {
            //All targets are immune to cold damage.
            base.AddImmuneToDamageTrigger((DealDamageAction dd) => dd.DamageType == DamageType.Cold);

            //At the start of the environment turn, search the environment deck and trash for a First, Second, or Third Waypoint card and put it into play, then shuffle the deck and destroy this card.
            base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.SearchForWaypoints, TriggerType.PutIntoPlay);
        }

        private IEnumerator SearchForWaypoints(PhaseChangeAction pca)
        {
            IEnumerator coroutine = base.SearchForWaypoints();
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            // ...and destroy this card.
            coroutine = base.DestroyThisCardResponse(pca);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }
    }
}