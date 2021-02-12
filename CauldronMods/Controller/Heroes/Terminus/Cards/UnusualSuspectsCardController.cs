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
            List<string> cardKeywords = target.GetKeywords().ToList();
            List<Location> allTrash = base.GameController.AllTurnTakers.Where((tt) => !tt.IsIncapacitatedOrOutOfGame && tt.Trash.HasCards).Select((tt) => tt.Trash).ToList();
            bool matchesKeyword = false;

            foreach (Location trash in allTrash)
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
