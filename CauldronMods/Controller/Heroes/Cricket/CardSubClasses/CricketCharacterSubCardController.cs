using Cauldron.Tiamat;
using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Cricket
{
    public class CricketCharacterSubCardController : HeroCharacterCardController
    {
        public CricketCharacterSubCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddStartOfGameTriggers()
        {
            AddTrigger((GameAction ga) => TurnTakerController is CricketTurnTakerController ttc && !ttc.ArePromosSetup, SetupPromos, TriggerType.Hidden, TriggerTiming.Before, priority: TriggerPriority.High);
        }

        public IEnumerator SetupPromos(GameAction ga)
        {
            if (TurnTakerController is CricketTurnTakerController ttc && !ttc.ArePromosSetup)
            {
                ttc.SetupPromos(ttc.availablePromos);
                ttc.ArePromosSetup = true;
            }

            return DoNothing();
        }

    }
}