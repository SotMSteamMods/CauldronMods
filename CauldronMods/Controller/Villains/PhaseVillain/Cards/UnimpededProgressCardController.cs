using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Linq;

namespace Cauldron.PhaseVillain
{
    public class UnimpededProgressCardController : PhaseVillainCardController
    {
        public UnimpededProgressCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowSpecialString(() => $"{CharacterCard.Title} has been dealt {PhaseDamageThisRound()} damage this round.");
        }

        public override void AddTriggers()
        {
            //At the end of the villain turn, play the top card of the villain deck.
            base.AddEndOfTurnTrigger(tt => tt == base.TurnTaker, base.PlayTheTopCardOfTheVillainDeckResponse, TriggerType.PlayCard);
            //Destroy this card when {Phase} is dealt 2 times {H} or more damage in 1 round.
            base.AddTrigger<DealDamageAction>(action => PhaseDamageThisRound() >= Game.H * 2, base.DestroyThisCardResponse, TriggerType.DestroySelf, TriggerTiming.After);
        }

        private int PhaseDamageThisRound()
        {
            return base.Game.Journal.DealDamageEntriesThisRound()
                    .Where(entry => entry.TargetCard == base.CharacterCard)
                    .Sum(entry => entry.Amount);
        }
    }
}