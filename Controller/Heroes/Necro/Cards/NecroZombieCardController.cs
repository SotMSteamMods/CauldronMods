using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Linq;

namespace Cauldron.Necro
{
	public class NecroZombieCardController : UndeadCardController
	{
		public NecroZombieCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}
		public override IEnumerator Play()
		{
			//When this card enters play, # = the number of rituals in play plus 2.
			SetMaximumHPWithRituals(2);

			yield break;
		}

		public override void AddTriggers()
		{
			//At the end of your turn, this card deals the non-Undead hero target with the highest HP 2 toxic damage.
			base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, (PhaseChangeAction p) => base.DealDamageToHighestHP(base.Card, 1, (Card c) => !this.IsUndead(c) && c.IsHero, (Card c) => new int?(2), DamageType.Toxic, false, false, null, null, null, null, false), TriggerType.DealDamage, null, false);
		}
	}
}
