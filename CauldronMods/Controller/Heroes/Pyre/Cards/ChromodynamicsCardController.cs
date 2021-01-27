using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Pyre
{
    public class ChromodynamicsCardController : PyreUtilityCardController
    {
        private List<Guid> _recentIrradiatedPlays;
        private List<Guid> RecentIrradiatedCardPlays
        {
            get
            {
                if(_recentIrradiatedPlays == null)
                {
                    _recentIrradiatedPlays = new List<Guid>();
                }
                return _recentIrradiatedPlays;
            }
        }
        public ChromodynamicsCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //"Whenever a player plays a {PyreIrradiate} card, {Pyre} deals 1 target 1 energy damage."
            AddTrigger((PlayCardAction pc) => IsIrradiated(pc.CardToPlay) && !pc.IsPutIntoPlay, NoteIrradiatedPlay, TriggerType.Hidden, TriggerTiming.Before);
            AddTrigger((PlayCardAction pc) => RecentIrradiatedCardPlays.Contains(pc.InstanceIdentifier), IrradiatedPlayResponse, TriggerType.DealDamage, TriggerTiming.After, requireActionSuccess: false);
        }
        private IEnumerator NoteIrradiatedPlay(PlayCardAction pc)
        {
            RecentIrradiatedCardPlays.Add(pc.InstanceIdentifier);
            yield return null;
            yield break;
        }
        private IEnumerator IrradiatedPlayResponse(PlayCardAction pc)
        {
            RecentIrradiatedCardPlays.Remove(pc.InstanceIdentifier);
            if(pc.IsSuccessful)
            {
                IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), 1, DamageType.Energy, 1, false, 1, cardSource: GetCardSource());
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
        public override IEnumerator UsePower(int index = 0)
        {
            int numTargets = GetPowerNumeral(0, 2);
            int numDamage = GetPowerNumeral(1, 2);
            //"Discard a card.
            IEnumerator coroutine = GameController.SelectAndDiscardCard(DecisionMaker, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            //{Pyre} deals 2 targets 2 lightning damage each.
            coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), numDamage, DamageType.Lightning, numTargets, false, numTargets, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }
    }
}
