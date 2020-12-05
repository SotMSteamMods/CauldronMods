using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Celadroch
{
    public class PillarOfSkyCardController : CeladrochPillarCardController
    {
        public PillarOfSkyCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, TriggerType.UsePower)
        {

        }

        protected override IEnumerator SelectHeroAndGrantReward()
        {
            return GameController.SelectHeroToUsePower(DecisionMaker, optionalSelectHero: true, optionalUsePower: false, cardSource: GetCardSource());
        }
    }
}