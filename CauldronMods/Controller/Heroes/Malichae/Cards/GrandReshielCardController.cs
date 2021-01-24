using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Malichae
{
    public class GrandReshielCardController : DjinnOngoingController
    {
        public GrandReshielCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController, "HighReshiel", "Reshiel")
        {
        }

        public override void AddTriggers()
        {
            base.AddTriggers();
            AddDestroyAtEndOfTurnTrigger();
        }

        public override Power GetGrantedPower(CardController cardController)
        {
            return new Power(cardController.HeroTurnTakerController, cardController, $"{cardController.Card.Title}  deals each non-hero target 2 sonic damage.", UseGrantedPower(), 0, null, GetCardSource());
        }

        private IEnumerator UseGrantedPower()
        {
            int damages = GetPowerNumeral(0, 2);

            CardSource cs = GetCardSourceForGrantedPower();
            var card = cs.Card;

            var coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, card), damages, DamageType.Sonic, null, false, null,
                allowAutoDecide: true,
                additionalCriteria: c => c.IsInPlay && !c.IsHero,
                cardSource: cs);
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
