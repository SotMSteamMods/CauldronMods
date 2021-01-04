﻿using System;
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
            effect1.SourceCriteria.NativeDeck = Card.NativeDeck;
            effect1.UntilStartOfNextTurn(Card.Owner);
            IEnumerator addEffect1 = AddStatusEffect(effect1);
            IncreaseDamageStatusEffect effect2 = new IncreaseDamageStatusEffect(1);
            effect2.TargetCriteria.NativeDeck = Card.NativeDeck;
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
