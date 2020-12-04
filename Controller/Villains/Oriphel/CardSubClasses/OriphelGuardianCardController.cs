using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Oriphel
{
    public class OriphelGuardianCardController : OriphelUtilityCardController
    {
        public OriphelGuardianCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //"When this card is destroyed, reveal cards from the top of the villain deck until a Relic is revealed and play it. 
            
            //Shuffle the remaining cards into the villain deck."
        }
    }
}