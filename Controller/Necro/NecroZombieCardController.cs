using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Linq;

namespace Cauldron.Necro
{
	public class NecroZombieCardController : CardController
    {
		public NecroZombieCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}
		public override IEnumerator Play()
		{
			//When this card enters play, # = the number of rituals in play plus 2.
			this.Card.SetMaximumHP(this.GetNumberOfRitualsInPlay() + 2, true);

			yield break;
		}

		public override void AddTriggers()
		{
			//At the end of your turn, this card deals the non-Undead hero target with the highest HP 2 toxic damage.
			base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, (PhaseChangeAction p) => base.DealDamageToHighestHP(base.Card, 1, (Card c) => !this.IsUndead(c) && c.IsHero, (Card c) => new int?(2), DamageType.Toxic, false, false, null, null, null, null, false), TriggerType.DealDamage, null, false);
		}

		private bool IsRitual(Card card)
		{
			return card != null && this.GameController.DoesCardContainKeyword(card, "ritual", false, false);
		}

		private int GetNumberOfRitualsInPlay()
		{
			return base.FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && this.IsRitual(c), false, null, false).Count<Card>();
		}

		private bool IsUndead(Card card)
		{
			return card != null && base.GameController.DoesCardContainKeyword(card, "undead", false, false);
		}
	}
}
