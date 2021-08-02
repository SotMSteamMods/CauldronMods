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

        public override Power GetGrantedPower(CardController cardController, Card damageSource = null)
        {
            Card dmgSource = damageSource ?? cardController.Card;
            return new Power(cardController.HeroTurnTakerController, cardController, $"{dmgSource.Title}  deals each non-hero target 2 sonic damage.", UseGrantedPower(dmgSource), 0, null, GetCardSource());
        }

        private IEnumerator UseGrantedPower(Card damageSource = null)
        {
            int damages = GetPowerNumeral(0, 2);

            CardSource cs = GetCardSourceForGrantedPower();
            var card = cs.Card;

            Card dmgSource = damageSource ?? card;

            if (dmgSource is null || !dmgSource.IsInPlayAndHasGameText)
            {
                string sourceTitle = dmgSource is null ? "Reshiel" : dmgSource.Title;
                //send message about damage fizzling
                IEnumerator sendMessage = GameController.SendMessageAction($"{sourceTitle} is not in play, so no damage will be dealt.", Priority.Medium, GetCardSource(), showCardSource: true);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(sendMessage);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(sendMessage);
                }

                yield break;
            }
            var coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, dmgSource), damages, DamageType.Sonic, null, false, null,
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
