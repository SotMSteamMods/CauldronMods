using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.PhaseVillain
{
    public class DensityRegulatorCardController : PhaseVillainCardController
    {
        public DensityRegulatorCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroTargetWithLowestHP(ranking: 1);
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
            IEnumerator coroutine = base.DealDamage(base.CharacterCard, (Card card) => IsHero(card), 2, DamageType.Radiant,
                exceptFor: new TargetInfo(HighestLowestHP.LowestHP, 1, 1, new LinqCardCriteria((Card c) => IsHero(c), "the hero target with the lowest HP")));
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