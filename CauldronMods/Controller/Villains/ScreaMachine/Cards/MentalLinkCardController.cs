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

        private bool _alreadyPlayingCard = false;

        protected override IEnumerator ActivateBandAbility()
        {
            // Development notes:
            // so initial investigation focused on usingthe AddInhibitor/AddInhibitorException methods
            // This failed becuse those conditions for those is tied to GameContoller.AllowInhibitors and that prop
            // is never true, it's only set to true during certain trigger resolution.
            // AskIfActionCanBePerformed is the answer.  Since this prevention only occurs while the card play is in
            // flight the local variable doesn't have resume/continue concerns.

            _alreadyPlayingCard = true;

            var coroutine = GameController.PlayTopCard(DecisionMaker, TurnTakerController, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            _alreadyPlayingCard = false;
        }

        public override bool AskIfActionCanBePerformed(GameAction gameAction)
        {
            if (_alreadyPlayingCard && gameAction is ActivateAbilityAction aa && ScreaMachineBandmate.AbilityKeys.Contains(aa.ActivatableAbility.AbilityKey))
            {
                return false;
            }

            return base.AskIfActionCanBePerformed(gameAction);
        }
    }
}
