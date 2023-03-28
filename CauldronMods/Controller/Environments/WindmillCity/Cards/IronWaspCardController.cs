using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.WindmillCity
{
    public class IronWaspCardController : WindmillCityUtilityCardController
    {
        public IronWaspCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //At the start of the environment turn, this card deals each hero target X melee damage, where X is the current HP of this card.
            AddStartOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, DealDamageResponse, TriggerType.DealDamage);
        }

        private IEnumerator DealDamageResponse(PhaseChangeAction pca)
        {
            //this card deals each hero target X melee damage, where X is the current HP of this card.
            IEnumerator coroutine = DealDamage(Card, (Card c) => c.IsTarget && IsHero(c), (Card c) => Card.HitPoints, DamageType.Melee);
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
