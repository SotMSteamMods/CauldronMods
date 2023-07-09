using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;

namespace Cauldron.MagnificentMara
{
    public class MaraUtilityCharacterCardController : HeroCharacterCardController
    {
        private List<StatusEffectController> _inUseTriggers;
        private List<StatusEffectController> InUseTriggers
        {
            get
            {
                if(_inUseTriggers == null)
                {
                    _inUseTriggers = new List<StatusEffectController>();
                }
                return _inUseTriggers;
            }
        }
        private List<StatusEffectController> _hasBeenUsedTriggers;
        private List<StatusEffectController> HasBeenUsedTriggers
        {
            get
            {
                if (_hasBeenUsedTriggers == null)
                {
                    _hasBeenUsedTriggers = new List<StatusEffectController>();
                }
                return _hasBeenUsedTriggers;
            }
        }
        private const string CrystalEffectString = "DowsingCrystalDamageBoostResponse";

        protected MaraUtilityCharacterCardController(Card card, TurnTakerController ttc) : base(card, ttc)
        {
        }

        public override void AddTriggers()
        {
            //for Dowsing Crystal's power

            //"(Once before your next turn,) when a non-hero card enters play..."
            AddTrigger((CardEntersPlayAction cep) => cep.CardEnteringPlay != null && !IsHero(cep.CardEnteringPlay) && GameController.StatusEffectManager.StatusEffectControllers.Any((StatusEffectController sec) => sec.StatusEffect is OnDealDamageStatusEffect odds && odds.MethodToExecute == CrystalEffectString),
                            DowsingCrystalDamage,
                            TriggerType.DealDamage,
                            TriggerTiming.After);       
            //clean up in-use trigger list
            AddTrigger((PhaseChangeAction pc) => true, _ => ClearTriggerLists(), TriggerType.HiddenLast, TriggerTiming.Before);
        }

        public IEnumerator DowsingCrystalDamage(CardEntersPlayAction cep)
        {
            //base.SetCardPropertyToTrueIfRealAction("InUse");
            //Log.Debug("DowsingCrystalDamage triggers");
            var dcTriggers = GameController.StatusEffectManager
                                            .StatusEffectControllers
                                            .Where((StatusEffectController sec) => sec.StatusEffect is OnDealDamageStatusEffect odds && odds.MethodToExecute == CrystalEffectString)
                                            .ToList();

            //for each of the various Dowsing Crystal uses that have happened...
            foreach(StatusEffectController seController in dcTriggers)
            {
                var currentTriggerEffect = seController.StatusEffect as OnDealDamageStatusEffect;
                Card triggeringCrystal = currentTriggerEffect.CardSource;
                CardSource crystalSource = FindCardController(triggeringCrystal).GetCardSource(currentTriggerEffect);

                if(!IsSpecificTriggerAvailable(seController))
                {
                    continue;
                }
                InUseTriggers.Add(seController);

                
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
                    int numDamage = currentTriggerEffect.PowerNumeralsToChange?[0] ?? 2;

                    //"one hero target (may deal damage...)"
                    var storedDamageSource = new List<SelectTargetDecision> { };
                    var heroTargets = GameController.FindCardsWhere(new LinqCardCriteria((Card c) => c != null && c.IsTarget && IsHero(c) && c.IsInPlayAndHasGameText), visibleToCard: crystalSource);
                    coroutine = GameController.SelectTargetAndStoreResults(DecisionMaker, heroTargets, storedDamageSource, damageAmount: (Card c) => numDamage, selectionType: SelectionType.HeroToDealDamage, cardSource: crystalSource);
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

                        //"deal a non-hero target 2 damage"

                        coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, damageSource), numDamage, selectedDamage, 1, false, 1, additionalCriteria:((Card c) => !IsHero(c)), cardSource: crystalSource);
                        if (UseUnityCoroutines)
                        {
                            yield return GameController.StartCoroutine(coroutine);
                        }
                        else
                        {
                            GameController.ExhaustCoroutine(coroutine);
                        }

                    }

                    //"Once before your next turn..."
                    if (!crystalSource.Card.IsInPlay)
                    {
                        GameController.AddInhibitorException(crystalSource.CardController, (GameAction ga) => ga is ExpireStatusEffectAction);
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


                    HasBeenUsedTriggers.Add(seController);
                }

                InUseTriggers.Remove(seController);
            }

            yield break;
        }

        private bool LogAndReturnTrue(DealDamageAction dd)
        {
            //Log.Debug("LogAndDebug happens");
            return true;
        }

        private IEnumerator ClearTriggerLists()
        {
            InUseTriggers.Clear();
            HasBeenUsedTriggers.Clear();
            yield return null;
            yield break;
        }
        private bool IsSpecificTriggerAvailable(StatusEffectController sec)
        {
            if(InUseTriggers.Contains(sec) || HasBeenUsedTriggers.Contains(sec))
            {
                return false;
            }
            return true;
        }
    }
}
