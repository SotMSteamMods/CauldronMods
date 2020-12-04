using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.SuperstormAkela
{
    public class SkulkingIntermediaryCardController : SuperstormAkelaCardController
    {

        public SkulkingIntermediaryCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //At the end of the environment turn, each environment and villain target regains X+1 HP, where X is the number of environment cards to the left of this one.
            AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, HealNonHeroTargetsResponse, TriggerType.GainHP);

            //This card is immune to damage dealt by villain targets.
            AddImmuneToDamageTrigger((DealDamageAction dd) => dd.DamageSource != null && dd.DamageSource.Card.IsVillainTarget);
        }

        private IEnumerator HealNonHeroTargetsResponse(PhaseChangeAction arg)
        {
            //Each environment and villain target regains X+1 HP, where X is the number of environment cards to the left of this one.
            Func<Card, int> X = (Card c) => GetNumberOfCardsToTheLeftOfThisOne(base.Card).Value + 1;
            IEnumerator coroutine = GameController.GainHP(DecisionMaker, (Card c) => base.IsVillainTarget(c) || c.IsEnvironmentTarget, X, cardSource: GetCardSource());
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