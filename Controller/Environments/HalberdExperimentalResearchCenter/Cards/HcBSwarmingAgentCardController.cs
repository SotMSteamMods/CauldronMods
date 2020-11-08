using System;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.HalberdExperimentalResearchCenter
{
    public class HcBSwarmingAgentCardController : ChemicalTriggerCardController
    {
        #region Constructors

        public HcBSwarmingAgentCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        #endregion Constructors

        #region Methods
        public override void AddTriggers()
        {
            base.AddTriggers();
        }
        #endregion Methods
    }
}