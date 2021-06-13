using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.WindmillCity
{
    public class GrayPharmaceuticalBuildingCardController : WindmillCityUtilityCardController
    {
        public GrayPharmaceuticalBuildingCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //Whenever a hero uses a power that deals damage, increase that damage by 2.
            AddIncreaseDamageTrigger((DealDamageAction dd) => dd.CardSource != null && dd.CardSource.PowerSource != null && !dd.CardSource.Card.IsBeingDestroyed, dd => 2);
            AddTrigger((AddStatusEffectAction se) => se.StatusEffect.DoesDealDamage && se.CardSource != null && se.CardSource.PowerSource != null, IncreaseDamageFromEffectResponse, TriggerType.Hidden, TriggerTiming.Before);

            //After a hero uses a power on a non-character card, destroy that card.
            AddTrigger((UsePowerAction up) => GameController.IsTurnTakerVisibleToCardSource(up.HeroUsingPower.TurnTaker, GetCardSource()) && up.Power != null && up.Power.CardSource != null && !up.Power.CardSource.Card.IsCharacter, UseNonCharacterPowerResponse, TriggerType.DestroyCard, TriggerTiming.After);

            //At the start of the environment turn, destroy this card.
            AddStartOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, DestroyThisCardResponse, TriggerType.DestroySelf);

        }
        private IEnumerator IncreaseDamageFromEffectResponse(AddStatusEffectAction se)
        {
            IncreaseDamageStatusEffect increaseDamageStatusEffect = new IncreaseDamageStatusEffect(2);
            increaseDamageStatusEffect.StatusEffectCriteria.Effect = se.StatusEffect;
            increaseDamageStatusEffect.CreateImplicitExpiryConditions();

            IEnumerator coroutine = AddStatusEffect(increaseDamageStatusEffect);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
        }
        private IEnumerator UseNonCharacterPowerResponse(UsePowerAction up)
        {
            Card powerCardSource = up.Power.CardSource.Card;
          
            IEnumerator coroutine = GameController.DestroyCard(DecisionMaker, powerCardSource, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            yield break;
        }

    }
}
