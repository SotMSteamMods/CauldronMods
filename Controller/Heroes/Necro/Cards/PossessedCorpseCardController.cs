using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Linq;

namespace Cauldron.Necro
{
	public class PossessedCorpseCardController : UndeadCardController
	{
		public PossessedCorpseCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}
		public override IEnumerator Play()
		{
			//When this card enters play, # = the number of rituals in play plus 2.
			SetMaximumHPWithRituals(2);

			yield break;
		}
	}
}
