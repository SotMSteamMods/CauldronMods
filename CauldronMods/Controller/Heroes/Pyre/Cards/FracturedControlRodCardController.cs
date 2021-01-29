using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Pyre
{
    public class FracturedControlRodCardController : PyreUtilityCardController
    {
        private bool WasPlayedIrradiated = false;
        private List<Guid> _recentIrradiatedDiscards;
        private List<Guid> RecentIrradiatedDiscards
        {
            get
            {
                if(_recentIrradiatedDiscards == null)
                {
                    _recentIrradiatedDiscards = new List<Guid>();
                }
                return _recentIrradiatedDiscards;
            }
        }
        public FracturedControlRodCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddStartOfGameTriggers()
        {
            AddTrigger((PlayCardAction pc) => pc.CardToPlay == Card, MarkIrradiatedPlay, TriggerType.Hidden, TriggerTiming.Before, outOfPlayTrigger: true);
        }
        public override IEnumerator Play()
        {
            //"If this card is {PyreIrradiate} when you play it, {Pyre} deals 1 target 3 toxic damage.",
            if(WasPlayedIrradiated)
            {
                WasPlayedIrradiated = false;
                IEnumerator coroutine = GameController.SelectTargetsAndDealDamage(DecisionMaker, new DamageSource(GameController, CharacterCard), 3, DamageType.Toxic, 1, false, 1, cardSource: GetCardSource());
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

        public override void AddTriggers()
        {
            //"Whenever a player discards a {PyreIrradiate} card, they may destroy this card to play the discarded card."
            AddTrigger((DiscardCardAction dc) => IsIrradiated(dc.CardToDiscard), MarkIrradiatedDiscard, TriggerType.Hidden, TriggerTiming.Before);
            AddTrigger((DiscardCardAction dc) => RecentIrradiatedDiscards.Contains(dc.InstanceIdentifier), PlayDiscardedCardResponse, TriggerType.PlayCard, TriggerTiming.After, isActionOptional: true);
        }

        private IEnumerator PlayDiscardedCardResponse(DiscardCardAction dc)
        {
            RecentIrradiatedDiscards.Remove(dc.InstanceIdentifier);
            if(dc.IsSuccessful)
            {
                var cardToPlay = dc.CardToDiscard;
                var hero = dc.CardToDiscard.Owner.ToHero();
                var heroTTC = dc.HeroTurnTakerController ?? FindHeroTurnTakerController(hero);
                var storedYesNo = new List<YesNoCardDecision>();
                IEnumerator coroutine = GameController.MakeYesNoCardDecision(heroTTC, SelectionType.PlayCard, cardToPlay, storedResults: storedYesNo, cardSource: GetCardSource());
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
                    coroutine = GameController.DestroyCard(heroTTC, this.Card, postDestroyAction: () => GameController.PlayCard(heroTTC, cardToPlay, cardSource: GetCardSource()), cardSource: GetCardSource());
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

        private IEnumerator MarkIrradiatedPlay(PlayCardAction pc)
        {
            WasPlayedIrradiated = IsIrradiated(Card);
            yield return null;
            yield break;
        }
        private IEnumerator MarkIrradiatedDiscard(DiscardCardAction dc)
        {
            RecentIrradiatedDiscards.Add(dc.InstanceIdentifier);
            yield return null;
            yield break;
        }
    }
}
