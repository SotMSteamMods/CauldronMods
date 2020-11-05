using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Linq;
using System;

namespace Cauldron.Necro
{
	public class AbominationCardController : CardController
    {
		public AbominationCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}
		public override IEnumerator Play()
		{
			//When this card enters play, # = the number of rituals in play plus 6.
			this.Card.SetMaximumHP(this.GetNumberOfRitualsInPlay() + 6, true);

			yield break;
		}

		public override void AddTriggers()
		{
			//At the end of your turn, this card deals all non-Undead hero targets 2 toxic damage.        
			base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, (PhaseChangeAction p) => (base.DealDamage(this.Card, (Card card) => !this.IsUndead(card) && card.IsHero, 2, DamageType.Toxic, false, false, null, null, null, false, null, null, false, false)), TriggerType.DealDamage, null, false);
			//When this card is destroyed, all players draw a card.
			base.AddWhenDestroyedTrigger(new Func<DestroyCardAction, IEnumerator>(this.OnDestroyResponse), new TriggerType[] {TriggerType.PlayCard}, null, null);
		}

		private IEnumerator OnDestroyResponse(DestroyCardAction dca)
		{
			//all players draw a card.
			IEnumerator coroutine = base.EachPlayerDrawsACard((HeroTurnTaker tt) => true, false, true, null, true, false);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}
			yield break;
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
