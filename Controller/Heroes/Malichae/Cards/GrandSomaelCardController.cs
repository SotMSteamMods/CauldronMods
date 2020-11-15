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

        public override IEnumerator UsePower(int index = 0)
        {
            int reduces = GetPowerNumeral(0, 2);

            var card = GetCardThisCardIsNextTo();
            ReduceDamageStatusEffect effect = new ReduceDamageStatusEffect(reduces);
            effect.CardSource = card; //TODO - PROMO - CardSource can be Malichae
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
            yield break;
        }
    }
}
