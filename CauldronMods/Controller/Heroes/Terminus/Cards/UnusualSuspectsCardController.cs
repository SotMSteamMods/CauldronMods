using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Terminus
{
    public class UnusualSuspectsCardController : TerminusBaseCardController
    {
        /* 
         * Add 2 tokens to your Wrath pool.
         * {Terminus} deals up to 2 targets 2 cold damage each. If {Terminus} deals damage this way to
         * a target that shares a keyword with a card in any trash pile, increase that damage by 2.
         */
        public UnusualSuspectsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowTokenPool(base.WrathPool);
            base.AllowFastCoroutinesDuringPretend = false;
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;
            ITrigger tempDealDamageTrigger;
            Func<DealDamageAction, bool> increaseDamageCriteria = (dda) => {
                return dda.CanDealDamage && dda.CardSource.Card == base.Card && HasKeywordInAnyTrash(dda.Target);
            };

            // Add 2 tokens to your Wrath pool.
            coroutine = base.AddWrathTokens(2);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            tempDealDamageTrigger = base.AddToTemporaryTriggerList(base.AddIncreaseDamageTrigger(increaseDamageCriteria, 2));

            // {Terminus} deals up to 2 targets 2 cold damage each.
            coroutine = base.GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(base.GameController, base.CharacterCard), 2, DamageType.Cold, 2, false, 0, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            base.RemoveTemporaryTrigger(tempDealDamageTrigger);

            yield break;
        }

        private bool HasKeywordInAnyTrash(Card target)
        {
            List<Location> trashes = new List<Location>();
            foreach (TurnTaker tt in Game.TurnTakers)
            {
                if (tt.IsIncapacitatedOrOutOfGame) continue;
                if (tt.Trash.IsRealTrash && GameController.IsLocationVisibleToSource(tt.Trash, GetCardSource()))
                {
                    trashes.Add(tt.Trash);
                }
                trashes = trashes.Concat(tt.SubTrashes.Where(l => l.IsRealTrash && GameController.IsLocationVisibleToSource(l, GetCardSource()))).ToList();
            }
            List<string> cardKeywords = target.GetKeywords().ToList();
            bool matchesKeyword = false;

            foreach (Location trash in trashes)
            {
                if (trash.Cards.Any(card => card.DoKeywordsContain(cardKeywords)))
                {
                    matchesKeyword = true;
                    break;
                }
            }

            return matchesKeyword;
        }
    }
}
