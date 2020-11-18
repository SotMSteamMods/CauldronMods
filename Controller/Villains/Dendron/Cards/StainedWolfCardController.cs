using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Dendron
{
    public class StainedWolfCardController : CardController
    {
        //==============================================================
        // At the end of the villain turn, this card deals
        // the hero target with the highest HP {H - 1} melee damage.
        //==============================================================

        public static string Identifier = "StainedWolf";

        public StainedWolfCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
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
            IEnumerator findTargetWithLowestHpRoutine = base.GameController.FindTargetWithHighestHitPoints(1,
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
            int damageToDeal = Game.H - 1;

            IEnumerator dealDamageRoutine = this.DealDamage(this.Card, storedResults.First(),
                damageToDeal, DamageType.Melee);

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