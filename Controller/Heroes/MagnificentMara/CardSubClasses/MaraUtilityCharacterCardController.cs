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
        private List<StatusEffectController> _inUseTriggers;
        private List<StatusEffectController> _hasBeenUsedTriggers;
        public MaraUtilityCharacterCardController(Card card, TurnTakerController ttc) : base(card, ttc)
        {
            _inUseTriggers = new List<StatusEffectController> { };
            _hasBeenUsedTriggers = new List<StatusEffectController> { };
        }

        public override void AddTriggers()
        {
            //for Dowsing Crystal's power

            //"(Once before your next turn,) when a non-hero card enters play..."
            AddTrigger((CardEntersPlayAction cep) => IsTriggerAvailable() && CanActivateEffect(this.Card, "Dowsing Crystal trigger") && cep.CardEnteringPlay != null && !cep.CardEnteringPlay.IsHero,
                            DowsingCrystalDamage,
                            TriggerType.DealDamage,
                            TriggerTiming.After);       
            AddTrigger((PhaseChangeAction pc) => true, _ => ClearTriggerLists(), TriggerType.HiddenLast, TriggerTiming.Before);
        }

        public IEnumerator DowsingCrystalDamage(CardEntersPlayAction cep)
        {
            //base.SetCardPropertyToTrueIfRealAction("InUse");
            //Log.Debug("DowsingCrystalDamage triggers");
            var dcTriggers = GameController.StatusEffectManager
                                            .GetStatusEffectControllersInList(CardControllerListType.ActivatesEffects)
                                            .Where((StatusEffectController sec) => (sec.StatusEffect as ActivateEffectStatusEffect).EffectName == "Dowsing Crystal trigger")
                                            .ToList();

            //for each of the various Dowsing Crystal uses that have happened...
            foreach(StatusEffectController seController in dcTriggers)
            {
                var currentTriggerEffect = seController.StatusEffect as ActivateEffectStatusEffect;
                Card triggeringCrystal = currentTriggerEffect.CardSource;
                CardSource crystalSource = new CardSource(FindCardController(triggeringCrystal));

                if(!IsSpecificTriggerAvailable(seController))
                {
                    continue;
                }
                _inUseTriggers.Add(seController);

                if(!crystalSource.Card.IsInPlay)
                {
                    GameController.RemoveInhibitor(crystalSource.CardController);
                }

                //"(one hero target) may..."
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

                    //"one hero target (may deal damage...)"
                    var storedDamageSource = new List<SelectTargetDecision> { };
                    var heroTargets = GameController.FindCardsWhere(new LinqCardCriteria((Card c) => c != null && c.IsTarget && c.IsHero && c.IsInPlayAndHasGameText), visibleToCard: crystalSource);
                    coroutine = GameController.SelectTargetAndStoreResults(DecisionMaker, heroTargets, storedDamageSource, damageAmount: (Card c) => 2, selectionType: SelectionType.HeroToDealDamage, cardSource: crystalSource);
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

                        //"...of a type of their choosing."
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

                        //attempts to give the damage a destroy-dowsing-crystal-for-boost effect

                        //does not seem to be needed yet, causes warnings in multiple-play-reactions
                        //may be required in some future scenario
                        //var damageSourceTempVar = (Card)AddTemporaryVariable("DowsingCrystalDamageSource", damageSource);

                        var boostDamageTrigger = new IncreaseDamageTrigger(GameController, (DealDamageAction dd) => LogAndReturnTrue(dd) && dd.DamageSource.Card == damageSource && dd.CardSource == crystalSource && triggeringCrystal.IsInPlay, DestroyCrystalToBoostDamageResponse, null, TriggerPriority.Low, false, crystalSource);

                        AddTrigger(boostDamageTrigger);

                        //"deal a non-hero target 2 damage"

                        coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, damageSource), 2, selectedDamage, 1, false, 1, additionalCriteria:((Card c) => !c.IsHero), cardSource: crystalSource);
                        if (UseUnityCoroutines)
                        {
                            yield return GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            GameController.ExhaustCoroutine(coroutine);
                        }

                        RemoveTrigger(boostDamageTrigger);
                        //RemoveTemporaryVariables();
                    }

                    //"Once before your next turn..."
                    coroutine = GameController.ExpireStatusEffect(currentTriggerEffect, crystalSource);
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(coroutine);
                    }
                    _hasBeenUsedTriggers.Add(seController);
                }
                _inUseTriggers.Remove(seController);
            }

            yield break;
        }

        private bool LogAndReturnTrue(DealDamageAction dd)
        {
            //Log.Debug("LogAndDebug happens");
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

                    GameController.RemoveInhibitor(FindCardController(sourceCrystal));

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
            else if (!sourceCrystal.IsInPlay)
            {
                GameController.RemoveInhibitor(FindCardController(sourceCrystal));
            }

            


            yield break;
        }

        private IEnumerator ClearTriggerLists()
        {
            _inUseTriggers.Clear();
            _hasBeenUsedTriggers.Clear();
            yield return null;
            yield break;
        }
        private bool IsTriggerAvailable()
        {
            //return Game.Journal.CardPropertiesEntriesThisTurn(Card).Any(j => j.Key == "InUse") != true;
            return true;
        }
        private bool IsSpecificTriggerAvailable(StatusEffectController sec)
        {
            if(_inUseTriggers.Contains(sec) || _hasBeenUsedTriggers.Contains(sec))
            {
                return false;
            }
            return true;
        }
    }
}
