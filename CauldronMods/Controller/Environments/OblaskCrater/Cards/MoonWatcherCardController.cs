using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.OblaskCrater
{
    public class MoonWatcherCardController : OblaskCraterUtilityCardController
    {
        /*
         * At the end of the environment turn, this card deals each non-environment target 1 sonic damage.
         * When this card is destroyed, if it has 0 or fewer HP, reveal the top card of each deck and 
         * replace or discard each one.
         */
        public MoonWatcherCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            base.AddEndOfTurnTrigger((tt) => tt == base.TurnTaker, PhaseChangeActionResponse, TriggerType.DealDamage);
            base.AddWhenDestroyedTrigger(DestroyCardActionResponse, TriggerType.RevealCard);
        }

        private IEnumerator PhaseChangeActionResponse(PhaseChangeAction phaseChangeAction)
        {
            // This card deals each non-environment target 1 sonic damage.
            IEnumerator coroutine;

            coroutine = base.DealDamage(base.Card, (card) => !card.IsEnvironment, 1, DamageType.Sonic);
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

        private IEnumerator DestroyCardActionResponse(DestroyCardAction destroyCardAction)
        {
            // When this card is destroyed, if it has 0 or fewer HP, reveal the top card of each deck and
            // replace or discard each one.
            IEnumerator coroutine;

            if (base.Card.HitPoints <= 0)
            {
                coroutine = base.GameController.SelectTurnTakersAndDoAction(base.HeroTurnTakerController, new LinqTurnTakerCriteria((TurnTaker tt) => true), SelectionType.RevealTopCardOfDeck, (TurnTaker tt) => base.RevealCard_DiscardItOrPutItOnDeck(base.HeroTurnTakerController, base.FindTurnTakerController(tt), tt.Deck, false), allowAutoDecide: true, cardSource: base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }

            yield break;
        }
    }
}
