using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Drift
{
	public class ThrowingShardCardController : DriftUtilityCardController
	{
		public ThrowingShardCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
		{

		}

		public override IEnumerator UsePower(int index = 0)
		{
			IEnumerator coroutine;

			switch (index)
			{
				case 0:
					{
						//{DriftPast} 
						if (base.IsTimeMatching(Past))
						{
							int targetNumeral = base.GetPowerNumeral(0, 1);
							int gainHPNumeral = base.GetPowerNumeral(1, 2);
							//1 target regains 2 HP.
							coroutine = base.GameController.SelectAndGainHP(base.HeroTurnTakerController, gainHPNumeral, numberOfTargets: targetNumeral, cardSource: base.GetCardSource());
							if (base.UseUnityCoroutines)
							{
								yield return base.GameController.StartCoroutine(coroutine);
							}
							else
							{
								base.GameController.ExhaustCoroutine(coroutine);
							}

							//Shift {DriftRR}.
							coroutine = base.ShiftRR();
							if (base.UseUnityCoroutines)
							{
								yield return base.GameController.StartCoroutine(coroutine);
							}
							else
							{
								base.GameController.ExhaustCoroutine(coroutine);
							}
						}
						else
						{
							coroutine = GameController.SendMessageAction($"{CharacterCard.Title} is not in the past, so nothing happens!", Priority.High, GetCardSource(), showCardSource: true);
							if (base.UseUnityCoroutines)
							{
								yield return base.GameController.StartCoroutine(coroutine);
							}
							else
							{
								base.GameController.ExhaustCoroutine(coroutine);
							}
						}

						break;
					}
				case 1:
					{
						//{DriftFuture} 
						if (base.IsTimeMatching(Future))
						{
							int damageNumeral = base.GetPowerNumeral(2, 1);
							//{Drift} deals each non-hero target 1 radiant damage. 
							coroutine = base.DealDamage(base.GetActiveCharacterCard(), (Card c) => !c.IsHero, damageNumeral, DamageType.Radiant);
							if (base.UseUnityCoroutines)
							{
								yield return base.GameController.StartCoroutine(coroutine);
							}
							else
							{
								base.GameController.ExhaustCoroutine(coroutine);
							}

							//Shift {DriftLL}.
							coroutine = base.ShiftLL();
							if (base.UseUnityCoroutines)
							{
								yield return base.GameController.StartCoroutine(coroutine);
							}
							else
							{
								base.GameController.ExhaustCoroutine(coroutine);
							}
						}
						else
						{
							coroutine = GameController.SendMessageAction($"{CharacterCard.Title} is not in the future, so nothing happens!", Priority.High, GetCardSource(), showCardSource: true);
							if (base.UseUnityCoroutines)
							{
								yield return base.GameController.StartCoroutine(coroutine);
							}
							else
							{
								base.GameController.ExhaustCoroutine(coroutine);
							}
						}
						break;
					}
			}
		   
			
			yield break;
		}
	}
}
