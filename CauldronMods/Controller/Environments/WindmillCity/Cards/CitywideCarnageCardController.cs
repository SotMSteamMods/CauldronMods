using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.WindmillCity
{
    public class CitywideCarnageCardController : WindmillCityUtilityCardController
    {
        public CitywideCarnageCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override IEnumerator Play()
        {
            //When this card enters play, it deals each Responder 2 energy damage.
            IEnumerator coroutine = DealDamage(Card, (Card c) => IsResponder(c), (Card c) => 2, DamageType.Energy);
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

        public override void AddTriggers()
        {
            //At the end of the environment turn, this card deals each hero target 1 toxic damage and each villain target 1 energy damage.
            AddDealDamageAtEndOfTurnTrigger(TurnTaker, Card, (Card c) => IsHeroTarget(c) && GameController.IsCardVisibleToCardSource(c, GetCardSource()), TargetType.All, 1, DamageType.Toxic);
            AddDealDamageAtEndOfTurnTrigger(TurnTaker, Card, (Card c) => IsVillainTarget(c) && GameController.IsCardVisibleToCardSource(c, GetCardSource()), TargetType.All, 1, DamageType.Energy);

        }
    }
}
