using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using Handelabra;

namespace Cauldron.MagnificentMara
{
    public class HandIsFasterThanTheEyeCardController : CardController
    {
        private int _lastTriggerIndex = -1;
        private int? _turnEntered = null;
        private string _entryPhase = "";

        private List<ITrigger> _triggersSuppressed;
        private Dictionary<ITrigger, Func<PhaseChangeAction, bool>> _suppressors;
        private List<ITrigger> _wrappedTriggers;


        private ITrigger _finalProcessedTrigger;
        private List<ITrigger> _triggersToSave;

        private int lastTriggerIndex {
            get
            {
                if (_turnEntered != null)
                {
                    if(_turnEntered == Game.TurnIndex && _entryPhase == Game.ActiveTurnPhase.FriendlyPhaseName)
                    {
                        return _lastTriggerIndex;
                    }
                    else
                    {
                        _turnEntered = null;
                        _lastTriggerIndex = -1;
                    }
                }
                return _lastTriggerIndex;
            }
        }
        
        public HandIsFasterThanTheEyeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            _suppressors = new Dictionary<ITrigger, Func<PhaseChangeAction, bool>>();
            SpecialStringMaker.ShowSpecialString(() => "This might not handle being played in the middle of a start-of-turn or end-of-turn phase well.");
        }

        public override IEnumerator Play()
        {
            if(Game.ActiveTurnPhase.IsStart || Game.ActiveTurnPhase.IsEnd)
            {
                _turnEntered = Game.TurnIndex;
                _lastTriggerIndex = GameController.GetMostRecentTriggerIndex();
                _entryPhase = Game.ActiveTurnPhase.FriendlyPhaseName;
                Log.Debug($"Entering play, most recent trigger is {_lastTriggerIndex}");
            }
            AddLastTriggers();
            IEnumerator coroutine = ApplyChangesResponse(null);
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
        }

        public override void AddTriggers()
        {
            /*
            //"When a non-relic, non-character villain card would activate start of turn or end of turn text, destroy it instead. Then, destroy this card.",
            AddStartOfTurnTrigger((TurnTaker tt) => true, CheckForSuppressedTriggerResponse, new TriggerType[] { TriggerType.DestroyCard });
            AddEndOfTurnTrigger((TurnTaker tt) => true, CheckForSuppressedTriggerResponse, new TriggerType[] { TriggerType.DestroyCard });
            */
            AddBeforeDestroyAction(PreventTriggerRemoval);
            //AddBef(PreventTriggerRemoval, TriggerType.Hidden);
        }

        private IEnumerator PreventTriggerRemoval(GameAction ga)
        {
            /*
            if (ga is DestroyCardAction && ((DestroyCardAction)ga).CardToDestroy == this)
            {
                ((DestroyCardAction)ga).PreventRemoveTriggers = true;
            }
            */
            yield return null;
            yield break;
        }
        public override void AddLastTriggers()
        {
            _triggersSuppressed = new List<ITrigger> { };
            _wrappedTriggers = new List<ITrigger> { };
            AddTrigger((PhaseChangeAction p) => IsVillain(p.ToPhase.TurnTaker) && base.GameController.IsTurnTakerVisibleToCardSource(p.ToPhase.TurnTaker, GetCardSource()), ApplyChangesResponse, TriggerType.HiddenLast, TriggerTiming.Before);
            AddTrigger((FlipCardAction f) => IsVillain(f.CardToFlip.Card) && base.GameController.IsCardVisibleToCardSource(f.CardToFlip.Card, GetCardSource()), ApplyChangesResponse, TriggerType.HiddenLast, TriggerTiming.After);
            AddTrigger((PlayCardAction p) => IsVillain(p.CardToPlay) && base.GameController.IsCardVisibleToCardSource(p.CardToPlay, GetCardSource()), ApplyChangesResponse, TriggerType.HiddenLast, TriggerTiming.After);
            AddTrigger((CardEntersPlayAction p) => base.GameController.IsInCardControllerList(p.CardEnteringPlay, CardControllerListType.ModifiesDeckKind), ReapplyAllTriggersResponse, TriggerType.HiddenLast, TriggerTiming.After);
            AddTrigger((CardEntersPlayAction p) => p.CardEnteringPlay != null && p.CardEnteringPlay.Identifier == "IsolatedHero" && IsVillain(base.Card), ReapplyAllTriggersResponse, TriggerType.HiddenLast, TriggerTiming.After);
            AddTrigger((SwitchBattleZoneAction sba) => sba.Origin != sba.Destination, ReapplyAllTriggersResponse, TriggerType.HiddenLast, TriggerTiming.After);
            AddTrigger((MoveCardAction mc) => mc.Origin.BattleZone != mc.Destination.BattleZone, ReapplyAllTriggersResponse, TriggerType.HiddenLast, TriggerTiming.After);
            AddBeforeLeavesPlayActions((GameAction ga) => RestoreTriggersOnDestroy(ga, true));
            AddAfterLeavesPlayAction(ReapplySavedTriggers);
        }

        private IEnumerator ApplyChangesResponse(GameAction action)
        {
            foreach (TurnTakerController item in base.GameController.TurnTakerControllers.Where((TurnTakerController ttc) => GameController.IsTurnTakerVisibleToCardSource(ttc.TurnTaker, GetCardSource()) && base.BattleZone == ttc.BattleZone))
            {
                TurnPhase start = item.TurnTaker.TurnPhases.Where((TurnPhase phase) => phase.IsStart).First();
                TurnPhase end = item.TurnTaker.TurnPhases.Where((TurnPhase phase) => phase.IsEnd).First();
                PhaseChangeAction startOfTurn = new PhaseChangeAction(GetCardSource(), null, start, start.IsEphemeral);
                PhaseChangeAction endOfTurn = new PhaseChangeAction(GetCardSource(), null, end, end.IsEphemeral);

                //IEnumerable<ITrigger> changeMe = FindTriggersWhere((ITrigger t) => t is PhaseChangeTrigger && IsVillain(t.CardSource.Card.Owner) && !t.CardSource.Card.IsCharacter && !t.CardSource.Card.IsRelic && !t.CardSource.Card.IsMissionCard && (t.DoesMatchTypeAndCriteria(endOfTurn) || t.DoesMatchTypeAndCriteria(startOfTurn)) && t.CardSource.BattleZone == base.BattleZone);
                IEnumerable<ITrigger> changeMe = FindTriggersWhere((ITrigger t) => t is PhaseChangeTrigger && t.CardSource != null && (t.DoesMatchTypeAndCriteria(endOfTurn) || t.DoesMatchTypeAndCriteria(startOfTurn) && t.CardSource.BattleZone == this.BattleZone)).OrderBy((ITrigger t) => t.CardSource.Card.PlayIndex);

                changeMe.ForEach(delegate (ITrigger t)
                {
                    SuppressTrigger((PhaseChangeTrigger)t, end);
                });
                if (Game.ActiveTurnPhase != null && (Game.ActiveTurnPhase.IsEnd || Game.ActiveTurnPhase.IsStart))
                {
                    GameController.AddTemporaryTriggerInhibitor((ITrigger t) => changeMe.Contains(t), (GameAction ga) => (ga is PhaseChangeAction && ((ga as PhaseChangeAction).FromPhase.IsEnd || (ga as PhaseChangeAction).FromPhase.IsStart)) || !Card.IsInPlayAndHasGameText, GetCardSource());
                }
            }
            yield return null;
            yield break;
        }

        private void SuppressTrigger(PhaseChangeTrigger trigger, TurnPhase triggeringPhase)
        {
            if (!_triggersSuppressed.Contains(trigger))
            {
                Log.Debug($"Suppressing trigger on {trigger.CardSource.Card.Title}: {trigger.ToString()}");
                Func<PhaseChangeAction, bool> stop = (PhaseChangeAction pca) => pca.CardSource != null && pca.CardSource.Card == this.Card;

                if (_suppressors.ContainsKey(trigger))
                {
                    trigger.RemoveAdditionalCriteria(_suppressors[trigger]);
                    _suppressors.Remove(trigger);
                }
                var wrappedTrigger = WrapTrigger(trigger);
                AddTrigger(wrappedTrigger);
                _wrappedTriggers.Add(wrappedTrigger);

                trigger.AddAdditionalCriteria(stop);
                _triggersSuppressed.Add(trigger);
                _suppressors.Add(trigger, stop);
            }
        }
        public IEnumerator CheckForSuppressedTriggerResponse(PhaseChangeAction pc)
        {
            if (_triggersSuppressed.Count() == 0)
            {
                yield break;
            }
            //Log.Debug("Checking for suppressed triggers");
            //Log.Debug($"Phase change card source: {pc.CardSource}");
            var unsuppressorAction = new PhaseChangeAction(GetCardSource(), pc.FromPhase, pc.ToPhase, pc.IsEphemeral, pc.ForceIncrementTurnIndex);

            Func<ITrigger, bool> logFunc = (ITrigger trigger) => LogAndReturnTrue(trigger) && trigger.DoesMatchTypeAndCriteria(unsuppressorAction);
            // && trigger.Index > lastTriggerIndex

            //not quite correct - it will get the first card that would act in a phase, 
            //not the first one that WOULD act after Hand is Faster than the Eye enters play
            //that's a whole other level of tricky to deal with
            var firstSuppressedTrigger = _triggersSuppressed.Where(logFunc).FirstOrDefault();

            bool foundTrigger = firstSuppressedTrigger != null;
            //Log.Debug($"Found a suppressed trigger? {foundTrigger}");
            if (firstSuppressedTrigger != null)
            {
                Card cardToDestroy = firstSuppressedTrigger.CardSource.Card;
                IEnumerator coroutine = GameController.SendMessageAction($"{this.Card.Title} interrupts the {pc.ToPhase.FriendlyPhaseName} trigger on {cardToDestroy.Title}!", Priority.High, GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
                coroutine = GameController.DestroyCard(DecisionMaker, cardToDestroy, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
                coroutine = DestroyThisCardResponse(pc);
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

        private bool LogAndReturnTrue(ITrigger trigger)
        {
            Log.Debug("Trigger info: " + trigger.ToString() + " index " + trigger.Index.ToString());
            //Log.Debug("Suppressor record: " + _suppressors[trigger].ToString());
            return true;
        }

        private IEnumerator ReapplyAllTriggersResponse(GameAction action)
        {
            IEnumerator coroutine = RestoreTriggersOnDestroy(action, false);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            coroutine = ApplyChangesResponse(action);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
        }

        private IEnumerator RestoreTriggersOnDestroy(GameAction action, bool isRealDestroy)
        {
            foreach (PhaseChangeTrigger trigger in _triggersSuppressed)
            {
                trigger.RemoveAdditionalCriteria(_suppressors[trigger]);
                if (!GameController.ActiveTurnPhase.IsStart && !GameController.ActiveTurnPhase.IsEnd || trigger.CardSource == null)
                {
                    continue;
                }
                /* this suppresses the outgoing SZA triggers, i think
                 * TODO - figure out what all this does, and what of it this card needs!
                TurnPhase turnPhase = base.GameController.ActiveTurnPhase.TurnTaker.TurnPhases.Where((TurnPhase p) => p.IsStart).First();
                PhaseChangeAction action2 = new PhaseChangeAction(GetCardSource(), null, turnPhase, turnPhase.IsEphemeral);
                bool isInPlayAndHasGameText = trigger.CardSource.CardController.CardWithoutReplacements.IsInPlayAndHasGameText;
                bool flag = trigger.DoesMatchTypeAndCriteria(action2);
                bool flag2 = FindTriggersWhere((ITrigger t) => t != trigger && t.CardSource.CardController.CardWithoutReplacements == trigger.CardSource.CardController.CardWithoutReplacements).FirstOrDefault() != null;
                if (isInPlayAndHasGameText && !flag && !flag2)
                {
                    base.GameController.AddTemporaryInhibitor(trigger.CardSource.CardController, (PhaseChangeAction phaseChange) => true, GetCardSource());
                    base.GameController.AddInhibitorException(trigger.CardSource.CardController, (GameAction subAction) => subAction is MoveCardAction && ((MoveCardAction)subAction).CardToMove == base.Card);
                }
                */
            }

            if(isRealDestroy && GameController.RealMode)
            {
                if (_triggersToSave == null)
                {
                    _triggersToSave = new List<ITrigger> { };
                }
                Log.Debug("Actual destruction, looking for triggers to save.");
                Log.Debug($"Last processed: {_finalProcessedTrigger}");
                Log.Debug($"Wrapped triggers to process: {_wrappedTriggers.Count()}");
            }

            bool saveFollowingTriggers = false;
            foreach (PhaseChangeTrigger wrapped in _wrappedTriggers)
            {
                if (!GameController.ActiveTurnPhase.IsStart && !GameController.ActiveTurnPhase.IsEnd || wrapped.CardSource == null)
                {
                    RemoveTrigger(wrapped);
                }
                else
                {
                     if(isRealDestroy && GameController.RealMode)
                    {
                        //Log.Debug("Actual destruction, looking for triggers to save:");
                        if (saveFollowingTriggers)
                        {
                            Log.Debug($"Saving trigger: {wrapped.AssociatedTriggers.FirstOrDefault()}");
                            _triggersToSave.Add(wrapped);
                        }
                        else
                        {
                            saveFollowingTriggers = wrapped.AssociatedTriggers.Any((ITrigger trigger) => _finalProcessedTrigger.CompareTo(trigger) == 0);
                            if(saveFollowingTriggers)
                            {
                                Log.Debug($"Found destruction trigger: {wrapped.AssociatedTriggers.FirstOrDefault()}");
                            }
                        }
                    }
                }
            }
            if (isRealDestroy && GameController.RealMode)
            {
                Log.Debug($"Found {_triggersToSave.Count()} triggers to save");
                int? currentTurnIndex = Game.TurnIndex;
                int currentPhaseIndex = Game.TurnPhaseIndex;
                AddInhibitorException((GameAction ga) => !(ga is DestroyCardAction) && Game.TurnIndex == currentTurnIndex && Game.TurnPhaseIndex == currentPhaseIndex);
            }
            _triggersSuppressed.Clear();
            _wrappedTriggers.Clear();
            yield return null;
            yield break;
        }
        
        private IEnumerator ReapplySavedTriggers()
        {
            RemoveInhibitor();
            Log.Debug("Hopefully triggers continue...");
            
            foreach(Trigger<PhaseChangeAction> trigger in _triggersToSave)
            {
                ITrigger lateTrigger = new Trigger<PhaseChangeAction>(GameController,
                                            trigger.Criteria,
                                            trigger.Response,
                                            trigger.Types,
                                            trigger.Timing,
                                            trigger.CardSource,
                                            trigger.ActionDescriptions,
                                            trigger.IsConditionalOnSimilar,
                                            trigger.RequireActionSuccess,
                                            trigger.IsActionOptional,
                                            trigger.IsOutOfPlayTrigger,
                                            trigger.OrderMatters,
                                            trigger.Priority,
                                            trigger.IgnoreBattleZone == true,
                                            respondEvenIfPlayedAfterAction: true,
                                            trigger.CopyingCardController);
                Log.Debug($"Trigger being added: {lateTrigger}");
                AddTrigger(lateTrigger);
            }
            
            yield break;
        }
        
        private ITrigger WrapTrigger(Trigger<PhaseChangeAction> trigger)
        {
            var pc = trigger as PhaseChangeTrigger;
            PhaseChangeTrigger wrappedTrigger = new PhaseChangeTrigger(GameController,
                                                                    pc.TurnTakerCriteria,
                                                                    pc.PhaseCriteria,
                                                                    (PhaseChangeAction x) => true,
                                                                    (PhaseChangeAction phase) => InterruptOrAllowResponse(phase, trigger),
                                                                    pc.Types,
                                                                    pc.Timing,
                                                                    pc.IgnoreBattleZone == true,
                                                                    pc.CardSource);
            /*
            ITrigger wrappedTrigger = new Trigger<PhaseChangeAction>(GameController,
                                                        trigger.Criteria,
                                                        (PhaseChangeAction pc) => InterruptOrAllowResponse(pc, trigger),
                                                        trigger.Types,
                                                        trigger.Timing,
                                                        trigger.CardSource,
                                                        trigger.ActionDescriptions,
                                                        trigger.IsConditionalOnSimilar,
                                                        trigger.RequireActionSuccess,
                                                        trigger.IsActionOptional,
                                                        trigger.IsOutOfPlayTrigger,
                                                        trigger.OrderMatters,
                                                        trigger.Priority,
                                                        trigger.IgnoreBattleZone == true,
                                                        trigger.RespondEvenIfPlayedAfterAction,
                                                        trigger.CopyingCardController);
            */
            wrappedTrigger.AddAssociatedTrigger(trigger);
            return wrappedTrigger;
        }

        private IEnumerator InterruptOrAllowResponse(PhaseChangeAction pc, Trigger<PhaseChangeAction> trigger)
        {
            if (this.Card.IsInPlay)
            {
                _finalProcessedTrigger = trigger;
            }
            Log.Debug($"Calling wrapper function for {trigger.ToString()}");
            Card triggering = null;
            var unlockPCA = new PhaseChangeAction(GetCardSource(), pc.FromPhase, pc.ToPhase, pc.IsEphemeral, pc.ForceIncrementTurnIndex);

            if (trigger.CardSource != null)
            {
                triggering = trigger.CardSource.Card;
            }
            /*
            Log.Debug($"Original Card Is : {triggering.ToString()}");
            Log.Debug("Conditions:");
            Log.Debug($"Not Character: {!triggering.IsCharacter}");
            Log.Debug($"Is Villain: {IsVillain(triggering)}");
            Log.Debug($"Is Not Relic: {!triggering.IsRelic}");
            Log.Debug($"Would otherwise trigger: {trigger.DoesMatchTypeAndCriteria(unlockPCA)}"); // destruction criteria go here
            */
            if (triggering != null && this.Card.IsInPlay && !this.Card.IsBeingDestroyed && GameController.RealMode && IsVillain(triggering) && !triggering.IsCharacter && !triggering.IsRelic && !triggering.IsMissionCard && trigger.DoesMatchTypeAndCriteria(unlockPCA)) // destruction criteria go here
            {
                Card cardToDestroy = trigger.CardSource.Card;
                IEnumerator coroutine = GameController.SendMessageAction($"{this.Card.Title} interrupts the {pc.ToPhase.FriendlyPhaseName} trigger on {cardToDestroy.Title}!", Priority.High, GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
                coroutine = GameController.DestroyCard(DecisionMaker, cardToDestroy, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
                coroutine = DestroyThisCardResponse(pc);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }
            else
            {
                if (GameController.RealMode)
                { 
                    Log.Debug("Criteria do not match, so we should let it act."); 
                }
                if (trigger.DoesMatchTypeAndCriteria(unlockPCA))
                {
                    //Log.Debug("And we should be.");
                    IEnumerator coroutine = trigger.Response(unlockPCA);
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