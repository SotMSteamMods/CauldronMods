using System;
using System.Collections;
using System.Collections.Generic;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.DocHavoc
{
    public class RapidRegenCardController : CardController
    {
        //==============================================================
        // Increase all HP recovery by 1.
        //==============================================================

        public static string Identifier = "RapidRegen";
        private const int HpGainIncrease = 1;

        public RapidRegenCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //Increase all HP recovery by 1.
            this.AddTrigger<GainHPAction>(hp => true,
                hp => this.GameController.IncreaseHPGain(hp, HpGainIncrease, this.GetCardSource()),
                TriggerType.IncreaseHPGain, TriggerTiming.Before);
        }
    }
}
