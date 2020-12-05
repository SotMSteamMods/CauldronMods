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

        protected override IEnumerator SelectHeroAndGrantReward(List<SelectTurnTakerDecision> selected)
        {
            return GameController.SelectHeroToDrawCard(DecisionMaker, optionalSelectHero: true, optionalDrawCard: false, storedResults: selected, cardSource: GetCardSource());
        }
    }
}