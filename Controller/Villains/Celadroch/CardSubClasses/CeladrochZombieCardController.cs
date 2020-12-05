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
        private readonly DamageType _damageType;

        protected CeladrochZombieCardController(Card card, TurnTakerController turnTakerController, DamageType damageType) : base(card, turnTakerController)
        {
            _damageType = damageType;
        }

        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            return base.DeterminePlayLocation(storedResults, isPutIntoPlay, decisionSources, overridePlayArea, additionalTurnTakerCriteria);
        }

        protected abstract void CannotTakeActionFunction(Func<TurnTakerController, bool> criteria);

        public override void AddTriggers()
        {
            CannotTakeActionFunction(ttc => ttc.TurnTaker == GetCardThisCardIsNextTo().Owner);

            AddDealDamageAtEndOfTurnTrigger(TurnTaker, Card, c => c == GetCardThisCardIsNextTo(), TargetType.SelectTarget, H, _damageType);
        }

    }
}