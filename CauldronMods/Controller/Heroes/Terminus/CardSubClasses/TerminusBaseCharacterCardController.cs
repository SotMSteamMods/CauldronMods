using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Terminus
{
    public class TerminusBaseCharacterCardController : HeroCharacterCardController
    {
        public TerminusBaseCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            CardWithoutReplacements.TokenPools.ReorderTokenPool("TerminusWrathPool");
            base.SpecialStringMaker.ShowTokenPool(base.Card.FindTokenPool("TerminusWrathPool"));
            //base.AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
        }
    }
}
