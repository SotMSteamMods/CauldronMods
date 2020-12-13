using System;
using System.Collections;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Cypher
{
    public class RetinalAugCardController : AugBaseCardController
    {
        //==============================================================
        // Play this card next to a hero. The hero next to this card is augmented.
        // During their play phase, that hero may play an additional card.
        //==============================================================

        public static string Identifier = "RetinalAug";

        public RetinalAugCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            AddThisCardControllerToList(CardControllerListType.IncreasePhaseActionCount);
        }

        public override void AddTriggers()
        {
            //They may play an additional card during their play phase.
            base.AddAdditionalPhaseActionTrigger(this.ShouldIncreasePhaseActionCount, Phase.PlayCard, 1);

            base.AddTriggers();
        }

        private bool ShouldIncreasePhaseActionCount(TurnTaker tt)
        {
            return tt == base.GetCardThisCardIsNextTo(true).Owner;
        }
    }
}