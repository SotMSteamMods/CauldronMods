using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.WindmillCity
{
    public class BridgeDisasterCardController : WindmillCityUtilityCardController
    {
        public BridgeDisasterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowLowestHP(cardCriteria: new LinqCardCriteria(c => IsResponder(c), "responder"));
            SpecialStringMaker.ShowHeroTargetWithHighestHP(numberOfTargets: Game.H - 1);
        }

        public override IEnumerator Play()
        {
            //When this card enters play, it deals the Responder with the lowest HP 2 cold damage.
            IEnumerator coroutine = DealDamageToLowestHP(Card, 1, (Card c) => IsResponder(c), (Card c) => 2, DamageType.Cold);
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
            //At the end of the environment turn, this card deals the {H - 1} hero targets with the highest HP 2 cold damage each.
            AddDealDamageAtEndOfTurnTrigger(TurnTaker, Card, (Card c) => IsHeroTarget(c) && GameController.IsCardVisibleToCardSource(c, GetCardSource()), TargetType.HighestHP, 2, DamageType.Cold, numberOfTargets: Game.H - 1);
        }
    }
}
