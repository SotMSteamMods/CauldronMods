using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;

namespace Cauldron.PhaseVillain
{
    public abstract class PhaseVillainCardController : CardController
    {
        protected  PhaseVillainCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public bool IsObstacle(Card c)
        {
            return c.DoKeywordsContain("obstacle");
        }
    }
}