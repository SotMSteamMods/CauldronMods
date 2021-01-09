using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.TheChasmOfAThousandNights
{
    public class NatureCardController : TheChasmOfAThousandNightsUtilityCardController
    {

        public NatureCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
        }

        public override bool AskIfCardIsIndestructible(Card card)
        {
            //This card is indestructible.
            return card == Card;
        }

    }
}