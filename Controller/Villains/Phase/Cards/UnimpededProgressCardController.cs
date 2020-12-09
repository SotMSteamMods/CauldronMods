﻿using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Linq;

namespace Cauldron.Phase
{
    public class UnimpededProgressCardController : CardController
    {
        public UnimpededProgressCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowSpecialString(() => "Phase hase been dealt damage " + base.Game.Journal.DealDamageEntriesThisRound().Where((DealDamageJournalEntry entry) => entry.TargetCard == base.CharacterCard).Count() + " times this round.");
        }

        public override void AddTriggers()
        {
            //At the end of the villain turn, play the top card of the villain deck.
            base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, base.PlayTheTopCardOfTheVillainDeckResponse, TriggerType.PlayCard);
            //Destroy this card when {Phase} is dealt 2 times {H} or more damage in 1 round.
            base.AddTrigger<DestroyCardAction>((DestroyCardAction action) => base.Game.Journal.DealDamageEntriesThisRound().Where((DealDamageJournalEntry entry) => entry.TargetCard == base.CharacterCard).Count() >= Game.H * 2, base.DestroyThisCardResponse, TriggerType.DestroySelf, TriggerTiming.After);
        }
    }
}