using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.TheKnight
{
    public abstract class SingleHandEquipmentCardController : RoninAssignableCardController
    {
        protected SingleHandEquipmentCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsAtLocation(HeroTurnTaker.PlayArea, new LinqCardCriteria(c => IsEquipment(c) && IsSingleHandCard(c), "single hand equipment"));
        }

        public override void AddTriggers()
        {
            var trigger = base.AddTrigger<CardEntersPlayAction>(ca => ca.CardEnteringPlay == this.Card, ca => DestroyExcessSingleHandCardReponse(), TriggerType.DestroyCard, TriggerTiming.After);
            base.AddToTemporaryTriggerList(trigger);
            base.AddTriggers();
        }

        private IEnumerator DestroyExcessSingleHandCardReponse()
        {
            //"When this card is put into play, destroy all but 2 Single Hand Equipment cards.",
            int num = base.FindCardsWhere((Card c) => IsEquipment(c) && IsSingleHandCard(c) && c.IsInPlay).Count();
            if (num > 2)
            {
                List<DestroyCardAction> storedResults = new List<DestroyCardAction>();
                var criteria = new LinqCardCriteria(c => IsEquipment(c) && IsSingleHandCard(c) && c.IsInPlay, SingleHandKeyword + " equipment");
                var coroutine = base.GameController.SelectAndDestroyCards(this.DecisionMaker, criteria, num - 2, false, storedResultsAction: storedResults, cardSource: base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            yield break;
        }
    }
}
