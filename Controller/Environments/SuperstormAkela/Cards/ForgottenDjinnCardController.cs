using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.SuperstormAkela
{
    public class ForgottenDjinnCardController : SuperstormAkelaCardController
    {

        public ForgottenDjinnCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //At the end of the environment turn, this card deals the non-environment target with the second highest HP X+2 melee damage, where X is the number of environment cards to the left of this one.
            AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, DealDamageResponse, TriggerType.DealDamage);
        }

        private IEnumerator DealDamageResponse(PhaseChangeAction pca)
        {
            // this card deals the non-environment target with the second highest HP X+2 melee damage, where X is the number of environment cards to the left of this one.

            Func<Card, int?> amount = (Card c) => GetNumberOfCardsToTheLeftOfThisOne(base.Card) + 2;
            IEnumerator coroutine = DealDamageToHighestHP(base.Card, 2, (Card c) => c.IsNonEnvironmentTarget, amount, DamageType.Melee);
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