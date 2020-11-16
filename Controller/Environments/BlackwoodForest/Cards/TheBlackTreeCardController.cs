using System;
using System.Collections;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace Cauldron.BlackwoodForest
{
    public class TheBlackTreeCardController : CardController
    {
        //==============================================================
        // When this card enters play, place the top card of each hero
        // and villain deck face-down beneath it.
        // At the end of the environment turn, play a random card from
        // beneath this one. Then if there are no cards remaining, this card is destroyed.
        // When this card is destroyed, discard any remaining cards beneath it.
        //==============================================================

        public static string Identifier = "TheBlackTree";

        public TheBlackTreeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            // At the end of the environment turn, play a random card from beneath this one.
            // Then if there are no cards remaining, this card is destroyed.
            base.AddEndOfTurnTrigger(tt => tt == base.TurnTaker, EndOfTurnPlayCardBeneathResponse,
                TriggerType.PlayCard, null, false);

            // When this card is destroyed, discard any remaining cards beneath it.
            base.AddWhenDestroyedTrigger(DestroyCardResponse, TriggerType.DestroyCard);

            base.AddTriggers();
        }

        public override IEnumerator Play()
        {
            return base.Play();
        }

        private IEnumerator EndOfTurnPlayCardBeneathResponse(PhaseChangeAction pca)
        {
            yield break;
        }

        private IEnumerator DestroyCardResponse(DestroyCardAction dca)
        {
            yield break;
        }
    }
}