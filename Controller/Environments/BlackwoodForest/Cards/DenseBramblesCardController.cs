using System;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.BlackwoodForest
{
    public class DenseBramblesCardController : CardController
    {
        //==============================================================
        // The {H - 1} targets with the lowest HP are immune to damage.
        // At the start of the environment turn destroy this card.
        //==============================================================


        public static string Identifier = "DenseBrambles";

        public DenseBramblesCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            // Destroy self at start of next env. turn
            base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker,
                new Func<PhaseChangeAction, IEnumerator>(base.DestroyThisCardResponse),
                TriggerType.DestroySelf, null, false);

            base.AddTriggers();
        }
        public override IEnumerator Play()
        {
            int numberOfTargets = Game.H - 1;

            List<Card> storedResults = new List<Card>();
            IEnumerator findTargetsWithLowestHpRoutine = base.GameController.FindTargetsWithLowestHitPoints(1, numberOfTargets, 
                (Card c) => c.IsTarget, storedResults, null, null, false, false, null, false);

            //IEnumerator immuneToDamageRoutine = base.AddStatusEffect(immuneToDamageStatusEffect, true);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(findTargetsWithLowestHpRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(findTargetsWithLowestHpRoutine);
            }

            // (H - 1) targets with the lowest HP are immune to damage
            foreach (Card card in storedResults)
            {
                ImmuneToDamageStatusEffect immuneToDamageStatusEffect = new ImmuneToDamageStatusEffect();
                immuneToDamageStatusEffect.TargetCriteria.IsSpecificCard = card;
                immuneToDamageStatusEffect.UntilCardLeavesPlay(this.Card);
                immuneToDamageStatusEffect.CardDestroyedExpiryCriteria.Card = this.Card;

                IEnumerator immuneToDamageRoutine = base.AddStatusEffect(immuneToDamageStatusEffect, true);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(immuneToDamageRoutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(immuneToDamageRoutine);
                }
            }
        }
    }
}