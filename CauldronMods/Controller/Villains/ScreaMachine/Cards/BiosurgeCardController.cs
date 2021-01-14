using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.ScreaMachine
{
    public class BiosurgeCardController : ScreaMachineBandCardController
    {
        public BiosurgeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, ScreaMachineBandmate.Value.Bloodlace)
        {
        }

        protected override IEnumerator ActivateBandAbility()
        {
            var card = GetBandmate();
            var effect = new IncreaseDamageStatusEffect(2);
            effect.SourceCriteria.IsSpecificCard = card;
            effect.NumberOfUses = 1;
            effect.CardSource = Card;
            effect.UntilTargetLeavesPlay(card);

            var coroutine = AddStatusEffect(effect);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
        }
    }
}
