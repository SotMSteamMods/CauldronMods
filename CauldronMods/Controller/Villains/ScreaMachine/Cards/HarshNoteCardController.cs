using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.ScreaMachine
{
    public class HarshNoteCardController : ScreaMachineUtilityCardController
    {
        public HarshNoteCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerable<ScreaMachineBandmate.Value> AbilityIcons => new[] { ScreaMachineBandmate.Value.Slice, ScreaMachineBandmate.Value.Valentine };

        public override IEnumerator Play()
        {
            var effect = new IncreaseDamageStatusEffect(H);
            effect.NumberOfUses = 1;
            effect.SourceCriteria.IsVillain = true;
            effect.SourceCriteria.IsTarget = true;
            effect.CardSource = Card;

            var coroutine = AddStatusEffect(effect);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = AcivateBandAbilities(AbilityIcons);
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
