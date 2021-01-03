using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Malichae
{
	public class UnshackledCardController : MalichaeCardController
	{
		public UnshackledCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
            AddThisCardControllerToList(CardControllerListType.IncreasePhaseActionCount);
		}

        public override void AddTriggers()
        {
            base.AddAdditionalPhaseActionTrigger(tt => tt == this.TurnTaker, Phase.UsePower, 1);
        }
    }
}
