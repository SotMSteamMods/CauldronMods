using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;

namespace Cauldron.MagnificentMara
{
    class MaraUtilityCharacterCardController : HeroCharacterCardController
    {
        public MaraUtilityCharacterCardController(Card card, TurnTakerController ttc) : base(card, ttc)
        {
        }

        public override void AddTriggers()
        {
            AddTrigger((CardEntersPlayAction cep) => CanActivateEffect(this.Card, "Dowsing Crystal trigger") && cep.CardEnteringPlay != null && !cep.CardEnteringPlay.IsHero,
                            DowsingCrystalDamage,
                            TriggerType.DealDamage,
                            TriggerTiming.After);             
        }

        public IEnumerator DowsingCrystalDamage(CardEntersPlayAction cep)
        {
            Log.Debug("DowsingCrystalDamage triggers");
            var dcTriggers = GameController.StatusEffectManager
                                            .GetStatusEffectControllersInList(CardControllerListType.ActivatesEffects)
                                            .Where((StatusEffectController sec) => (sec.StatusEffect as ActivateEffectStatusEffect).EffectName == "Dowsing Crystal trigger")
                                            .ToList();
            
            foreach(StatusEffectController seController in dcTriggers)
            {
                var currentTriggerEffect = seController.StatusEffect as ActivateEffectStatusEffect;
                Card triggeringCrystal = currentTriggerEffect.CardSource;
                CardSource crystalSource = new CardSource(FindCardController(triggeringCrystal));


                var storedYesNo = new List<YesNoCardDecision> { };
                IEnumerator coroutine = GameController.MakeYesNoCardDecision(DecisionMaker, SelectionType.DealDamage, triggeringCrystal, storedResults: storedYesNo,  cardSource: crystalSource);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                if (DidPlayerAnswerYes(storedYesNo))
                {
                    var storedDamageSource = new List<SelectTargetDecision> { };
                    var heroTargets = GameController.FindCardsWhere(new LinqCardCriteria((Card c) => c != null && c.IsTarget && c.IsHero), visibleToCard: crystalSource);
                    coroutine = GameController.SelectTargetAndStoreResults(DecisionMaker, heroTargets, storedDamageSource, damageAmount: (Card c) => 2, selectionType: SelectionType.SelectTargetFriendly, cardSource: crystalSource);
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }

                    var selectedDecision = storedDamageSource.FirstOrDefault();
                    if (selectedDecision != null && selectedDecision.SelectedCard != null)
                    {
                        var damageSource = selectedDecision.SelectedCard;

                        var damageTypeDecision = new List<SelectDamageTypeDecision> { };
                        coroutine = GameController.SelectDamageType(FindHeroTurnTakerController(damageSource.Owner.ToHero()), damageTypeDecision, cardSource: crystalSource);
                        if (base.UseUnityCoroutines)
                        {
                            yield return base.GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            base.GameController.ExhaustCoroutine(coroutine);
                        }

                        var selectedDamage = DamageType.Melee;
                        if(damageTypeDecision.FirstOrDefault() != null && damageTypeDecision.FirstOrDefault().SelectedDamageType != null)
                        {
                            selectedDamage = (DamageType)damageTypeDecision.FirstOrDefault().SelectedDamageType;
                        }


                        //Log.Debug("Dowsing Crystal's trigger-on-Mara approach works so far.");

                        AllowFastCoroutinesDuringPretend = false;

                        var damageSourceTempVar = (Card)AddTemporaryVariable("DowsingCrystalDamageSource", damageSource);

                        var boostDamageTrigger = new IncreaseDamageTrigger(GameController, (DealDamageAction dd) => LogAndReturnTrue(dd) && dd.DamageSource.Card == damageSourceTempVar && dd.CardSource == crystalSource && triggeringCrystal.IsInPlay, DestroyCrystalToBoostDamageResponse, null, TriggerPriority.Low, false, crystalSource);
                        //boostDamageTrigger.

                        AddToTemporaryTriggerList(boostDamageTrigger);
                        
                        coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, damageSourceTempVar), 2, selectedDamage, 1, false, 1, cardSource: crystalSource);
                        if (UseUnityCoroutines)
                        {
                            yield return GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            GameController.ExhaustCoroutine(coroutine);
                        }
                        RemoveTemporaryTriggers();
                        RemoveTemporaryVariables();
                        AllowFastCoroutinesDuringPretend = true;
                    }

                    coroutine = GameController.ExpireStatusEffect(currentTriggerEffect, crystalSource);
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

            yield break;
        }

        private bool LogAndReturnTrue(DealDamageAction dd)
        {
            Log.Debug("LogAndDebug happens");
            return true;
        }

        public IEnumerator DestroyCrystalToBoostDamageResponse(DealDamageAction dd)
        {
            Card sourceCrystal = dd.CardSource.Card;
            if (sourceCrystal != null && sourceCrystal.IsInPlay)
            {
                List<YesNoCardDecision> stored = new List<YesNoCardDecision> { };
                IEnumerator decision = GameController.MakeYesNoCardDecision(DecisionMaker, SelectionType.DestroyCard, sourceCrystal, dd, stored, cardSource: dd.CardSource);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(decision);
                }
                else
                {
                    GameController.ExhaustCoroutine(decision);
                }

                if(DidPlayerAnswerYes(stored))
                {
                    IEnumerator coroutine = GameController.DestroyCard(DecisionMaker, sourceCrystal, cardSource: dd.CardSource);
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(coroutine);
                    }

                    coroutine = GameController.IncreaseDamage(dd, 2, false, dd.CardSource);
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
            yield break;
        }
    }
}
