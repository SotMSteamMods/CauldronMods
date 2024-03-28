using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.VaultFive
{
    public class SigQuilsSignCardController : ArtifactCardController
    {
        public SigQuilsSignCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator UniqueOnPlayEffect()
        {
            //increase damage dealt to and by targets from its deck by 1 until the start of their next turn
            IncreaseDamageStatusEffect effect1 = new IncreaseDamageStatusEffect(1);
            effect1.SourceCriteria.NativeDeck = GetNativeDeck(this.Card);
            effect1.SourceCriteria.OutputString = $"targets from {Card.Owner.Name}'s deck";
            effect1.UntilStartOfNextTurn(Card.Owner);
            IEnumerator addEffect1 = AddStatusEffect(effect1);

            IncreaseDamageStatusEffect effect2 = new IncreaseDamageStatusEffect(1);
            effect2.TargetCriteria.NativeDeck = GetNativeDeck(this.Card);
            effect2.TargetCriteria.OutputString = $"targets from {Card.Owner.Name}'s deck";
            effect2.UntilStartOfNextTurn(Card.Owner);
            IEnumerator addEffect2 = AddStatusEffect(effect2);

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(addEffect1);
                yield return base.GameController.StartCoroutine(addEffect2);

            }
            else
            {
                base.GameController.ExhaustCoroutine(addEffect1);
                base.GameController.ExhaustCoroutine(addEffect2);
            }
            yield break;
        }
    }
}
