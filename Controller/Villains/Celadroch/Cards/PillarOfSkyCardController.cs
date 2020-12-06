using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Celadroch
{
    public class PillarOfSkyCardController : CeladrochPillarCardController
    {
        /*
         * 	"This card may not regain HP. Reduce damage dealt to {Celadroch} by 1.",
			"For every {H + 1} HP this card loses, including when it reaches 0HP, 1 hero may immediately use a power. When this card would leave play, remove it from the game."
         */

        public PillarOfSkyCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, TriggerType.UsePower)
        {

        }

        protected override IEnumerator SelectHeroAndGrantReward(List<SelectTurnTakerDecision> selected)
        {
            return GameController.SelectHeroToUsePower(DecisionMaker, optionalSelectHero: true, optionalUsePower: false, storedResultsDecision: selected, cardSource: GetCardSource());
        }
    }
}