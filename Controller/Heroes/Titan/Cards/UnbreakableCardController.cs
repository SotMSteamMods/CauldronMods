using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

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
            base.AddTrigger<PhaseChangeAction>((PhaseChangeAction action) => action.ToPhase.IsEnd && action.ToPhase.IsVillain && base.GameController.IsTurnTakerVisibleToCardSource(action.ToPhase.TurnTaker, base.GetCardSource()), (PhaseChangeAction action) => base.CancelAction(action), TriggerType.CancelAction, TriggerTiming.Before);
            //You may not use powers.
            base.CannotUsePowers((TurnTakerController ttc) => ttc == base.HeroTurnTakerController);
            //You may not draw cards.
            base.CannotDrawCards((TurnTakerController ttc) => ttc == base.HeroTurnTakerController);
            //At the start of your turn, destroy this card.
            base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.DestroyThisCardResponse, TriggerType.DestroySelf);
        }
    }
}