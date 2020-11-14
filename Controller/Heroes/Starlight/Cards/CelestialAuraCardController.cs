using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Starlight
{
    public class CelestialAuraCardController : StarlightCardController
    {
        public CelestialAuraCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //"Whenever {Starlight} would deal damage to a hero target next to a constellation, instead that target regains that much HP.",
            AddPreventDamageTrigger((DealDamageAction dd) => dd.DamageSource.IsOneOfTheseCards(ListStarlights()) && dd.Target.IsHero && IsNextToConstellation(dd.Target), (DealDamageAction dd) => base.GameController.GainHP(dd.Target, dd.Amount, null, null, GetCardSource()), new TriggerType[1]
            {    
                TriggerType.GainHP
            }, isPreventEffect: true);
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //set stuff up for PowerNumerals and multi-character promo
            int powerNumeral = GetPowerNumeral(0, 1);
            int powerNumeral2 = GetPowerNumeral(1, 1);
            

            List<Card> storedResults = new List<Card> { }; 
            IEnumerator chooseDamageSource = SelectActiveCharacterCardToDealDamage(storedResults, powerNumeral2, DamageType.Radiant);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(chooseDamageSource);
            }
            else
            {
                base.GameController.ExhaustCoroutine(chooseDamageSource);
            }
            Card damageSource = storedResults.FirstOrDefault();

            //"Starlight deals 1 target 1 radiant damage."
            IEnumerator coroutine = base.GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(base.GameController, damageSource), powerNumeral2, DamageType.Radiant, powerNumeral, optional: false, powerNumeral, isIrreducible: false, allowAutoDecide: false, autoDecide: false, null, null, null, null, null, selectTargetsEvenIfCannotDealDamage: false, null, null, ignoreBattleZone: false, null, GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //"Draw a card"
            IEnumerator coroutine2 = DrawCard(base.HeroTurnTakerController.HeroTurnTaker);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine2);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine2);
            }
            yield break;
        }
    }
}