using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Vector
{
    public class VectorBaseCardController : CardController
    {
        protected VectorBaseCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        protected bool IsVirus(Card card)
        {
            return card != null && base.GameController.DoesCardContainKeyword(card, "virus");
        }

        protected bool IsVehicle(Card card)
        {
            return card != null && base.GameController.DoesCardContainKeyword(card, "vehicle");
        }

        protected bool IsPawn(Card card)
        {
            return card != null && base.GameController.DoesCardContainKeyword(card, "pawn");
        }

        protected bool IsSuperVirusInPlay()
        {
            return base.GameController.IsCardInPlayAndNotUnderCard(SupervirusCardController.Identifier);
        }

        protected Card GetSuperVirusCard()
        {
            return FindCardsWhere(card => card.Identifier == SupervirusCardController.Identifier 
                                && card.IsInPlayAndHasGameText).FirstOrDefault();
        }

        protected bool ShouldVectorFlip()
        {
            if (!IsSuperVirusInPlay())
            {
                return false;
            }

            int cardFlipThreshold = base.Game.H + 2;

            Card superVirus = GetSuperVirusCard();
            return superVirus.UnderLocation.Cards.Count() >= cardFlipThreshold;
        }
    }
}
