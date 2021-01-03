using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Linq;

namespace Cauldron.Menagerie
{
    public class HyrianSnipeCardController : CardController
    {
        public HyrianSnipeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //At the end of the villain turn, this card deals the 2 targets other than itself with the highest HP {H - 1} psychic damage each. Then, destroy 1 equipment card.
            base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.DealDamageAndDestroyResponse, new TriggerType[] { TriggerType.DealDamage, TriggerType.DestroyCard });
        }

        private IEnumerator DealDamageAndDestroyResponse(PhaseChangeAction action)
        {
            //...this card deals the 2 targets other than itself with the highest HP {H - 1} psychic damage each.
            IEnumerator coroutine = base.DealDamageToHighestHP(base.Card, 1, (Card c) => c != base.Card, (Card c) => Game.H - 1, DamageType.Psychic, numberOfTargets: () => 2);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //Then, destroy 1 equipment card.
            if (base.FindCardsWhere(new LinqCardCriteria((Card c) => base.IsEquipment(c) && c.IsInPlayAndHasGameText)).Any())
            {
                coroutine = base.GameController.SelectAndDestroyCard(this.DecisionMaker, new LinqCardCriteria((Card c) => base.IsEquipment(c), "equipment"), false);
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