using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Starlight
{
    public class NovaShieldCardController : StarlightCardController
    {
        public NovaShieldCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //"Whenever a constellation enters play next to a target, Starlight deals that target 1 energy damage and regains 1 HP."
            AddTrigger<CardEntersPlayAction>(criteria: IsCardEnteringPlayAsConstellationNextToTarget,
                       response: DamageAndHealResponse,
                       new TriggerType[2] { TriggerType.DealDamage, TriggerType.GainHP },
                       TriggerTiming.After);
        }

        private bool IsCardEnteringPlayAsConstellationNextToTarget(CardEntersPlayAction ce)
        {
            //"Whenever a constellation enters play next to a target..."
            if (IsConstellation(ce.CardEnteringPlay) && ce.CardEnteringPlay.BattleZone == base.BattleZone)
            {
                var enteringBy = FindCardsWhere((Card c) => c.IsInPlay && c.GetAllNextToCards(false).Contains(ce.CardEnteringPlay)).FirstOrDefault();
                return enteringBy != null && enteringBy.IsTarget;
            }
            return false;
        }
        private IEnumerator DamageAndHealResponse(CardEntersPlayAction ce)
        {

            var constellation = ce.CardEnteringPlay;

            var testmessage = GameController.SendMessageAction("Damage-and-heal-response triggers", Priority.Low, GetCardSource());
            GameController.ExhaustCoroutine(testmessage);

            if (constellation == null)
            {
                yield break;
            }

            //pick Starlight to act with
            List<Card> storedResults = new List<Card> { };
            IEnumerator chooseDamageSource = SelectActiveCharacterCardToDealDamage(storedResults, 1, DamageType.Energy);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(chooseDamageSource);
            }
            else
            {
                base.GameController.ExhaustCoroutine(chooseDamageSource);
            }
            Card actingStarlight = storedResults.FirstOrDefault();

            if (actingStarlight == null)
            {
                yield break;
            }

            var target = FindCardsWhere((Card c) => c.IsInPlay && c.IsTarget && c.GetAllNextToCards(false).Contains(constellation), visibleToCard: GetCardSource()).FirstOrDefault();
            

            if (target != null)
            {
                //"...Starlight deals that target 1 energy damage..."
                var damageAction = DealDamage(actingStarlight, target, 1, DamageType.Energy);

                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(damageAction);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(damageAction);
                }
            }

            testmessage = GameController.SendMessageAction("Damage-and-heal-response does not stop after constellation dies", Priority.Low, GetCardSource());
            GameController.ExhaustCoroutine(testmessage);

            //"...and regains 1 HP."
            var gainHPAction = GameController.GainHP(actingStarlight, 1, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(gainHPAction);
            }
            else
            {
                base.GameController.ExhaustCoroutine(gainHPAction);
            }

            yield break;
        }
    }
}