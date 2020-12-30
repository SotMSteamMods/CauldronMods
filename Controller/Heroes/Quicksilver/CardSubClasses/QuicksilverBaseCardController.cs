using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.Quicksilver
{
    public abstract class QuicksilverBaseCardController : CardController
    {
        public static readonly string ComboSelfDamageKey = "ComboSelfDamage";
        public static readonly string ComboKeyword = "combo";
        public static readonly string FinisherKeyword = "finisher";

        protected QuicksilverBaseCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        protected void FlagCharacterTakingComboDamage()
        {
            base.CharacterCardController.SetCardPropertyToTrueIfRealAction(ComboSelfDamageKey);
        }

        protected void ClearCharacterTakingComboDamage()
        {
            if (IsRealAction())
            {
                base.CharacterCardController.SetCardProperty(ComboSelfDamageKey, false);
            }
        }

        protected bool IsCharacterTakingComboDamage()
        {
            return base.CharacterCardController.IsPropertyTrue(ComboSelfDamageKey);
        }


        public IEnumerator ComboOrFinisherResponse()
        {
            //You may play a Finisher, or {Quicksilver} may deal herself 2 melee damage and play a Combo.
            List<Function> functions = new List<Function> {
                //You may play a Finisher,...
                new Function(base.HeroTurnTakerController, "Play a finisher", SelectionType.PlayCard, () => base.GameController.SelectAndPlayCardFromHand(base.HeroTurnTakerController, true, cardCriteria: new LinqCardCriteria((Card c) => c.DoKeywordsContain("finisher"), "finisher"), cardSource: base.GetCardSource())),
                //...{Quicksilver} may deal herself 2 melee damage and play a Combo.
                new Function(base.HeroTurnTakerController, "Deal yourself 2 melee damage and play a combo", SelectionType.PlayCard, () => ContinueWithComboResponse())
            };
            IEnumerator coroutine = base.SelectAndPerformFunction(base.HeroTurnTakerController, functions, true);
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

        public IEnumerator ContinueWithComboResponse()
        {
            FlagCharacterTakingComboDamage();
            //...{Quicksilver} may deal herself 2 melee damage...
            IEnumerator coroutine = base.DealDamage(base.CharacterCard, base.CharacterCard, 2, DamageType.Melee, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            ClearCharacterTakingComboDamage();
            //...play a Combo.
            coroutine = base.GameController.SelectAndPlayCardFromHand(base.HeroTurnTakerController, false, cardCriteria: new LinqCardCriteria((Card c) => c.DoKeywordsContain("combo"), "combo"), cardSource: base.GetCardSource());
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