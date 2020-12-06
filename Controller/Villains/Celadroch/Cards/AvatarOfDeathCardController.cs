﻿using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Celadroch
{
    public class AvatarOfDeathCardController : CardController
    {
        /*
         *  "At the start of the villain turn, destroy 1 hero ongoing card.",
			"Reduce damage dealt to this card by 1.",
			"At the end of the villain turn, this card deals each hero target {H} projectile damage."
         */
        public AvatarOfDeathCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }
    }
}