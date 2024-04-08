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

        public FracturedControlRodCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            this.ShowIrradiatedCardsInHands(SpecialStringMaker);
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

            AddTrigger((MoveCardAction mc) => mc.IsDiscard && mc.CardToMove.IsIrradiated() && mc.CanChangeDestination && IsFirstOrOnlyCopyOfThisCardInPlay() && GameController.CanPlayCard(FindCardController(mc.CardToMove)) == CanPlayCardResult.CanPlay && GameController.IsTurnTakerVisibleToCardSource(mc.Origin.OwnerTurnTaker, GetCardSource()), PlayDiscardedCardFromMove, TriggerType.PlayCard, TriggerTiming.Before);
        }

        private IEnumerator PlayDiscardedCardFromMove(MoveCardAction mc)
        {
            //mc.SetDestination(mc.CardToMove.Owner.PlayArea);
            var cardToPlay = mc.CardToMove;
            var hero = cardToPlay.Owner.ToHero();
            var heroTTC = FindHeroTurnTakerController(hero);
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
                coroutine = this.ClearIrradiation(cardToPlay);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }

                coroutine = GameController.DestroyCard(heroTTC, this.Card, postDestroyAction: () => PlayCardAndCancelMove(mc, heroTTC, cardToPlay), cardSource: GetCardSource());
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

        private IEnumerator PlayCardAndCancelMove(MoveCardAction mc, HeroTurnTakerController heroTTC, Card cardToPlay)
        {
            var playStorage = new List<bool>();
            IEnumerator coroutine = GameController.PlayCard(heroTTC, cardToPlay, wasCardPlayed: playStorage, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            if(playStorage.Any(x => x))
            {
                coroutine = CancelAction(mc, showOutput: false);
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

        private IEnumerator MarkIrradiatedPlay(PlayCardAction pc)
        {
            WasPlayedIrradiated = Card.IsIrradiated();
            yield return null;
            yield break;
        }
    }
}
