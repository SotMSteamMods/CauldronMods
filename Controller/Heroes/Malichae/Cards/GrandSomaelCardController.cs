using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Malichae
{
	public class GrandSomaelCardController : DjinnOngoingController
	{
		public GrandSomaelCardController(Card card, TurnTakerController turnTakerController)
			: base(card, turnTakerController, "HighSomael", "Somael")
		{
		}
	}
}
