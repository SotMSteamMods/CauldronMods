using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Malichae
{
    public class HighEzaelCardController : DjinnOngoingController
    {
        public HighEzaelCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController, "Ezael", "Ezael")
        {
        }

        public override void AddTriggers()
        {
            base.AddEndOfTurnTrigger(tt => tt == base.TurnTaker, EndOfTurnReponse, TriggerType.GainHP);
            base.AddTriggers();
        }

        private IEnumerator EndOfTurnReponse(PhaseChangeAction pca)
        {
            var coroutine = GameController.GainHP(this.DecisionMaker, c => c.IsTarget && IsDjinn(c), 1, cardSource: GetCardSource());
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

        public override IEnumerator UsePower(int index = 0)
        {
            int djinnHP = GetPowerNumeral(0, 2);
            int otherHP = GetPowerNumeral(1, 1);

            var card = GetCardThisCardIsNextTo();
            var coroutine = base.GameController.GainHP(DecisionMaker, c => c.IsTarget && (c.IsHero || IsDjinn(c)), c => IsDjinn(c) ? djinnHP : otherHP, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            coroutine = DestroySelf();
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
