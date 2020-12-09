using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;

namespace Cauldron
{
    public class PhaseCardController : CardController
    {
        public PhaseCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public bool IsObstacle(Card c)
        {
            return c.DoKeywordsContain("Obstacle");
        }
    }
}