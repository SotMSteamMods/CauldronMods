using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.ScreaMachine
{
    public class MentalLinkCardController : ScreaMachineBandCardController
    {
        public MentalLinkCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, ScreaMachineBandmate.Value.Valentine)
        {
        }

        protected override IEnumerator ActivateBandAbility()
        {

            var topCard = TurnTaker.Deck.TopCard;
            var cc = FindCardController(topCard);

            GameController.AddInhibitor(cc);
            GameController.AddInhibitorException(cc, ga => !(ga is ActivateAbilityAction));

            var coroutine = GameController.PlayTopCard(DecisionMaker, TurnTakerController, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            GameController.RemoveInhibitorException(cc);
            GameController.RemoveInhibitor(cc);
        }
    }
}
