using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Celadroch
{
    public class PillarOfStormsCardController : CeladrochPillarCardController
    {
        /*
         * 	"This card may not regain HP. Reduce damage dealt to {Celadroch} by 1. ",
			"For every {H + 1} HP this card loses, including when it reaches 0HP, 1 player may immediately play a card. When this card would leave play, remove it from the game."
         */

        public PillarOfStormsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, TriggerType.PlayCard)
        {

        }

        protected override IEnumerator SelectHeroAndGrantReward(List<SelectTurnTakerDecision> selected)
        {
            return GameController.SelectHeroToPlayCard(DecisionMaker, optionalSelectHero: true, optionalPlayCard: false, storedResultsTurnTaker: selected, cardSource: GetCardSource());
        }
    }
}
