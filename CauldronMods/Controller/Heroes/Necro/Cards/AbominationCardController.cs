using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Linq;
using System;

namespace Cauldron.Necro
{
    public class AbominationCardController : UndeadCardController
    {
        public AbominationCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, 6)
        {
        }

        public override void AddTriggers()
        {
            //At the end of your turn, this card deals all non-Undead hero targets 2 toxic damage.        
            base.AddEndOfTurnTrigger(tt => tt == base.TurnTaker, _ => base.DealDamage(this.Card, card => !IsUndead(card) && IsHeroTargetConsidering1929(card), 2, DamageType.Toxic), TriggerType.DealDamage);
            //When this card is destroyed, all players draw a card.
            base.AddWhenDestroyedTrigger(OnDestroyResponse, new TriggerType[] { TriggerType.PlayCard });
        }

        private IEnumerator OnDestroyResponse(DestroyCardAction dca)
        {
            //all players draw a card.
            IEnumerator coroutine = base.EachPlayerDrawsACard(tt => true);
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
