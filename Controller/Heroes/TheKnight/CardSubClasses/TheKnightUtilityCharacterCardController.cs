using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.TheKnight
{
    public class TheKnightUtilityCharacterCardController : HeroCharacterCardController
    {
        protected bool IsCoreCharacterCard = true;
        public TheKnightUtilityCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddStartOfGameTriggers()
        {
            if (IsCoreCharacterCard)
            {
                (TurnTakerController as TheKnightTurnTakerController).ManageCharactersOffToTheSide(true);
            }
        }
    }
}