using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Malichae
{
    public class GrandSomaelCardController : DjinnOngoingController
    {
        public GrandSomaelCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController, "HighSomael", "Somael")
        {
        }

        public override void AddTriggers()
        {
            base.AddTriggers();
            AddDestroyAtEndOfTurnTrigger();
        }

        public override Power GetGrantedPower(CardController cardController, Card damageSource = null)
        {
            return new Power(cardController.HeroTurnTakerController, cardController, "Reduce damage dealt to hero targets by 2 until the start of your next turn.", UseGrantedPower(), 0, null, GetCardSource());
        }

        private IEnumerator UseGrantedPower()
        {
            int reduces = GetPowerNumeral(0, 2);

            CardSource cs = GetCardSourceForGrantedPower();
            var card = cs.Card;

            ReduceDamageStatusEffect effect = new ReduceDamageStatusEffect(reduces);
            effect.CardSource = card;
            effect.UntilStartOfNextTurn(this.DecisionMaker.TurnTaker);
            effect.TargetCriteria.IsInPlayAndNotUnderCard = true;
            effect.TargetCriteria.IsHero = true;
            effect.TargetCriteria.IsTarget = true;

            var coroutine = GameController.AddStatusEffect(effect, true, cs);
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
