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
        public AceInTheHoleCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

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
            yield break;
        }
    }
}