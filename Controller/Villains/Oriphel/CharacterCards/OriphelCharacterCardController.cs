using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Oriphel
{
    public class OriphelCharacterCardController : VillainCharacterCardController
    {
        public OriphelCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddSideTriggers()
        {
            if(!Card.IsFlipped)
            {
                //"Whenever a villain relic enters play, play the top card of the villain deck.",
                //"Whenever a villain ongoing card enters play, destroy it and play the top card of the villain deck."

            }
            else
            {
                //"When {Oriphel} is flipped to this side, shuffle all relics in the villain trash into the villain deck.",
                //"Reduce damage dealt to {Oriphel} by 1.",
                //"At the end of the villain turn, {Oriphel} deals the 2 hero targets with the highest HP {H - 1} infernal damage each.",
                //"When there are 2 villain relics in the villain trash, flip {Oriphel}'s villain character cards."

            }
            AddDefeatedIfDestroyedTriggers();
        }
    }
}