using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Linq;
using System;

namespace Cauldron.Necro
{
    public class DemonicImpCardController : UndeadCardController
    {
        public DemonicImpCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, 2)
        {
        }

        public override void AddTriggers()
        {
            //At the end of your turn, destroy 1 hero equipment or ongoing card.          
            base.AddEndOfTurnTrigger(tt => tt == base.TurnTaker, _ => base.GameController.SelectAndDestroyCards(this.DecisionMaker, new LinqCardCriteria(c => IsHeroConsidering1929(c) && (IsOngoing(c) || IsEquipment(c)), $"{HeroStringConsidering1929} ongoing or equipment"), 1, cardSource: base.GetCardSource()), TriggerType.DestroyCard);
            //When this card is destroyed, one player may play a card.
            base.AddWhenDestroyedTrigger(OnDestroyResponse, new TriggerType[] { TriggerType.PlayCard });
        }

        private IEnumerator OnDestroyResponse(DestroyCardAction dca)
        {
            //one player may play a card.
            IEnumerator coroutine = base.SelectHeroToPlayCard(this.DecisionMaker);
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
