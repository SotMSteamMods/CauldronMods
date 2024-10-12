using System;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Linq;

namespace Cauldron.TheStranger
{
    public class GlyphOfPerceptionCardController : GlyphCardController
    {
        #region Constructors

        public GlyphOfPerceptionCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        #endregion Constructors

        #region Methods
        public override void AddTriggers()
        {
            base.AddTriggers();
            //When a villain target enters play, you may play a Rune.
            base.AddTargetEntersPlayTrigger((Card c) => base.IsVillainTarget(c), (Card c) => base.SelectAndPlayCardFromHand(base.HeroTurnTakerController, true, null, IsRuneCriteria(), false, false, true, null), TriggerType.PlayCard, TriggerTiming.After, false, false);
        }
        #endregion Methods
    }
}
