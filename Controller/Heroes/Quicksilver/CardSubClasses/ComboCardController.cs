using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.Quicksilver
{
    public class ComboCardController : CardController
    {
        public ComboCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public IEnumerator ComboOrFinish()
        {
            //You may play a Finisher, or {Quicksilver} may deal herself 2 melee damage and play a Combo.
            List<Function> functions = new List<Function> {
                //You may play a Finisher,...
                new Function(base.HeroTurnTakerController, "Play a finisher", SelectionType.PlayCard, () => base.GameController.SelectAndPlayCardFromHand(base.HeroTurnTakerController, true, cardCriteria: new LinqCardCriteria((Card c) => c.DoKeywordsContain("finisher")), cardSource: base.GetCardSource())),
                //...{Quicksilver} may deal herself 2 melee damage and play a Combo.
                new Function(base.HeroTurnTakerController, "Deal yourself 2 melee damage and play a combo", SelectionType.PlayCard, () => ContinueComboResponse())
            };
            IEnumerator coroutine = base.SelectAndPerformFunction(base.HeroTurnTakerController, functions);
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

        public IEnumerator ContinueComboResponse()
        {
            //...{Quicksilver} may deal herself 2 melee damage...
            IEnumerator coroutine = base.DealDamage(base.CharacterCard, base.CharacterCard, 2, DamageType.Melee, cardSource: base.GetCardSource());
            //...play a Combo.
            IEnumerator coroutine2 = base.GameController.SelectAndPlayCardFromHand(base.HeroTurnTakerController, false, cardCriteria: new LinqCardCriteria((Card c) => c.DoKeywordsContain("combo")), cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
                yield return base.GameController.StartCoroutine(coroutine2);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
                base.GameController.ExhaustCoroutine(coroutine2);
            }
            yield break;
        }
    }
}