using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.ScreaMachine
{
    public abstract class ScreaMachineUtilityCardController : CardController
    {
        protected ScreaMachineUtilityCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        protected Card SliceCharacter => base.FindCard("SliceCharacter");

        protected Card ValentineCharacter => base.FindCard("ValentineCharacter");

        protected Card BloodlaceCharacter => base.FindCard("BloodlaceCharacter");

        protected Card RickyGCharacter => base.FindCard("RickyGCharacter");

        protected Card TheSetList => base.FindCard("TheSetList");

        protected TheSetListCardController TheSetListCardController => base.FindCardController(TheSetList) as TheSetListCardController;




    }
}
