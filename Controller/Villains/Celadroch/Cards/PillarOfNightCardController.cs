using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Celadroch
{
    public class PillarOfNightCardController : CeladrochPillarCardController
    {
        public PillarOfNightCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, TriggerType.DrawCard)
        {

        }

        protected override IEnumerator SelectHeroAndGrantReward()
        {
            return GameController.SelectHeroToDrawCard(DecisionMaker, optionalSelectHero: true, optionalDrawCard: false, cardSource: GetCardSource());
        }
    }
}