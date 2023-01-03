using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace Cauldron.Impact
{
    public class ImpactSubCharacterCardController : HeroCharacterCardController
    {
        public ImpactSubCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddStartOfGameTriggers()
        {
            AddTrigger((GameAction ga) => TurnTakerController is ImpactTurnTakerController ttc && !ttc.ArePromosSetup, SetupPromos, TriggerType.Hidden, TriggerTiming.Before, priority: TriggerPriority.High);
        }

        public IEnumerator SetupPromos(GameAction ga)
        {
            if (TurnTakerController is ImpactTurnTakerController ttc && !ttc.ArePromosSetup)
            {
                ttc.SetupPromos(ttc.availablePromos);
                ttc.ArePromosSetup = true;
            }

            return DoNothing();
        }
    }
}