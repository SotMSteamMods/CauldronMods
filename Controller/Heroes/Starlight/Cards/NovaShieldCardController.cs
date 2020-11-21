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
                       new TriggerType[] { TriggerType.DealDamage, TriggerType.GainHP },
                       TriggerTiming.After);
        }

        private bool IsCardEnteringPlayAsConstellationNextToTarget(CardEntersPlayAction ce)
        {
            //"Whenever a constellation enters play next to a target..."
            if (IsConstellation(ce.CardEnteringPlay) && ce.CardEnteringPlay.BattleZone == this.BattleZone)
            {
                var location = ce.CardEnteringPlay.Location;
                if (location != null && location.IsNextToCard && location.OwnerCard != null)
                    return location.OwnerCard.IsTarget;
            }
            return false;
        }
        private IEnumerator DamageAndHealResponse(CardEntersPlayAction ce)
        {
            var constellation = ce.CardEnteringPlay;
            if (constellation == null)
            {
                yield break;
            }

            //pick Starlight to act with
            List<Card> storedResults = new List<Card> { };
            IEnumerator chooseDamageSource = SelectActiveCharacterCardToDealDamage(storedResults, 1, DamageType.Energy);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(chooseDamageSource);
            }
            else
            {
                GameController.ExhaustCoroutine(chooseDamageSource);
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
                var damageAction = DealDamage(actingStarlight, target, 1, DamageType.Energy, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(damageAction);
                }
                else
                {
                    GameController.ExhaustCoroutine(damageAction);
                }
            }

            //"...and regains 1 HP."
            IEnumerator gainHPAction = GameController.GainHP(actingStarlight, 1, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(gainHPAction);
            }
            else
            {
                GameController.ExhaustCoroutine(gainHPAction);
            }

            yield break;
        }
    }
}