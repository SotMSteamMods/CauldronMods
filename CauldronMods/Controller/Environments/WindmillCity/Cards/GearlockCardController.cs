using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.WindmillCity
{
    public class GearlockCardController : WindmillCityUtilityCardController
    {
        public GearlockCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        private bool _triggered;
        public override void AddTriggers()
        {
            //Reduce damage dealt to this card by {H - 1}.
            AddReduceDamageTrigger((Card c) => c == Card, Game.H - 1);

            //Whenever this card is dealt damage, it deals the source of that damage 3 lightning damage.
            AddCounterDamageTrigger((DealDamageAction dd) => dd.Target == Card, () => Card, () => Card, false, 3, DamageType.Lightning);

            //When this card is reduced to 0 HP, play the top card of each hero deck in turn order.
            AddBeforeDestroyAction(LessThanZeroResponse);
        }
        private IEnumerator LessThanZeroResponse(GameAction action)
        {
            if (Card.HitPoints <= 0 && !_triggered)
            {
                _triggered = true;
                IEnumerator coroutine = GameController.SendMessageAction($"{Card.Title} plays the top card of each hero deck!", Priority.Medium, GetCardSource(), showCardSource: true);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                coroutine = PlayTopCardOfEachDeckInTurnOrder((TurnTakerController ttc) => IsHero(ttc.TurnTaker), (Location loc) => loc.IsHero);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                _triggered = false;
            }
            yield break;
        }
    }
}
