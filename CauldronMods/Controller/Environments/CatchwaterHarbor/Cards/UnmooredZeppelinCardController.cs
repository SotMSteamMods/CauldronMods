using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.CatchwaterHarbor
{
    public class UnmooredZeppelinCardController : TransportCardController
    {
        public UnmooredZeppelinCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator ActivateTravel()
        {
            //Increase all damage dealt by 1 until the start of the next environment turn.
            IncreaseDamageStatusEffect effect = new IncreaseDamageStatusEffect(1);
            effect.UntilStartOfNextTurn(TurnTaker);
            IEnumerator coroutine = AddStatusEffect(effect);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }
    }
}
