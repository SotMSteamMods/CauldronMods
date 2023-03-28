using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Cauldron.Celadroch
{
    public class WhisperingBreathCardController : CeladrochZombieCardController
    {
        /*
         * 	"Play this card next to the hero with the most cards in play. That hero cannot play cards.",
			"At the end of the villain turn, this card deals the hero next to it {H} sonic damage."
         */

        public WhisperingBreathCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, DamageType.Sonic)
        {

        }

        protected override IEnumerator SelectZombieTarget(List<MoveCardDestination> storedResults, List<IDecision> decisionSources, bool isPutIntoPlay, LinqTurnTakerCriteria additionalCriteria = null)
        {
            List<TurnTaker> turnTakers = new List<TurnTaker>();
            var coroutine = FindHeroWithMostCardsInPlay(turnTakers, 1, 1, additionalCriteria);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            coroutine = SelectCardThisCardWillMoveNextTo(new LinqCardCriteria(c =>  IsHeroCharacterCard(c) && turnTakers.Contains(c.Owner), "hero character"), storedResults, isPutIntoPlay, decisionSources);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        protected override void CannotTakeActionFunction(Func<TurnTakerController, bool> criteria)
        {
            CannotPlayCards(criteria);
        }
    }
}