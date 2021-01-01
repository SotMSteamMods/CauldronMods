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

        protected override void AddTacticEffectTrigger()
        {
            //"Whenever a hero uses a power that deals damage, increase that damage by 1. You may change the type of that damage to psychic."
            AddTrigger((UsePowerAction up) => GameController.IsTurnTakerVisibleToCardSource(up.HeroUsingPower.TurnTaker, GetCardSource()),
                (UsePowerAction up) => ModifyDamageTypeAndAmountFromPowerResponse(up,
                                            (Func<DealDamageAction, bool> c) => AddIncreaseDamageTrigger(c, 1),
                                            1),
                TriggerType.IncreaseDamage,
                TriggerTiming.Before);
        }

        private IEnumerator ModifyDamageTypeAndAmountFromPowerResponse(UsePowerAction power, Func<Func<DealDamageAction, bool>, ITrigger> addDealDamageTrigger, int? increaseDamageAmount = null, bool makeDamageIrreducible = false)
        {
            RemoveTemporaryTriggers();
            CardController powerCardController = power.Power.IsContributionFromCardSource ? power.Power.CardSource.CardController : power.Power.CardController;
            Func<DealDamageAction, bool> argIncreaseDamage = (DealDamageAction dd) => dd.CardSource.PowerSource != null && dd.CardSource.PowerSource == power.Power && (dd.CardSource.CardController == powerCardController || dd.CardSource.AssociatedCardSources.Any((CardSource cs) => cs.CardController == powerCardController)) && !dd.DamageModifiers.Where((ModifyDealDamageAction md) => md is IncreaseDamageAction).Select((ModifyDealDamageAction md) => md.CardSource.CardController).Contains(this) && !powerCardController.Card.IsBeingDestroyed;
            AddToTemporaryTriggerList(addDealDamageTrigger(argIncreaseDamage));
            if (IsFirstOrOnlyCopyOfThisCardInPlay())
            {
                Func<DealDamageAction, bool> argChangeType = (DealDamageAction dd) => dd.CardSource.PowerSource != null && dd.CardSource.PowerSource == power.Power && (dd.CardSource.CardController == powerCardController || dd.CardSource.AssociatedCardSources.Any((CardSource cs) => cs.CardController == powerCardController)) && !dd.DamageModifiers.Where((ModifyDealDamageAction md) => md is ChangeDamageTypeAction).Select((ModifyDealDamageAction md) => md.CardSource.CardController).Contains(this) && !powerCardController.Card.IsBeingDestroyed;
                AddToTemporaryTriggerList(AddChangeTypeTrigger(argChangeType));
                AddToTemporaryTriggerList(AddTrigger((AddStatusEffectAction se) => se.StatusEffect.DoesDealDamage && se.CardSource.PowerSource != null && se.CardSource.PowerSource == power.Power, (AddStatusEffectAction se) => ModifyDamageFromEffectResponse(se, increaseDamageAmount.Value, power.Power), TriggerType.Hidden, TriggerTiming.Before));
            }
            yield return null;
            yield break;
        }
        private bool LogAndReturnTrue(DealDamageAction dd, string variety)
        {
            if (IsRealAction())
            {
                Log.Debug($"Checking what to do with dealDamageAction regarding " + variety);
            }
            return true;
        }
        private ITrigger AddChangeTypeTrigger(Func<DealDamageAction, bool> criteria)
        {
            //Log.Debug("Makes immediate change-type trigger");
            var trigger = new ChangeDamageTypeTrigger(GameController, criteria, MaybeMakeDamagePsychic, new TriggerType[] { TriggerType.ChangeDamageType }, new DamageType[] { DamageType.Psychic }, GetCardSource());
            return AddTrigger(trigger);
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


        private IEnumerator ModifyDamageFromEffectResponse(AddStatusEffectAction se, int increaseAmount, Power power)
        {

            IncreaseDamageStatusEffect increaseDamageStatusEffect = new IncreaseDamageStatusEffect(increaseAmount);
            increaseDamageStatusEffect.StatusEffectCriteria.Effect = se.StatusEffect;
            if (power != null && power.CardController != null)
            {
                increaseDamageStatusEffect.StatusEffectCriteria.CardWithPower = power.CardController.Card;
            }
            IEnumerator coroutine = AddStatusEffect(increaseDamageStatusEffect);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            var storedYesNo = new List<YesNoCardDecision>();
            coroutine = GameController.MakeYesNoCardDecision(DecisionMaker, SelectionType.DamageType, this.Card, storedResults: storedYesNo, associatedCards: new Card[] { power.CardSource.Card }, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            if(DidPlayerAnswerYes(storedYesNo))
            {
                var makeDamagePsychic = new ChangeDamageTypeStatusEffect(DamageType.Psychic);
                makeDamagePsychic.StatusEffectCriteria.Effect = se.StatusEffect;
                if (power != null && power.CardController != null)
                {
                    increaseDamageStatusEffect.StatusEffectCriteria.CardWithPower = power.CardController.Card;
                }
                coroutine = AddStatusEffect(makeDamagePsychic, false);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }

                coroutine = GameController.SendMessageAction($"Damage from the status effect made by {power.Title} will be changed to psychic.", Priority.Medium, GetCardSource());
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
    }
}