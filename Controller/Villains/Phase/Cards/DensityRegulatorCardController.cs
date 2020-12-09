using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.Phase
{
    public class DensityRegulatorCardController : CardController
    {
        public DensityRegulatorCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //Reduce damage dealt to {Phase} by 1.
            base.AddReduceDamageTrigger((Card c) => c == base.CharacterCard, 1);
            //At the end of the villain turn, {Phase} deals each hero target except the hero target with the lowest HP 2 radiant damage.
            base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.DealDamageResponse, TriggerType.DealDamage);
        }

        private IEnumerator DealDamageResponse(PhaseChangeAction action)
        {
            //...{Phase} deals each hero target except the hero target with the lowest HP 2 radiant damage.
            IEnumerator coroutine = base.DealDamage(base.CharacterCard, (Card card) => card.IsHero, 2, DamageType.Radiant, damageSourceInfo: new TargetInfo(HighestLowestHP.LowestHP, 1, 1, new LinqCardCriteria((Card c) => c.IsHero, "the hero target with the lowest HP")));
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }
    }
}