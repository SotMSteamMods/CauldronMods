using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra;

namespace Cauldron.Echelon
{
    public class SurpriseAttackCardController : TacticBaseCardController
    {
        //==============================================================
        //"At the start of your turn, you may discard a card. If you do not, draw a card and destroy this card.",
        //"Whenever a hero uses a power that deals damage, increase that damage by 1. You may change the type of that damage to psychic."
        //==============================================================

        public static string Identifier = "SurpriseAttack";

        public SurpriseAttackCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            AllowFastCoroutinesDuringPretend = false;
        }

        private string PowerSourceTitleForCustomText = "";

        protected override void AddTacticEffectTrigger()
        {
            //"Whenever a hero uses a power that deals damage, increase that damage by 1. You may change the type of that damage to psychic."
            AddIncreaseDamageTrigger((DealDamageAction dd) => dd.CardSource != null && dd.CardSource.PowerSource != null && !dd.CardSource.Card.IsBeingDestroyed, dd => 1);
            var immediateTypeTrigger = new ChangeDamageTypeTrigger(GameController, (DealDamageAction dd) => dd.CardSource != null && dd.CardSource.PowerSource != null && IsFirstOrOnlyCopyOfThisCardInPlay(), MaybeMakeDamagePsychic, new TriggerType[] { TriggerType.ChangeDamageType }, new DamageType[] { DamageType.Psychic }, GetCardSource());
            AddTrigger(immediateTypeTrigger);
            AddTrigger((AddStatusEffectAction se) => se.StatusEffect.DoesDealDamage && se.CardSource != null && se.CardSource.PowerSource != null, IncreaseDamageFromEffectResponse, TriggerType.Hidden, TriggerTiming.Before);
            AddTrigger((AddStatusEffectAction se) => se.StatusEffect.DoesDealDamage && se.CardSource != null && se.CardSource.PowerSource != null && IsFirstOrOnlyCopyOfThisCardInPlay(), ChangeDamageTypeFromEffectResponse, TriggerType.Hidden, TriggerTiming.Before);

        }

        private IEnumerator MaybeMakeDamagePsychic(DealDamageAction dd)
        {
            //Log.Debug("Maybe-make-damage-psychic goes off");
            IEnumerator coroutine;
            var storedType = new List<SelectDamageTypeDecision>();

            if(dd.OriginalDamageType != DamageType.Psychic)
            {
                var types = new DamageType[] { dd.OriginalDamageType, DamageType.Psychic };
                coroutine = GameController.SelectDamageType(DecisionMaker, storedType, types, dd, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }

            

            var selectedType = storedType.FirstOrDefault()?.SelectedDamageType;
            if(selectedType != null)
            {

                coroutine = GameController.ChangeDamageType(dd, (DamageType)selectedType, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }
            yield break;
        }

        private IEnumerator IncreaseDamageFromEffectResponse(AddStatusEffectAction se)
        {
            IncreaseDamageStatusEffect increaseDamageStatusEffect = new IncreaseDamageStatusEffect(1);
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

        private IEnumerator ChangeDamageTypeFromEffectResponse(AddStatusEffectAction se)
        {
            var storedYesNo = new List<YesNoCardDecision>();
            PowerSourceTitleForCustomText = se.CardSource.PowerSource.Title;
            IEnumerator coroutine = GameController.MakeYesNoCardDecision(DecisionMaker, SelectionType.Custom, this.Card, storedResults: storedYesNo, associatedCards: new Card[] { se.CardSource.Card }, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            if (DidPlayerAnswerYes(storedYesNo))
            {
                var makeDamagePsychic = new ChangeDamageTypeStatusEffect(DamageType.Psychic);
                makeDamagePsychic.StatusEffectCriteria.Effect = se.StatusEffect;
                makeDamagePsychic.CreateImplicitExpiryConditions();

                coroutine = AddStatusEffect(makeDamagePsychic, false);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }

                coroutine = GameController.SendMessageAction($"Damage from the status effect made by {se.CardSource.PowerSource.Title} will be changed to psychic.", Priority.Medium, GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }
        }

        public override CustomDecisionText GetCustomDecisionText(IDecision decision)
        {

            return new CustomDecisionText($"Do you want to change all damage dealt from the status effect made by {PowerSourceTitleForCustomText} to Psychic?", $"Should they change all damage dealt from the status effect made by {PowerSourceTitleForCustomText} to Psychic?", $"Vote for if they should change all damage dealt from the status effect made by {PowerSourceTitleForCustomText} to Psychic?", "change damage to Psychic");

        }


    }
}