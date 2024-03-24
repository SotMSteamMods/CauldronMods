using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.Menagerie
{
    public class HiredKeeperCardController : MenagerieCardController
    {
        public HiredKeeperCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHighestHP(1, () => 2, new LinqCardCriteria(c => IsHeroTarget(c) && !IsCaptured(c.Owner), "non-captured hero targets", false));
        }

        public override void AddTriggers()
        {
            //At the end of the villain turn, this card deals the 2 non-Captured hero targets with the highest HP 2 sonic damage each.
            base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.DealDamageResponse, TriggerType.DealDamage);
            //Whenever a Specimen is destroyed, destroy 1 hero ongoing or equipment card.
            base.AddTrigger<DestroyCardAction>((DestroyCardAction action) => action.WasCardDestroyed && base.IsSpecimen(action.CardToDestroy.Card), this.DestroyCardResponse, TriggerType.DestroyCard, TriggerTiming.After);
            base.AddTriggers();

        }

        private IEnumerator DealDamageResponse(PhaseChangeAction action)
        {
            //...this card deals the 2 non-Captured hero targets with the highest HP 2 sonic damage each.
            IEnumerator coroutine = base.DealDamageToHighestHP(base.Card, 1, (Card c) => !base.IsCaptured(c.Owner) && IsHeroTarget(c), (Card c) => 2, DamageType.Sonic, numberOfTargets: () => 2);
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

        private IEnumerator DestroyCardResponse(DestroyCardAction action)
        {
            //...destroy 1 hero ongoing or equipment card.
            IEnumerator coroutine = base.GameController.SelectAndDestroyCard(this.DecisionMaker, new LinqCardCriteria((Card c) => IsHero(c) && (IsOngoing(c) || base.IsEquipment(c)), "hero ongoing or equipment"), false, cardSource: GetCardSource());
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