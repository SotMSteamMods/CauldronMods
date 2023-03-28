using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Malichae
{
    public class GrandEzaelCardController : DjinnOngoingController
    {
        public GrandEzaelCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController, "HighEzael", "Ezael")
        {
        }

        public override void AddTriggers()
        {
            base.AddTriggers();
            AddDestroyAtEndOfTurnTrigger();
        }

        public override Power GetGrantedPower(CardController cardController, Card damageSource = null)
        {
            return new Power(cardController.HeroTurnTakerController, cardController, "All hero targets regain 3HP.", UseGrantedPower(), 0, null, GetCardSource());
        }

        private IEnumerator UseGrantedPower()
        {
            int hp = GetPowerNumeral(0, 3);

            CardSource cs = GetCardSourceForGrantedPower();
            var card = cs.Card;

            var coroutine = base.GameController.GainHP(DecisionMaker, c => c.IsTarget && IsHero(c) && c.IsInPlayAndHasGameText, hp, cardSource: cs);
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
