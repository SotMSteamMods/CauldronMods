using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Anathema
{
    public class AnathemaCardController : CardController
    {

        public AnathemaCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

		protected bool IsArm(Card card)
		{
			return card != null && base.GameController.DoesCardContainKeyword(card, "arm");
		}

		protected bool IsHead(Card card)
		{
			return card != null && base.GameController.DoesCardContainKeyword(card, "head");
		}

		protected bool IsBody(Card card)
		{
			return card != null && base.GameController.DoesCardContainKeyword(card, "body");
		}

		protected int GetNumberOfArmsInPlay()
		{
			return base.FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && this.IsArm(c)).Count();
		}

		protected int GetNumberOfHeadInPlay()
		{
			return base.FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && this.IsHead(c)).Count();
		}

		protected IEnumerable<Card> GetHeadsInPlay()
		{
			return base.FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && this.IsHead(c));
		}

		protected IEnumerable<Card> GetBodiesInPlay()
		{
			return base.FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && this.IsBody(c));
		}

		protected IEnumerable<Card> GetArmsInPlay()
		{
			return base.FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && this.IsArm(c));
		}

		protected int GetNumberOfBodyInPlay()
		{
			return base.FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && this.IsBody(c)).Count();
		}

		protected bool IsArmHeadOrBody(Card c)
		{
			return IsArm(c) || IsHead(c) || IsBody(c);
		}

	}
}