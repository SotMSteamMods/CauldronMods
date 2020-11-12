using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Markup;

namespace Cauldron.TheKnight
{
    public class ShortSwordCardController : SingleHandEquipmentCardController
    {
        public ShortSwordCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //"Increase damage dealt by {TheKnight} by 1."
            base.AddIncreaseDamageTrigger(dd => IsEquipmentEffectingCard(dd.DamageSource.Card), 1);
            base.AddTriggers();
        }

        public override IEnumerator UsePower(int index = 0)
        {
            //"{TheKnight} deals 1 target 2 melee damage."
            int targets = GetPowerNumeral(0, 1);
            int damages = GetPowerNumeral(1, 2);

            List<SelectCardDecision> results = new List<SelectCardDecision>();
            var coroutine = base.SelectOwnCharacterCard(results, SelectionType.HeroToDealDamage);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            Card card = GetSelectedCard(results);
            coroutine = base.GameController.SelectTargetsAndDealDamage(this.DecisionMaker, new DamageSource(base.GameController, card), damages, DamageType.Melee, targets, false, targets, cardSource: base.GetCardSource());
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
