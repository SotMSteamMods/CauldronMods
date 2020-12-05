using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Celadroch
{
    public class PillarOfStormsCardController : CeladrochPillarCardController
    {
        public PillarOfStormsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, TriggerType.PlayCard)
        {

        }

        protected override IEnumerator SelectHeroAndGrantReward(List<SelectTurnTakerDecision> selected)
        {
            return GameController.SelectHeroToPlayCard(DecisionMaker, optionalSelectHero: true, optionalPlayCard: false, storedResultsTurnTaker: selected, cardSource: GetCardSource());
        }
    }
}
