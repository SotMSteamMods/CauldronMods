using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Linq;
using System;

namespace Cauldron.Necro
{
	public class DemonicImpCardController : UndeadCardController
	{
		public DemonicImpCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
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
			//At the end of your turn, destroy 1 hero equipment or ongoing card.          
			base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, (PhaseChangeAction p) => (base.GameController.SelectAndDestroyCards(this.DecisionMaker, new LinqCardCriteria((Card c) => c.IsHero && (c.IsOngoing || base.IsEquipment(c)), "hero ongoing or equipment", true, false, null, null, false), new int?(1), false, null, null, null, null, false, null, null, null, base.GetCardSource(null))), TriggerType.DestroyCard, null, false);
			//When this card is destroyed, one player may play a card.
			base.AddWhenDestroyedTrigger(new Func<DestroyCardAction, IEnumerator>(this.OnDestroyResponse), new TriggerType[] {TriggerType.PlayCard}, null, null);
		}

		private IEnumerator OnDestroyResponse(DestroyCardAction dca)
		{
			//one player may play a card.
			IEnumerator coroutine = base.SelectHeroToPlayCard(this.DecisionMaker, false, true, false, null, null, null, false, true);
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
	}
}
