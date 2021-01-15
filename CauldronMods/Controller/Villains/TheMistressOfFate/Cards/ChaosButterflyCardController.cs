using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheMistressOfFate
{
    public class ChaosButterflyCardController : TheMistressOfFateUtilityCardController
    {
        public ChaosButterflyCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            _isStoredCard = false;
        }

        public override void AddTriggers()
        {
            //"At the end of the villain turn, this card deals each hero target 3 projectile damage and 3 cold damage.",
            AddEndOfTurnTrigger(tt => tt == TurnTaker, DealDamageResponse, TriggerType.DealDamage);
            //"When this card is destroyed, the players may swap the position of 2 face up Day cards. Cards under them move as well."
            AddWhenDestroyedTrigger(SwapDayCardsResponse, TriggerType.MoveCard);
        }

        private IEnumerator DealDamageResponse(GameAction ga)
        {
            var damages = new List<DealDamageAction>
            {
                new DealDamageAction(GetCardSource(), new DamageSource(GameController, this.Card), null, 3, DamageType.Projectile),
                new DealDamageAction(GetCardSource(), new DamageSource(GameController, this.Card), null, 3, DamageType.Cold)
            };
            IEnumerator coroutine = DealMultipleInstancesOfDamage(damages, (Card c) => c.IsHero);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }

        private IEnumerator SwapDayCardsResponse(DestroyCardAction dc)
        {
            //TODO
            IEnumerator coroutine = GameController.SendMessageAction("Not implemented yet! Sorry for your trouble.", Priority.High, GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }
    }
}
