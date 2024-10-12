using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Terminus
{
    public class ReturnFireCardController : TerminusBaseCardController
    {
        /*
         * Select a non-hero target. That target deals {Terminus} 1 projectile damage, then {Terminus} deals it 2 projectile damage. 
         * You may draw a card. 
         * You may play a card.
         */
        public ReturnFireCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;
            SelectCardDecision selectCardDecision;
            IEnumerable<Card> cardChoices = base.GameController.FindTargetsInPlay((card) => !IsHeroTarget(card));

            // Select a non-hero target.
            selectCardDecision = new SelectCardDecision(base.GameController, DecisionMaker, SelectionType.DealDamage, cardChoices, cardSource: base.GetCardSource());
            coroutine = base.GameController.SelectCardAndDoAction(selectCardDecision, ActionWithCardResponse);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            // You may draw a card. 
            coroutine = base.GameController.DrawCard(base.HeroTurnTaker, true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            // You may play a card.
            coroutine = base.SelectAndPlayCardFromHand(DecisionMaker, true);
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

        private IEnumerator ActionWithCardResponse(SelectCardDecision targetCard)
        {
            IEnumerator coroutine;

            //That target deals {Terminus} 1 projectile damage,
            coroutine = base.DealDamage(targetCard.SelectedCard, base.CharacterCard, 1, DamageType.Projectile, cardSource: base.GetCardSource()); 
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            // then {Terminus} deals it 2 projectile damage. 
            coroutine = base.DealDamage(base.CharacterCard, targetCard.SelectedCard, 2, DamageType.Projectile, cardSource: base.GetCardSource());
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
