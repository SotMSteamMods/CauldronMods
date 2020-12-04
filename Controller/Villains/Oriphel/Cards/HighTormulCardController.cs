using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Oriphel
{
    public class HighTormulCardController : OriphelGuardianCardController
    {
        public HighTormulCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //Guardian destroy trigger
            base.AddTriggers();

            //"At the start of each hero turn, this card deals that hero 2 toxic damage.",

        }
    }
}