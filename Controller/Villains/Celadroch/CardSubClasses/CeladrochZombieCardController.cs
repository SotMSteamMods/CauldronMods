using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Cauldron.Celadroch
{
    public abstract class CeladrochZombieCardController : CardController
    {
        /*
         * 	"Play this card next to {SelectZombieTarget}. That hero cannot {CannotTakeActionFunction}.",
			"At the end of the villain turn, this card deals the hero next to it {H} {_damageType} damage."
         */

        private readonly DamageType _damageType;

        protected CeladrochZombieCardController(Card card, TurnTakerController turnTakerController, DamageType damageType) : base(card, turnTakerController)
        {
            _damageType = damageType;
        }

        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            var coroutine = SelectZombieTarget(storedResults, decisionSources, isPutIntoPlay, additionalTurnTakerCriteria);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        protected abstract IEnumerator SelectZombieTarget(List<MoveCardDestination> storedResults, List<IDecision> decisionSources, bool isPutIntoPlay, LinqTurnTakerCriteria additionalCriteria = null);

        protected abstract void CannotTakeActionFunction(Func<TurnTakerController, bool> criteria);

        public override void AddTriggers()
        {
            CannotTakeActionFunction(ttc => (Card.Location.IsNextToCard && ttc.TurnTaker == GetCardThisCardIsNextTo().Owner) || (Card.Location.IsPlayArea && ttc.TurnTaker == Card.Location.OwnerTurnTaker));

            AddDealDamageAtEndOfTurnTrigger(TurnTaker, Card, c => !c.IsIncapacitatedOrOutOfGame && c == GetCardThisCardIsNextTo(), TargetType.SelectTarget, H, _damageType);

            //This just covers weird cases where the character card totally leaves play (Final Wasteland perhaps).
            AddIfTheCardThatThisCardIsNextToLeavesPlayMoveItToTheirPlayAreaTrigger(false, false);
        }
    }
}