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

        public override IEnumerator UsePower(int index = 0)
        {
            var card = GetCardThisCardIsNextTo();
            ReduceDamageStatusEffect effect = new ReduceDamageStatusEffect(1);
            effect.CardSource = card;
            effect.UntilStartOfNextTurn(this.DecisionMaker.TurnTaker);
            effect.TargetCriteria.IsInPlayAndNotUnderCard = true;
            effect.TargetCriteria.IsHero = true;
            effect.TargetCriteria.IsTarget = true;

            var coroutine = GameController.AddStatusEffect(effect, true, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            coroutine = base.GameController.DestroyCard(DecisionMaker, this.Card,
                            responsibleCard: this.CharacterCard,
                            cardSource: GetCardSource());
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
