using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Quicksilver
{
    public class ForestOfNeedlesCardController : QuicksilverBaseCardController
    {
        public ForestOfNeedlesCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
        public override IEnumerator Play()
        {
            //{Quicksilver} may deal 6 melee damage to a target with more than 8HP, or 3 melee damage to a target with 8 or fewer HP.
            var storedResults = new List<SelectTargetDecision>();
            IEnumerator coroutine = base.GameController.SelectTargetAndStoreResults(base.HeroTurnTakerController, GameController.FindTargetsInPlay((Card c) => GameController.IsCardVisibleToCardSource(c, GetCardSource())), storedResults, optional: true, damageSource: CharacterCard, damageAmount: c => DynamicDamage(c), damageType: DamageType.Melee, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            var target = storedResults.FirstOrDefault()?.SelectedCard;
            if(target != null)
            {
                int chosenAmount = DynamicDamage(target);
                coroutine = DealDamage(CharacterCard, target, chosenAmount, DamageType.Melee, cardSource: GetCardSource());
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

        private int DynamicDamage(Card c)
        {
            int amount;
            if (c != null && c.IsTarget && c.HitPoints > 8)
            {
                amount = 6;
            }
            else
            {
                amount = 3;
            }
            return amount;
        }
    }
}