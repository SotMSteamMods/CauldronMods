using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.WindmillCity
{
    public class CrackedWaterMainCardController : WindmillCityUtilityCardController
    {
        public CrackedWaterMainCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowListOfCards(new LinqCardCriteria(c => IsResponder(c), "responder"));
        }

        public override IEnumerator Play()
        {
            //When this card enters play, it deals 1 Responder 2 melee damage.            
            IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, Card), 2, DamageType.Melee, 1, false, 1, additionalCriteria: (Card c) => IsResponder(c) && GameController.IsCardVisibleToCardSource(c, GetCardSource()), cardSource: GetCardSource());
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
            //Reduce all damage dealt by non-environment cards by 1.
            AddReduceDamageTrigger((Card c) => !c.IsEnvironment && GameController.IsCardVisibleToCardSource(c, GetCardSource()), 1);
        }
    }
}
