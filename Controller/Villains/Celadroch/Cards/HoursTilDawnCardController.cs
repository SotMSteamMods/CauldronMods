using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Celadroch
{
    public class HoursTilDawnCardController : CeladrochOngoingCardController
    {
        /*
         *  "When this card enters play, play the top card of the villain deck.",
			"At the end of the villain turn, each villain target regains 2HP.",
			"When this card is destroyed, {Celadroch} regains 10HP."
         */

        public HoursTilDawnCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            return base.Play();
        }

        public override void AddTriggers()
        {
            AddEndOfTurnTrigger(tt => tt == TurnTaker, pca => GameController.GainHP(DecisionMaker, c => IsVillainTarget(c), 2, cardSource: GetCardSource()), TriggerType.GainHP);

            AddWhenDestroyedTrigger(dca => GameController.GainHP(TurnTaker.CharacterCard, 10, cardSource: GetCardSource()), TriggerType.GainHP);
        }

    }
}