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
            var coroutine = GameController.GainHP(this.DecisionMaker, c => c.IsTarget && c.IsInPlayAndHasGameText && IsDjinn(c), 1, cardSource: GetCardSource());
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

        public override Power GetGrantedPower(CardController cardController, Card damageSource = null)
        {
            return new Power(cardController.HeroTurnTakerController, cardController, $"All Djinn regain 2HP. All other hero targets regain 1HP. Destroy {this.Card.Title}.", UseGrantedPower(), 0, null, GetCardSource());
        }

        private IEnumerator UseGrantedPower()
        {
            int djinnHP = GetPowerNumeral(0, 2);
            int otherHP = GetPowerNumeral(1, 1);

            CardSource cs = GetCardSourceForGrantedPower();
            var card = cs.Card;

            var coroutine = base.GameController.GainHP(DecisionMaker, c => c.IsTarget && c.IsInPlayAndHasGameText && (IsHeroTarget(c) || IsDjinn(c)), c => IsDjinn(c) ? djinnHP : otherHP, cardSource: cs);
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
