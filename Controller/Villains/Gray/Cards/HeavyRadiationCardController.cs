using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.Gray
{
    public class HeavyRadiationCardController : GrayCardController
    {
        public HeavyRadiationCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria((Card c) => c.DoKeywordsContain("radiation"), "radiation"));
        }

        public override void AddTriggers()
        {
            //Reduce damage dealt to {Gray} by 1 for each Radiation card in play.
            base.AddReduceDamageTrigger((Card c) => c == base.CharacterCard, FindNumberOfRadiationCardsInPlay() ?? default);
            //At the end of the villain turn, if there are no Radiation cards in play, play the top card of the villain deck.
            base.AddEndOfTurnTrigger((TurnTaker turnTaker) => turnTaker == base.TurnTaker, base.PlayTheTopCardOfTheVillainDeckResponse, TriggerType.PlayCard, (PhaseChangeAction action) => FindNumberOfRadiationCardsInPlay() == 0);
        }
    }
}
