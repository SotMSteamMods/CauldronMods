using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Malichae
{
	public class HighReshielCardController : DjinnOngoingController
	{
		public HighReshielCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController, "Reshiel", "Reshiel")
		{
		}
	}
}
