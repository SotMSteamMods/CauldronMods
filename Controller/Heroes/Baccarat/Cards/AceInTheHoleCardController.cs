using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Baccarat
{
    public class AceInTheHoleCardController : CardController
    {
        #region Constructors

        public AceInTheHoleCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController) 
        {

        }

        #endregion Constructors

        #region Methods

        public override IEnumerator Play()
        {
            //You may play a card.
            IEnumerator coroutine = base.SelectAndPlayCardFromHand(base.HeroTurnTakerController);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

			//You may use {Baccarat}'s innate power twice during your phase this turn.
			if (base.GameController.ActiveTurnTaker == base.TurnTaker)
			{
				AllowSetNumberOfPowerUseStatusEffect allowSetNumberOfPowerUseStatusEffect = new AllowSetNumberOfPowerUseStatusEffect(2);
				allowSetNumberOfPowerUseStatusEffect.UsePowerCriteria.IsSpecificCard = base.CharacterCard;
				allowSetNumberOfPowerUseStatusEffect.UsePowerCriteria.CardSource = base.CharacterCard;
				allowSetNumberOfPowerUseStatusEffect.UntilThisTurnIsOver(base.GameController.Game);
				allowSetNumberOfPowerUseStatusEffect.CardDestroyedExpiryCriteria.Card = base.CharacterCard;
				allowSetNumberOfPowerUseStatusEffect.NumberOfUses = new int?(1);
				coroutine = base.AddStatusEffect(allowSetNumberOfPowerUseStatusEffect);
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
				int timesUsed = (from e in Journal.UsePowerEntriesThisTurn()
								 where e.CardWithPower == base.CharacterCard
								 select e).Count<UsePowerJournalEntry>();
				if (timesUsed < 2)
				{
					List<YesNoCardDecision> storedResults = new List<YesNoCardDecision>();
					SelectionType type = SelectionType.UsePowerTwice;
					if (timesUsed == 1)
					{
						type = SelectionType.UsePowerAgain;
					}
					IEnumerator coroutine2 = base.GameController.MakeYesNoCardDecision(base.HeroTurnTakerController, type, base.CharacterCard, null, storedResults);
					if (base.UseUnityCoroutines)
					{
						yield return base.GameController.StartCoroutine(coroutine2);
					}
					else
					{
						base.GameController.ExhaustCoroutine(coroutine2);
					}
					if (base.DidPlayerAnswerYes(storedResults))
					{
						int num;
						for (int i = 0; i < 2 - timesUsed; i = num + 1)
						{
							coroutine2 = base.UsePowerOnOtherCard(base.CharacterCard);
							if (base.UseUnityCoroutines)
							{
								yield return base.GameController.StartCoroutine(coroutine2);
							}
							else
							{
								base.GameController.ExhaustCoroutine(coroutine2);
							}
							num = i;
						}
					}
					storedResults = null;
				}
				else
				{
					IEnumerator coroutine3 = base.GameController.SendMessageAction(base.TurnTaker.Name + " has already used " + base.CharacterCard.Definition.Body.First<string>() + " twice this turn.", Priority.High, base.GetCardSource(null), null, true);
					if (base.UseUnityCoroutines)
					{
						yield return base.GameController.StartCoroutine(coroutine3);
					}
					else
					{
						base.GameController.ExhaustCoroutine(coroutine3);
					}
				}
			}
			yield break;
        }

        #endregion Methods
    }
}