using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Celadroch
{
    public class LingeringExhalationCardController : CeladrochOngoingCardController
    {
        /*
         * 	"When this card enters play, play the top card of the villain deck.",
			"Destroy this card when {Celadroch} is dealt 16 damage in a single round.",
			"When this card is destroyed, put all targets from the villain trash into play."
         */

        public LingeringExhalationCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowSpecialString(() => CharacterCard.AlternateTitleOrTitle + " has been dealt " + Journal.DealDamageEntriesThisRound().Where(j => j.TargetCard == CharacterCard).Sum(j => j.Amount) + " damage this round.").Condition = () => Card.IsInPlayAndHasGameText;
            SpecialStringMaker.ShowNumberOfCardsAtLocation(TurnTaker.Trash, new LinqCardCriteria(c => c.IsTarget, "target"));
        }

        public override void AddTriggers()
        {
            AddTrigger<DealDamageAction>(dda => dda.Target == CharacterCard && SelfDestuctionCriteria(dda), dda => DestroyThisCardResponse(dda), TriggerType.DestroySelf, TriggerTiming.After, ActionDescription.DamageTaken);

            AddWhenDestroyedTrigger(dca => PlayTargetsFromTrash(), TriggerType.PutIntoPlay);
        }

        private bool SelfDestuctionCriteria(DealDamageAction dda)
        {
            var total = Journal.DealDamageEntriesThisRound().Where(j => j.TargetCard == CharacterCard).Sum(j => j.Amount);
            return total >= 16;
        }
        private IEnumerator PlayTargetsFromTrash()
        {
            return GameController.MoveCards(TurnTakerController, TurnTaker.Trash.Cards.Where(c => c.IsTarget), TurnTaker.PlayArea, isPutIntoPlay: true, cardSource: GetCardSource());
        }
    }
}