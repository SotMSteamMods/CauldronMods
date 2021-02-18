using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.OblaskCrater
{
    public class SwarmOfFangsCardController : OblaskCraterUtilityCardController
    {
        /*
         * At the end of the environment turn, this card deals the target other than itself
         * with the lowest HP 2 melee damage 
         * If this damage destroys a target, repeat the text of this card.
         */
        public SwarmOfFangsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowLowestHP(cardCriteria: new LinqCardCriteria(c => c != Card, "target othan the itself", useCardsSuffix: false));
        }

        public override void AddTriggers()
        {
            base.AddEndOfTurnTrigger((tt) => tt.IsEnvironment, PhaseChangeActionResponse, TriggerType.DealDamage);
        }

        private IEnumerator PhaseChangeActionResponse(PhaseChangeAction phaseChangeAction)
        {
            IEnumerator coroutine;
            IEnumerable<Card> lowestHpTargets;
            List<DealDamageAction> dealDamageActions = new List<DealDamageAction>();

            lowestHpTargets = base.GameController.FindAllTargetsWithLowestHitPoints(1, (card) => card != base.Card, base.GetCardSource());

            if (lowestHpTargets != null && lowestHpTargets.Count() > 0)
            {
                coroutine = base.GameController.SelectTargetsAndDealDamage(base.HeroTurnTakerController, new DamageSource(base.GameController, base.Card), 2, DamageType.Energy, 1, false, 1, additionalCriteria: (card) => lowestHpTargets.Contains(card), storedResultsDamage: dealDamageActions, cardSource: base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                if (DidDealDamage(dealDamageActions) && dealDamageActions.Count((dda)=>dda.DidDestroyTarget) > 0)
                {
                    coroutine = GameController.SendMessageAction($"{Card.Title} destroyed a target, so it repeats its effect!", Priority.Medium, GetCardSource(), showCardSource: true);
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }

                    coroutine = PhaseChangeActionResponse(phaseChangeAction);
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                }
            }

            yield break;
        }
    }
}
