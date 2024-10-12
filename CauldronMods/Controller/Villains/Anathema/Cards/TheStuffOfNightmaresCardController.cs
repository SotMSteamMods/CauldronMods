using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Anathema
{
	public class TheStuffOfNightmaresCardController : CardController
    {
		public TheStuffOfNightmaresCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
            SpecialStringMaker.ShowHeroTargetWithLowestHP(ranking: 2);
		}

		public override void AddTriggers()
        {

            //Whenever a Villain target enters play, that target deals the hero target with the second lowest HP 2 psychic damage.

            base.AddTargetEntersPlayTrigger((Card c) => IsVillainTarget(c), (Card c) => DealDamageResponse(c), TriggerType.DealDamage, TriggerTiming.After);
        }

        private IEnumerator DealDamageResponse(Card c)
        {
            //that target deals the hero target with the second lowest HP 2 psychic damage
            IEnumerator coroutine =  base.DealDamageToLowestHP(c, 2, (Card h) => IsHeroTarget(h), (Card n) => new int?(2), DamageType.Psychic);

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
