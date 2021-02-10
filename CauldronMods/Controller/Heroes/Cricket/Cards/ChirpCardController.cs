using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Cricket
{
    public class ChirpCardController : CardController
    {
        public ChirpCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            List<DiscardCardAction> list = new List<DiscardCardAction>();
            //Discard up to 3 cards.
            IEnumerator coroutine = base.SelectAndDiscardCards(base.HeroTurnTakerController, 3, requiredDecisions: 0, storedResults: list);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            int discards = GetNumberOfCardsDiscarded(list);

            //{Cricket} deals up to 4 targets X sonic damage each, where X is 1 plus the number of cards discarded this way.
            coroutine = base.GameController.SelectTargetsAndDealDamage(base.HeroTurnTakerController, new DamageSource(base.GameController, base.CharacterCard), discards + 1, DamageType.Sonic, 4, false, 0, cardSource: base.GetCardSource());
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