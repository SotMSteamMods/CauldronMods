using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Linq;

namespace Cauldron.Phase
{
    public class FrequencyShiftCardController : PhaseCardController
    {
        public FrequencyShiftCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        //{Phase} regains X HP, where X is the number of Obstacle cards in play.
        //{Phase} deals the hero target with the highest HP {H} irreducible radiant damage and destroys 1 ongoing and 1 equipment card belonging to that hero.
        public override IEnumerator Play()
        {
            Func<int> X = () => base.FindCardsWhere(new LinqCardCriteria((Card c) => base.IsObstacle(c))).Count();
        }
    }
}