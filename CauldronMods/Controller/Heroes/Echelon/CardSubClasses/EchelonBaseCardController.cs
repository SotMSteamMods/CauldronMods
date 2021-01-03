using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Echelon
{
    public abstract class EchelonBaseCardController : CardController
    {
        public EchelonBaseCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        protected bool IsTactic(Card card)
        {
            return card != null && base.GameController.DoesCardContainKeyword(card, "tactic");
        }

        protected int GetDestroyedTacticsThisTurn()
        {
            // TODO
            return 0;
        }

    }
}