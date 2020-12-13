using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Handelabra;

namespace Cauldron.Impact
{
    public class CrushingRiftCardController : CardController
    {
        public CrushingRiftCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //"Select a non-character target in play with current HP less than its maximum HP.",
            var storedTarget = new List<SelectCardDecision> { };
            IEnumerator coroutine = GameController.SelectCardAndStoreResults(DecisionMaker, SelectionType.SetHP, new LinqCardCriteria((Card c) => c.IsInPlayAndHasGameText && c.IsTarget && !c.IsCharacter && c.HitPoints < c.MaximumHitPoints, "", false, singular: "non-character target with less than maximum HP", plural:"non-character targets with less than maximum HP"), storedTarget, false, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if(DidSelectCard(storedTarget))
            {
                //"Reduce that target to half of its current HP, rounded up."
                var selectedTarget = GetSelectedCard(storedTarget);
                int currentHP = selectedTarget.HitPoints ?? 0;
                int halfHP = currentHP;
                if(halfHP > 0)
                {
                    halfHP = (currentHP / 2) + currentHP % 2;
                }
                coroutine = GameController.SetHP(selectedTarget, halfHP, GetCardSource());
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