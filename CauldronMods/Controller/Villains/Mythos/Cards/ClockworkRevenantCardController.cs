using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Mythos
{
    public class ClockworkRevenantCardController : MythosUtilityCardController
    {
        public ClockworkRevenantCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        { 
        }
        protected override void ShowUniqueSpecialStrings()
        {
            base.SpecialStringMaker.ShowHeroTargetWithHighestHP();
            base.SpecialStringMaker.ShowSpecialString(() => "This card's damage is increased by " + (10 - this.Card.HitPoints));
        }

        public override void AddTriggers()
        {
            //{MythosDanger} Increase damage dealt by this card by X, where X is 10 minus its current HP.
            base.AddIncreaseDamageTrigger((DealDamageAction action) => base.IsTopCardMatching(MythosDangerDeckIdentifier) && action.DamageSource != null && action.DamageSource.Card != null && action.DamageSource.Card == this.Card, (DealDamageAction action) => 10 - this.Card.HitPoints ?? default);

            //At the end of the villain turn, this card deals the hero target with the highest HP 2 melee damage.
            base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.DealDamageResponse, TriggerType.DealDamage);
        }

        private IEnumerator DealDamageResponse(PhaseChangeAction action)
        {
            //...this card deals the hero target with the highest HP 2 melee damage.
            IEnumerator coroutine = base.DealDamageToHighestHP(this.Card, 1, (Card c) => IsHeroTarget(c) && c.IsInPlayAndHasGameText, (Card c) => 2, DamageType.Melee);
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
