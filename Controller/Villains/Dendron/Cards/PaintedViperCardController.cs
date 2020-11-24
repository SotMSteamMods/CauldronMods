using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace Cauldron.Dendron
{
    public class PaintedViperCardController : CardController
    {
        //==============================================================
        // At the end of the villain turn, this card deals
        // the hero target with the lowest HP {H - 2} toxic damage.
        //==============================================================

        public static string Identifier = "PaintedViper";

        public PaintedViperCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {

            base.AddEndOfTurnTrigger(tt => tt == base.TurnTaker, EndOfTurnDealDamageResponse, TriggerType.DealDamage);

            base.AddTriggers();
        }

        private IEnumerator EndOfTurnDealDamageResponse(PhaseChangeAction pca)
        {
            List<Card> storedResults = new List<Card>();
            IEnumerator findTargetWithLowestHpRoutine = base.GameController.FindTargetWithLowestHitPoints(1,
                c => c.IsHero && !c.IsIncapacitatedOrOutOfGame, storedResults);

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(findTargetWithLowestHpRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(findTargetWithLowestHpRoutine);
            }

            if (!storedResults.Any())
            {
                yield break;
            }

            //  At the end of the villain turn, this card deals the hero target with the lowest HP {H - 2} toxic damage.

            int damageToDeal = Game.H - 2;

            IEnumerator dealDamageRoutine = this.DealDamage(this.Card, storedResults.First(),
                damageToDeal, DamageType.Toxic);

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(dealDamageRoutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(dealDamageRoutine);
            }
        }
    }
}