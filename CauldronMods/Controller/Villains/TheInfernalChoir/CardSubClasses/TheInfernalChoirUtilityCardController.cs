using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheInfernalChoir
{
    public abstract class TheInfernalChoirUtilityCardController : CardController
    {
        protected TheInfernalChoirUtilityCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        protected bool IsGhost(Card c, bool evenIfUnderCard = false, bool evenIfFaceDown = false)
        {
            return c != null && (c.DoKeywordsContain("ghost", evenIfUnderCard, evenIfFaceDown) || GameController.DoesCardContainKeyword(c, "ghost", evenIfUnderCard, evenIfFaceDown));
        }
    }
}
