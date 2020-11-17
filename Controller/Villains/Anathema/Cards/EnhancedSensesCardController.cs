using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Linq;

namespace Cauldron.Anathema
{
	public class EnhancedSensesCardController : HeadCardController
    {
		public EnhancedSensesCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

		public override void AddTriggers()
		{
			//Change the type of all damage to sonic.
			base.AddChangeDamageTypeTrigger((DealDamageAction dd) => true, DamageType.Sonic);
			//Whenever a Hero card enters play, this card deals that Hero Character 1 sonic damage.
			base.AddTrigger<PlayCardAction>((PlayCardAction pca) => pca.CardToPlay.IsHero,
				(PlayCardAction pc) => base.DealDamage(base.Card, pc.CardToPlay.Owner.CharacterCard, 1, DamageType.Sonic, false, false, false, null, null, null, false, base.GetCardSource(null)),
				new TriggerType[] { TriggerType.DealDamage }, TriggerTiming.After, null, false, true, null, false, null, null, false, false);		
		}

	}
}
