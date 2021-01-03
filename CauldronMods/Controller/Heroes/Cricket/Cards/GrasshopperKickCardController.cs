using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.Cricket
{
    public class GrasshopperKickCardController : CardController
    {
        public GrasshopperKickCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator UsePower(int index = 0)
        {
            int targetNumeral = GetPowerNumeral(0, 1);
            int damageNumeral = GetPowerNumeral(1, 2);
            //{Cricket} deals 1 target 2 melee damage. 
            IEnumerator coroutine = base.GameController.SelectTargetsAndDealDamage(base.HeroTurnTakerController, new DamageSource(base.GameController, base.CharacterCard), damageNumeral, DamageType.Melee, targetNumeral, false, targetNumeral, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            //{Cricket} is immune to damage dealt by environment targets until the start of your next turn.
            ImmuneToDamageStatusEffect immuneToDamageStatusEffect = new ImmuneToDamageStatusEffect();
            //{Cricket}...
            immuneToDamageStatusEffect.TargetCriteria.IsSpecificCard = base.CharacterCard;
            //...is immune to damage dealt by environment targets...
            immuneToDamageStatusEffect.SourceCriteria.IsEnvironment = true;
            immuneToDamageStatusEffect.SourceCriteria.IsTarget = true;
            //...until the start of your next turn.
            immuneToDamageStatusEffect.UntilStartOfNextTurn(base.TurnTaker);
            immuneToDamageStatusEffect.CardDestroyedExpiryCriteria.Card = base.CharacterCard;
            IEnumerator coroutine2 = base.AddStatusEffect(immuneToDamageStatusEffect, true);
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