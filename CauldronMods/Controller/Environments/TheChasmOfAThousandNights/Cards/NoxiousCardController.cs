using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheChasmOfAThousandNights
{
    public class NoxiousCardController : NatureCardController
    {
        public NoxiousCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //At the end of the environment turn, the target next to this card deals each hero target 1 toxic damage.
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, DealDamageResponse, TriggerType.DealDamage, additionalCriteria: (PhaseChangeAction pca) => GetCardThisCardIsBelow() != null);
            base.AddTriggers();
        }

        private IEnumerator DealDamageResponse(PhaseChangeAction arg)
        {
            IEnumerator coroutine = DealDamage(GetCardThisCardIsBelow(), (Card c) => IsHeroTarget(c), 1, DamageType.Toxic);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }
    }
}
