using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Outlander
{
    public class DisarmingBlowCardController : OutlanderUtilityCardController
    {
        public DisarmingBlowCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            //{Outlander} deals the 2 non-villain targets with the highest HP 3 melee damage each.
            List<DealDamageAction> damageActions = new List<DealDamageAction>();
            IEnumerator coroutine = base.DealDamageToHighestHP(base.CharacterCard, 1, (Card c) => !base.IsVillain(c) && c.IsTarget, (Card c) => 3, DamageType.Melee, storedResults: damageActions);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }

        public override void AddTriggers()
        {
            //Any hero damaged this way discards 1 card.
            base.AddTrigger<DealDamageAction>((DealDamageAction action) => action.CardSource == base.GetCardSource() && action.Target.IsHero && action.Target.IsCharacter && action.DidDealDamage, (DealDamageAction action) => base.GameController.SelectAndDiscardCard(base.FindHeroTurnTakerController(action.Target.Owner.ToHero())), TriggerType.DiscardCard, TriggerTiming.After);
        }
    }
}
