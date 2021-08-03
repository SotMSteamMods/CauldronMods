using Handelabra.Sentinels.Engine.Controller;
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

        public override Power GetGrantedPower(CardController cardController, Card damageSource = null)
        {
            Card dmgSource = damageSource ?? cardController.Card;
            return new Power(cardController.HeroTurnTakerController, cardController, $"{dmgSource.Title} deals 1 target 6 energy damage.", UseGrantedPower(dmgSource), 0, null, GetCardSource());
        }

        private IEnumerator UseGrantedPower(Card damageSource = null)
        {
            int targets = GetPowerNumeral(0, 1);
            int damages = GetPowerNumeral(1, 6);

            CardSource cs = GetCardSourceForGrantedPower();
            var card = cs?.Card;
            Card dmgSource = damageSource ?? card;

            if(dmgSource is null || !dmgSource.IsInPlayAndHasGameText)
            {
                string sourceTitle = dmgSource is null ? "Bathiel" : dmgSource.Title;
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

            var coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, dmgSource), damages, DamageType.Energy, targets, false, targets, cardSource: cs);
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
