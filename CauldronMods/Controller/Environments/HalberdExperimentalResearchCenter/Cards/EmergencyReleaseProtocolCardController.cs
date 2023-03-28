using System;
using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.HalberdExperimentalResearchCenter
{
    public class EmergencyReleaseProtocolCardController : CardController
    {
        #region Constructors

        public EmergencyReleaseProtocolCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        #endregion Constructors

        #region Methods
        public override void AddTriggers()
        {

            //At the end of the environment turn, play the top card of the environment deck
            base.AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, (PhaseChangeAction action) => base.PlayTheTopCardOfTheEnvironmentDeckWithMessageResponse(action), TriggerType.PlayCard);
            // At the start of their turn, a player may skip the rest of their turn to destroy this card.
            base.AddStartOfTurnTrigger((TurnTaker tt) => IsHero(tt), new Func<PhaseChangeAction, IEnumerator>(base.SkipTheirTurnToDestroyThisCardResponse), new TriggerType[]
            {
                TriggerType.SkipTurn,
                TriggerType.DestroySelf
            });
        }
        #endregion Methods
    }
}