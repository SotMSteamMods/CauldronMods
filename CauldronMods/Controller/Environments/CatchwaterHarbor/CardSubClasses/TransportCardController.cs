using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.CatchwaterHarbor
{
    public class TransportCardController : CatchwaterHarborUtilityCardController
    {

        public TransportCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

		public override IEnumerator ActivateAbility(string abilityKey)
		{
			IEnumerator enumerator = null;
			if (abilityKey == "travel")
			{
				enumerator = ActivateTravel();
			}
			
			if (enumerator != null)
			{
				if (base.UseUnityCoroutines)
				{
					yield return base.GameController.StartCoroutine(enumerator);
				}
				else
				{
					base.GameController.ExhaustCoroutine(enumerator);
				}
			}
		}

		public virtual IEnumerator ActivateTravel()
		{
			yield return null;
		}
	}
}