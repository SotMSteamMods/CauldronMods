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
        private List<ITrigger> _surveilledTriggers;
        private Dictionary<ITrigger, Func<PhaseChangeAction, bool>> _spies;
        private ITrigger _justActivatedTrigger = null;
        
        public HandIsFasterThanTheEyeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            _spies = new Dictionary<ITrigger, Func<PhaseChangeAction, bool>>();
            //SpecialStringMaker.ShowSpecialString(() => "This might not handle being played in the middle of a start-of-turn or end-of-turn phase well.");
        }

        public override IEnumerator Play()
        {
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
            //"When a non-relic, non-character villain card would activate start of turn or end of turn text,
            //destroy it instead. Then, destroy this card."
            AddTrigger((GameAction ga) => !this.IsBeingDestroyed && _justActivatedTrigger != null && ga.CardSource != null && ga.CardSource.Card == _justActivatedTrigger.CardSource.Card && IsFirstOrOnlyCopyOfThisCardInPlay(),
                            (GameAction ga) => InterruptAction(ga),
                            TriggerType.CancelAction,
                            TriggerTiming.Before);

            //cleans up JustActivatedTrigger to reduce execution overhead
            AddPhaseChangeTrigger((TurnTaker tt) => true, (Phase p) => true, (PhaseChangeAction pc) => true, ClearJustActed, new TriggerType[] { TriggerType.HiddenLast }, TriggerTiming.Before);
        }
        private bool LogAndReturnTrue(GameAction ga)
        {
            //Log.Debug($"Checking action {ga}...");
            return true;
        }
        private IEnumerator ClearJustActed(PhaseChangeAction pca)
        {
            _justActivatedTrigger = null;
            yield return null;
            yield break;
        }

        private bool IsBookkeepingAction(GameAction ga)
        {

            if(ga is MessageAction || ga is ExpireStatusEffectAction || ga is DoneRoundOfDamageAction)
            {
                return true;
            }

            //There is a null reference error somewhere in TargetInfo.GetTargets if we 
            //cancel the SelectTurnTakerDecision in DetermineTurnTakersWithMostOrFewest
            //for an ambiguous who-to-damage decision as on Voss's Gene Bound Banshee
            //I have tried to keep this exception limited - it's possibly worth keeping an eye on.
            if (ga is MakeDecisionAction md && (md.Decision is SelectTurnTakerDecision || md.Decision is SelectTurnTakersDecision))
            {
                return _justActivatedTrigger != null && _justActivatedTrigger.Types.Contains(TriggerType.DealDamage);
            }
            return false;
        }
        private IEnumerator InterruptAction(GameAction ga)
        {
            //May end up with false positives from "fake" actions like messages -
            //we'll have to see what crops up
            //Log.Debug("InterruptAction fires");
            if(IsRealAction(ga) && !IsBookkeepingAction(ga))
            {
                
                //Log.Debug($"Would interrupt the trigger {_justActivatedTrigger}");
                Card cardToDestroy = ga.CardSource.Card;
                //prevent ourselves from canceling its destruction effects too
                _justActivatedTrigger = null;
                IEnumerator coroutine = GameController.SendMessageAction($"{this.Card.Title} interrupts the {Game.ActiveTurnPhase.FriendlyPhaseName} trigger on {cardToDestroy.Title}!", Priority.High, GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }

                //needed to prevent "put-into-play" triggers
                if(ga is PlayCardAction pca)
                {
                    pca.AllowPutIntoPlayCancel = true;
                }

                coroutine = GameController.CancelAction(ga, cardSource: GetCardSource());
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
                
                coroutine = DestroyThisCardResponse(ga);
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

        public override void AddLastTriggers()
        {
            _surveilledTriggers = new List<ITrigger> { };
            // a whole big list of things that might change what triggers exist and/or are visible 
            // and/or need to be responded to
            AddTrigger((PhaseChangeAction p) => IsVillain(p.ToPhase.TurnTaker) && base.GameController.IsTurnTakerVisibleToCardSource(p.ToPhase.TurnTaker, GetCardSource()), ApplyChangesResponse, TriggerType.HiddenLast, TriggerTiming.Before);
            AddTrigger((FlipCardAction f) => IsVillain(f.CardToFlip.Card) && base.GameController.IsCardVisibleToCardSource(f.CardToFlip.Card, GetCardSource()), ApplyChangesResponse, TriggerType.HiddenLast, TriggerTiming.After);
            AddTrigger((PlayCardAction p) => IsVillain(p.CardToPlay) && base.GameController.IsCardVisibleToCardSource(p.CardToPlay, GetCardSource()), ApplyChangesResponse, TriggerType.HiddenLast, TriggerTiming.After);
            AddTrigger((CardEntersPlayAction p) => base.GameController.IsInCardControllerList(p.CardEnteringPlay, CardControllerListType.ModifiesDeckKind), ReapplyAllTriggersResponse, TriggerType.HiddenLast, TriggerTiming.After);
            AddTrigger((CardEntersPlayAction p) => p.CardEnteringPlay != null && p.CardEnteringPlay.Identifier == "IsolatedHero" && IsVillain(base.Card), ReapplyAllTriggersResponse, TriggerType.HiddenLast, TriggerTiming.After);
            AddTrigger((SwitchBattleZoneAction sba) => sba.Origin != sba.Destination, ReapplyAllTriggersResponse, TriggerType.HiddenLast, TriggerTiming.After);
            AddTrigger((MoveCardAction mc) => mc.Origin.BattleZone != mc.Destination.BattleZone, ReapplyAllTriggersResponse, TriggerType.HiddenLast, TriggerTiming.After);
            AddBeforeLeavesPlayActions(RestoreTriggersOnDestroy);
        }

        private IEnumerator ApplyChangesResponse(GameAction action)
        {
            //Makes a list of start-of-turn and end-of-turn triggers on all appropriate cards,
            //then gives each an additional "spy" criterion that always lets it happen but also 
            //gives this card some information
            foreach (TurnTakerController item in base.GameController.TurnTakerControllers.Where((TurnTakerController ttc) => GameController.IsTurnTakerVisibleToCardSource(ttc.TurnTaker, GetCardSource()) && base.BattleZone == ttc.BattleZone))
            {
                TurnPhase start = item.TurnTaker.TurnPhases.Where((TurnPhase phase) => phase.IsStart).First();
                TurnPhase end = item.TurnTaker.TurnPhases.Where((TurnPhase phase) => phase.IsEnd).First();
                PhaseChangeAction startOfTurn = new PhaseChangeAction(GetCardSource(), null, start, start.IsEphemeral);
                PhaseChangeAction endOfTurn = new PhaseChangeAction(GetCardSource(), null, end, end.IsEphemeral);

                IEnumerable<ITrigger> changeMe = FindTriggersWhere((ITrigger t) => t is PhaseChangeTrigger && IsVillain(t.CardSource.Card.Owner) && !t.CardSource.Card.IsCharacter && !t.CardSource.Card.IsRelic && !t.CardSource.Card.IsMissionCard && (t.DoesMatchTypeAndCriteria(endOfTurn) || t.DoesMatchTypeAndCriteria(startOfTurn)) && t.CardSource.BattleZone == base.BattleZone);

                changeMe.ForEach(delegate (ITrigger t)
                {
                    AddTriggerSpy((PhaseChangeTrigger)t);
                });
            }
            yield return null;
            yield break;
        }

        private void AddTriggerSpy(PhaseChangeTrigger trigger)
        {
            if (!_surveilledTriggers.Contains(trigger))
            {
                //the CardSource check lets us examine whether the card would react to the *current* phase change without an infinite loop
                Func<PhaseChangeAction, bool> spy = (PhaseChangeAction pca) => (pca.CardSource != null && pca.CardSource.Card == this.Card) || UpdateActed(trigger, pca);

                trigger.AddAdditionalCriteria(spy);
                _surveilledTriggers.Add(trigger);
                _spies[trigger] = spy;
            }
        }

        private bool UpdateActed(PhaseChangeTrigger trigger, PhaseChangeAction pca)
        {
            if (GameController.RealMode && IsFirstOrOnlyCopyOfThisCardInPlay() && !trigger.Types.Any((TriggerType t) => t == TriggerType.Hidden || t == TriggerType.HiddenLast))
            {
                var TestPhaseChange = new PhaseChangeAction(GetCardSource(), pca.FromPhase, pca.ToPhase, true);
                if (trigger.DoesMatchTypeAndCriteria(TestPhaseChange))
                {
                    _justActivatedTrigger = trigger;
                }
                else
                {
                    _justActivatedTrigger = null;
                }
            }
            return true;
        }

        private IEnumerator ReapplyAllTriggersResponse(GameAction action)
        {
            IEnumerator coroutine = RestoreTriggersOnDestroy(action);
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

        private IEnumerator RestoreTriggersOnDestroy(GameAction action)
        {
            foreach (PhaseChangeTrigger trigger in _surveilledTriggers)
            {
                trigger.RemoveAdditionalCriteria(_spies[trigger]);
            }
            _surveilledTriggers.Clear();
            _spies.Clear();
            yield break;
        }  
    }
}