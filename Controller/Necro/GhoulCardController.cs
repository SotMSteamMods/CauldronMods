using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Linq;

namespace SotMWorkshop.Controller.Necro
{
	public class GhoulCardController : CardController
    {
		public GhoulCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
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
			//At the end of your turn, this card deals the non-undead target with the second lowest HP 2 toxic damage.
			base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, (PhaseChangeAction p) => base.DealDamageToLowestHP(base.Card, 2, (Card c) => !this.IsUndead(c), (Card c) => new int?(2), DamageType.Toxic, false, false, null, 1, null, null, false), TriggerType.DealDamage, null, false);
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
