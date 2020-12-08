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
            this.SpecialStringMaker.ShowLowestHP(1, () => Game.H - 1,
                new LinqCardCriteria(c => c.IsTarget && c.IsInPlay));
            AllowFastCoroutinesDuringPretend = false;
        }

        public bool? PerformImmune;

        public override void AddTriggers()
        {
            // Destroy self at start of next env. turn
            base.AddStartOfTurnTrigger(tt => tt == base.TurnTaker, base.DestroyThisCardResponse, TriggerType.DestroySelf);
            // (H - 1) targets with the lowest HP are immune to damage
            base.AddTrigger<DealDamageAction>(dd => GameController.FindAllTargetsWithLowestHitPoints(1, null, GetCardSource(), Game.H - 1).Any((Card c) => c == dd.Target), this.MaybeImmuneToDamageResponse, TriggerType.ImmuneToDamage, TriggerTiming.Before);
        }

        private IEnumerator MaybeImmuneToDamageResponse(DealDamageAction action)
        {
            int numberOfTargets = Game.H - 1;
            if (base.GameController.PretendMode)
            {
                List<bool> storedResults = new List<bool>();
                IEnumerator coroutine = DetermineIfGivenCardIsTargetWithinNthLowestHitPoints(action.Target, c => c.IsTarget, numberOfTargets, action, storedResults);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                this.PerformImmune = storedResults.Count() > 0 && storedResults.First();
            }
            
            if (this.PerformImmune != null && this.PerformImmune.Value)
            {
                IEnumerator coroutine2 = base.GameController.ImmuneToDamage(action, this.GetCardSource());
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

        // Variation on CardController.DetermineIfGivenCardIsTargetWithLowestOrHighestHitPoints that handles Nth lowest instead of single lowest.
        private IEnumerator DetermineIfGivenCardIsTargetWithinNthLowestHitPoints(Card card, Func<Card, bool> criteria, int numberOfTargets, DealDamageAction action, List<bool> storedResults, bool askIfTied = true)
        {
            bool item = true;
            IEnumerable<Card> source = base.GameController.FindAllTargetsWithLowestHitPoints(1, criteria, base.GetCardSource(), numberOfTargets: numberOfTargets);
            if (!source.Contains(card))
            {
                item = false;
            }
            else if (source.Count() > numberOfTargets && !GameController.PreviewMode && askIfTied
                // And this card is tied with the highest matching card.
                && card.HitPoints == source.Last().HitPoints)
            {
                List<YesNoCardDecision> yesNoResults = new List<YesNoCardDecision>();
                // Not quite correct, but without custom decision strings, this may be close enough.
                SelectionType type = SelectionType.LowestHP;
                IEnumerator coroutine = GameController.MakeYesNoCardDecision(base.HeroTurnTakerController, type, card, action, yesNoResults, null, base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
                item = (from d in yesNoResults
                        where d.Answer.HasValue
                        select d.Answer.Value).FirstOrDefault();
            }
            storedResults.Add(item);
        }
    }
}
