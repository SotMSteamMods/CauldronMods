using Handelabra;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace Cauldron.Necro
{
	public class BackfireHexCardController : NecroCardController
	{
		public BackfireHexCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
			base.SpecialStringMaker.ShowNumberOfCardsAtLocation(base.TurnTaker.Trash, new LinqCardCriteria((Card c) => this.IsUndead(c), "undead", true, false, null, null, false), null, false);
		}
		public override IEnumerator Play()
		{
			//You may destroy an ongoing card.
			IEnumerator coroutine = base.GameController.SelectAndDestroyCard(base.HeroTurnTakerController, new LinqCardCriteria((Card c) => c.IsOngoing, "ongoing", true, false, null, null, false), true, null, null, base.GetCardSource(null));          
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}

			//Put an undead card from the trash into play.

			MoveCardDestination obj = new MoveCardDestination(base.TurnTaker.PlayArea, false, false, false);
			IEnumerator coroutine2 = base.GameController.SelectCardFromLocationAndMoveIt(this.DecisionMaker, base.TurnTaker.Trash, new LinqCardCriteria((Card c) => this.IsUndead(c), "undead", true, false, null, null, false), obj.ToEnumerable<MoveCardDestination>(), true, true, false, false, null, false, true, null, false, false, null, null, base.GetCardSource(null)); 
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine2);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine2);
			}
			yield break;
		}
	}
}
