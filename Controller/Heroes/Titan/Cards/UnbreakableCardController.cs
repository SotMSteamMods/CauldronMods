using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Linq;

namespace Cauldron.Titan
{
    public class UnbreakableCardController : CardController
    {
        public UnbreakableCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //Skip any effects which would act at the end of the villain turn.
            base.AddInhibitorException((GameAction ga) => true);
            base.GameController.AddTemporaryTriggerInhibitor<PhaseChangeAction>((ITrigger trigger) => trigger is PhaseChangeTrigger && (trigger as PhaseChangeTrigger).PhaseCriteria(Phase.End) && base.Game.ActiveTurnTaker.IsVillain && (trigger as PhaseChangeTrigger).TurnTakerCriteria(base.Game.ActiveTurnTaker), (PhaseChangeAction action) => !base.Card.Location.IsInPlayAndNotUnderCard, base.GetCardSource());
            //You may not use powers.
            base.CannotUsePowers((TurnTakerController ttc) => ttc == base.HeroTurnTakerController);
            //You may not draw cards.
            base.CannotDrawCards((TurnTakerController ttc) => ttc == base.HeroTurnTakerController);
            //At the start of your turn, destroy this card.
            base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.DestroyThisCardResponse, TriggerType.DestroySelf);
        }
    }
}
