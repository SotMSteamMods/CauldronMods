using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheChasmOfAThousandNights
{
    public abstract class TheChasmOfAThousandNightsUtilityCardController : CardController
    {
        protected TheChasmOfAThousandNightsUtilityCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public static readonly string IreOfTheDjinnIdentifier = "IreOfTheDjinn";

        private IEnumerable<Card> FindIreOfTheDjinn()
        {
            return base.FindCardsWhere(c => c.Identifier == IreOfTheDjinnIdentifier);
        }

        protected bool IsIreOfTheDjinnInPlay()
        {
            return FindIreOfTheDjinn().Where(c => c.IsInPlayAndHasGameText).Any();
        }
    }
}
