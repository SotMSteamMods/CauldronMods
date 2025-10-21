using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Mythos
{
    public class RitualSiteCardController : MythosUtilityCardController
    {
        public RitualSiteCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //{MythosDanger} Increase damage dealt by environment cards to hero targets by 1.
            base.AddIncreaseDamageTrigger((DealDamageAction action) => base.IsTopCardMatching(MythosDangerDeckIdentifier) && action.DamageSource.IsEnvironmentCard && IsHeroTarget(action.Target), 1);

            //{MythosMadness} At the end of the villain turn, this card deals each hero target 1 psychic damage.
            base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.MadnessDealDamageResponse, TriggerType.DealDamage, (PhaseChangeAction action) => base.IsTopCardMatching(MythosMadnessDeckIdentifier));
        }

        private IEnumerator MadnessDealDamageResponse(PhaseChangeAction action)
        {
            //...this card deals each hero target 1 psychic damage.
            IEnumerator coroutine = base.DealDamage(this.Card, (Card c) => IsHeroTarget(c), 1, DamageType.Psychic);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }
    }
}
