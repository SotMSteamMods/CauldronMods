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

        public static readonly string Identifier = "DenseBrambles";

        public DenseBramblesCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            this.SpecialStringMaker.ShowLowestHP(1, () => Game.H - 1,
                new LinqCardCriteria(c => c.IsTarget && c.IsInPlay));
        }

        public bool? PerformImmune;

        public override void AddTriggers()
        {
            // Destroy self at start of next env. turn
            base.AddStartOfTurnTrigger(tt => tt == base.TurnTaker, base.DestroyThisCardResponse, TriggerType.DestroySelf);
            // (H - 1) targets with the lowest HP are immune to damage
            base.AddTrigger<DealDamageAction>(dd => true, this.MaybeImmuneToDamageResponse, TriggerType.ImmuneToDamage, TriggerTiming.Before);
        }

        private IEnumerator MaybeImmuneToDamageResponse(DealDamageAction action)
        {
            int numberOfTargets = Game.H - 1;

            if (base.GameController.PretendMode)
            {
                List<Card> storedResults = new List<Card>();
                IEnumerator coroutine = base.GameController.FindTargetsWithLowestHitPoints(1, numberOfTargets, c => c.IsTarget, storedResults);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                this.PerformImmune = new bool?(storedResults.Contains(action.Target));
                storedResults = null;
            }
            
            if (this.PerformImmune != null && this.PerformImmune.Value)
            {
                IEnumerator coroutine2 = base.GameController.ImmuneToDamage(action, base.GetCardSource(null));
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine2);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine2);
                }
            }

            if (!base.GameController.PretendMode)
            {
                this.PerformImmune = null;
            }
            yield break;
        }
    }
}