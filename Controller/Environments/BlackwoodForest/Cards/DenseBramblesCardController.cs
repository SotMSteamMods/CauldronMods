using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            this.SpecialStringMaker.ShowSpecialString(() => $"Dense Brambles is currently making {this.GetCurrentImmuneTargets()} immune to damage");
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

        private string GetCurrentImmuneTargets()
        {
            // Get all immune to damage effects from this card source
            List<ImmuneToDamageJournalEntry> immuneToDamageStatusEffects 
                = base.GameController.Game.Journal.ImmuneToDamageEntries(entry => entry.CardSource.Equals(this.Card)).ToList();

            // Take last H - 1 entries
            IEnumerable<ImmuneToDamageJournalEntry> mostRecent = immuneToDamageStatusEffects.Skip(Math.Max(0, immuneToDamageStatusEffects.Count() - (this.Game.H - 1)));

            // Build string of card titles
            string targets = mostRecent.Select(entry => entry.Target.Title).Aggregate((a, b) => a + ", " + b);

            return targets;
        }
    }
}