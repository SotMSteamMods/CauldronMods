using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace CauldronMods.Controller.Villains.Vector.CardSubClasses
{
    public class VectorBaseCardController : VillainCharacterCardController
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
    }
}
