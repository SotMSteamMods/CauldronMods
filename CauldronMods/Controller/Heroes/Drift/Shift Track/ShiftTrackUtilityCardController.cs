using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Drift
{
    public abstract class ShiftTrackUtilityCardController : DriftBaseCardController
    {
        protected ShiftTrackUtilityCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddStartOfGameTriggers()
        {
            if (Card.IsInPlay && Card.IsFlipped)
            {
                AddFlipTriggers();
            }
        }

        public override void AddTriggers()
        {
            base.AddTriggers();
            AddFlipTriggers();
        }

        private void AddFlipTriggers()
        {
            AddTrigger((FlipCardAction fc) => fc.CardToFlip != null && fc.CardToFlip.Card == GetActiveCharacterCard(), FlipThisCardResponse, TriggerType.Hidden, TriggerTiming.After);
        }

        public override IEnumerator AfterFlipCardImmediateResponse()
        {
            if (Card.IsFlipped)
            {
                RemoveAllTriggers(includingOutOfPlay: true, includeUnresolvedOnDestroyTriggers: false);
                AddFlipTriggers();
            }
            else if (Card.IsInPlay)
            {
                RemoveInhibitor();
                AddCardTriggers();
            }
            yield return null;
        }

        private IEnumerator FlipThisCardResponse(FlipCardAction fc)
        {
            if (Card.IsFlipped != CharacterCard.IsFlipped)
            {
                return GameController.FlipCard(this, cardSource: GetCardSource());
            }
            return DoNothing();
        }
    }
}
