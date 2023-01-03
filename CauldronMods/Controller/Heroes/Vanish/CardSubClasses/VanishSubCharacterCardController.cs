using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;

namespace Cauldron.Vanish
{
    public class VanishSubCharacterCardController : HeroCharacterCardController
    {
        public VanishSubCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddStartOfGameTriggers()
        {
            AddTrigger((GameAction ga) => TurnTakerController is VanishTurnTakerController ttc && !ttc.ArePromosSetup, SetupPromos, TriggerType.Hidden, TriggerTiming.Before, priority: TriggerPriority.High);
        }

        public IEnumerator SetupPromos(GameAction ga)
        {
            if (TurnTakerController is VanishTurnTakerController ttc && !ttc.ArePromosSetup)
            {
                ttc.SetupPromos(ttc.availablePromos);
                ttc.ArePromosSetup = true;
            }

            return DoNothing();
        }
    }
}