using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Malichae
{
	public class HighSomaelCardController : DjinnOngoingController
	{
		public HighSomaelCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController, "Somael", "Somael")
		{
		}
        public override void AddTriggers()
        {
            base.AddReduceDamageTrigger(c => IsDjinn(c), 1);
            base.AddTriggers();
        }

        public override Power GetGrantedPower(CardController cardController, Card damageSource = null)
        {
            return new Power(cardController.HeroTurnTakerController, cardController, $"Reduce damage dealt to hero targets by 1 until the start of your next turn. Destroy {this.Card.Title}.", UseGrantedPower(), 0, null, GetCardSource());
        }

        private IEnumerator UseGrantedPower()
        {
            int reduces = GetPowerNumeral(0, 1);

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
            //when played via discard, I'll already be in the trash, so skip
            if (!Card.IsInTrash)
            {
                coroutine = DestroySelf();
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
