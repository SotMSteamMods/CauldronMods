using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections.Generic;
using System.Linq;
using Cauldron.Impact;

namespace Cauldron.Echelon
{
    public abstract class EchelonBaseCardController : CardController
    {
        public EchelonBaseCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddStartOfGameTriggers()
        {
            AddTrigger((GameAction ga) => TurnTakerController is EchelonTurnTakerController ttc && !ttc.ArePromosSetup, SetupPromos, TriggerType.Hidden, TriggerTiming.Before, priority: TriggerPriority.High);
        }

        public IEnumerator SetupPromos(GameAction ga)
        {
            if (TurnTakerController is EchelonTurnTakerController ttc && !ttc.ArePromosSetup)
            {
                ttc.SetupPromos(ttc.availablePromos);
                ttc.ArePromosSetup = true;
            }

            return DoNothing();
        }

        protected bool IsTactic(Card card)
        {
            return card != null && base.GameController.DoesCardContainKeyword(card, "tactic");
        }

        protected int GetDestroyedTacticsThisTurn()
        {
            // TODO
            return 0;
        }

    }
}