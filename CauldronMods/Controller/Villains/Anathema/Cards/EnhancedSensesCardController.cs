using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Anathema
{
	public class EnhancedSensesCardController : HeadCardController
    {
		public EnhancedSensesCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{
		}

		public override void AddTriggers()
		{
			//Change the type of all damage to sonic.
			base.AddChangeDamageTypeTrigger((DealDamageAction dd) => true, DamageType.Sonic);

			//Whenever a Hero card enters play, this card deals that Hero Character 1 sonic damage.
			base.AddTrigger<PlayCardAction>((PlayCardAction pca) => IsHero(pca.CardToPlay), this.DealDamageResponse,
				new TriggerType[] { TriggerType.DealDamage }, TriggerTiming.After, null, false, true, null, false, null, null, false, false);		
		}

        private IEnumerator DealDamageResponse(PlayCardAction pc)
        {
			Card cardEnteringPlay = pc.CardToPlay;
			//this card deals that Hero Character 1 sonic damage.
			List<Card> results = new List<Card>();
			IEnumerator coroutine = base.FindCharacterCardToTakeDamage(cardEnteringPlay.Owner, results, Card, 1, DamageType.Sonic);
			if (base.UseUnityCoroutines)
			{
				yield return base.GameController.StartCoroutine(coroutine);
			}
			else
			{
				base.GameController.ExhaustCoroutine(coroutine);
			}

			if(results.Count() == 0)
            {
				yield break;
            }
			coroutine = base.DealDamage(base.Card, results.First(), 1, DamageType.Sonic, cardSource: base.GetCardSource());
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
