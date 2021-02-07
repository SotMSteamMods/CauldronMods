using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.WindmillCity
{
    public class CrashedTankerCardController : WindmillCityUtilityCardController
    {
        public CrashedTankerCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowLowestHP(cardCriteria: new LinqCardCriteria(c => IsResponder(c), "responder"));
            SpecialStringMaker.ShowHighestHP(ranking: 2);
        }

        public override IEnumerator Play()
        {
            //When this card enters play, it deals the Responder with the lowest HP 2 fire damage.
            IEnumerator coroutine = DealDamageToLowestHP(Card, 1, (Card c) => IsResponder(c), (Card c) => 2, DamageType.Fire);
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
            //At the end of the environment turn, this card deals the target with the second highest HP {H - 1} fire damage.
            AddDealDamageAtEndOfTurnTrigger(TurnTaker, Card, (Card c) => c.IsTarget && GameController.IsCardVisibleToCardSource(c, GetCardSource()), TargetType.HighestHP, Game.H - 1, DamageType.Fire, highestLowestRanking: 2);
        }
    }
}
