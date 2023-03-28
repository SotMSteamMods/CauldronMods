using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Celadroch
{
    public class LeechingBreathCardController : CeladrochZombieCardController
    {
        /*
         *  "Play this card next to the hero with the highest HP. That hero cannot use powers.",
			"At the end of the villain turn, this card deals the hero next to it {H} toxic damage."
         */

        public LeechingBreathCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, DamageType.Toxic)
        {

        }

        protected override IEnumerator SelectZombieTarget(List<MoveCardDestination> storedResults, List<IDecision> decisionSources, bool isPutIntoPlay, LinqTurnTakerCriteria additionalCriteria = null)
        {
            List<Card> storeHighest = new List<Card>();
            var coroutine = GameController.FindTargetWithHighestHitPoints(1, c =>  IsHeroCharacterCard(c) && (additionalCriteria?.Criteria(c.Owner) ?? true), storeHighest, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if (storeHighest.Any())
            {
                storedResults.Add(new MoveCardDestination(storeHighest.First().NextToLocation));
            }
        }

        protected override void CannotTakeActionFunction(Func<TurnTakerController, bool> criteria)
        {
            CannotUsePowers(criteria);
        }
    }
}