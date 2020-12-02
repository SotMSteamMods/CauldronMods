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
        private List<ITrigger> _triggersSuppressed;
        private Dictionary<ITrigger, Func<PhaseChangeAction, bool>> _suppressors;
        public HandIsFasterThanTheEyeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            _suppressors = new Dictionary<ITrigger, Func<PhaseChangeAction, bool>>();
            SpecialStringMaker.ShowSpecialString(() => "This might not handle being played in the middle of a start-of-turn or end-of-turn phase well.");
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
            //"When a non-relic, non-character villain card would activate start of turn or end of turn text, destroy it instead. Then, destroy this card.",
            AddStartOfTurnTrigger((TurnTaker tt) => true, CheckForSuppressedTriggerResponse, new TriggerType[] { TriggerType.DestroyCard });
            AddEndOfTurnTrigger((TurnTaker tt) => true, CheckForSuppressedTriggerResponse, new TriggerType[] { TriggerType.DestroyCard });
        }

        public override void AddLastTriggers()
        {
            _triggersSuppressed = new List<ITrigger> { };
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
            foreach (TurnTakerController item in base.GameController.TurnTakerControllers.Where((TurnTakerController ttc) => GameController.IsTurnTakerVisibleToCardSource(ttc.TurnTaker, GetCardSource()) && base.BattleZone == ttc.BattleZone))
            {
                TurnPhase start = item.TurnTaker.TurnPhases.Where((TurnPhase phase) => phase.IsStart).First();
                TurnPhase end = item.TurnTaker.TurnPhases.Where((TurnPhase phase) => phase.IsEnd).First();
                PhaseChangeAction startOfTurn = new PhaseChangeAction(GetCardSource(), null, start, start.IsEphemeral);
                PhaseChangeAction endOfTurn = new PhaseChangeAction(GetCardSource(), null, end, end.IsEphemeral);
                IEnumerable<ITrigger> changeMe = FindTriggersWhere((ITrigger t) => t is PhaseChangeTrigger && IsVillain(t.CardSource.Card.Owner) && !t.CardSource.Card.IsCharacter && !t.CardSource.Card.IsRelic && !t.CardSource.Card.IsMissionCard && (t.DoesMatchTypeAndCriteria(endOfTurn) || t.DoesMatchTypeAndCriteria(startOfTurn)) && t.CardSource.BattleZone == base.BattleZone);

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
                //Log.Debug($"Suppressing trigger on {trigger.CardSource.Card.Title}");
                Func<PhaseChangeAction, bool> stop = (PhaseChangeAction pca) => pca.CardSource != null && pca.CardSource.Card == this.Card;

                if (_suppressors.ContainsKey(trigger))
                {
                    trigger.RemoveAdditionalCriteria(_suppressors[trigger]);
                    _suppressors.Remove(trigger);
                }
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
            //Log.Debug("Trigger info: " + trigger.ToString());
            //Log.Debug("Suppressor record: " + _suppressors[trigger].ToString());
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
            _triggersSuppressed.Clear();
            yield return null;
            yield break;
        }
    }
}