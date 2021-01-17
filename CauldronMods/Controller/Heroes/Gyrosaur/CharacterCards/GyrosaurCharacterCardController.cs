﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Gyrosaur
{
    public class GyrosaurCharacterCardController : GyrosaurUtilityCharacterCardController
    {
        public GyrosaurCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
        public override IEnumerator UsePower(int index = 0)
        {
            //"If you have at least 2 crash cards in your had, {Gyrosaur} deals up to 3 targets 1 melee damage each. If not, draw a card."
            yield break;
        }

        public override IEnumerator UseIncapacitatedAbility(int index)
        {
            IEnumerator coroutine;
            switch (index)
            {
                case 0:
                    {
                        //"One player may draw a card now.",
                        break;
                    }
                case 1:
                    {
                        //"One target with more than 10 HP deals itself 3 melee damage.",
                        break;
                    }
                case 2:
                    {
                        //"Select a non-character target. Increase damage dealt to that target by 1 until the start of your turn."
                        break;
                    }
            }
            yield break;
        }
    }
}
