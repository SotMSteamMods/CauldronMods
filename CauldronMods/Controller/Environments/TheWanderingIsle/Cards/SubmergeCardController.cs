using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.TheWanderingIsle
{
    public class SubmergeCardController : TheWanderingIsleCardController
    {
        public SubmergeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowLocationOfCards(new LinqCardCriteria(c => c.Identifier == "Teryx", "Teryx", useCardsSuffix: false)).Condition = () => !Card.IsInPlayAndHasGameText;
        }

        public override void AddTriggers()
        {
            //Reduce all damage dealt by 2
            base.AddReduceDamageTrigger((Card c) => true, 2);
            //At the start of the environment turn, this card is destroyed.
            base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.DestroyThisCardResponse, TriggerType.DestroySelf);
        }

        public override IEnumerator Play()
        {
            return PlayTeryxFromDeckOrTrashThenShuffle();
        }
    }
}
