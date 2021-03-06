﻿using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Malichae
{
    public class GrandBathielCardController : DjinnOngoingController
    {
        public GrandBathielCardController(Card card, TurnTakerController turnTakerController)
            : base(card, turnTakerController, "HighBathiel", "Bathiel")
        {
        }

        public override void AddTriggers()
        {
            base.AddTriggers();
            AddDestroyAtEndOfTurnTrigger();
        }

        public override Power GetGrantedPower(CardController cardController)
        {
            return new Power(cardController.HeroTurnTakerController, cardController, $"{cardController.Card.Title} deals 1 target 6 energy damage.", UseGrantedPower(), 0, null, GetCardSource());
        }

        private IEnumerator UseGrantedPower()
        {
            int targets = GetPowerNumeral(0, 1);
            int damages = GetPowerNumeral(1, 6);

            CardSource cs = GetCardSourceForGrantedPower();
            var card = cs.Card;

            var coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, card), damages, DamageType.Energy, targets, false, targets, cardSource: cs);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }
    }
}
