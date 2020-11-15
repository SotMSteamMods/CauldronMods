using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Starlight
{
    public class PillarsOfCreationCardController : StarlightCardController
    {
        public PillarsOfCreationCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //"At the start of your play phase, put a constellation card from your hand or trash into play."
            //because of how the game handles play phase with no cards in hand, this is actually "end of start phase"
            AddPhaseChangeTrigger((TurnTaker tt) => tt == base.TurnTaker,
                                (Phase p) => true,
                                (PhaseChangeAction pc) => pc.FromPhase.Phase == Phase.Start,
                                PlayConstellationFromHandOrTrash,
                                new TriggerType[1] { TriggerType.PutIntoPlay },
                                TriggerTiming.Before);
        }

        private IEnumerator PlayConstellationFromHandOrTrash(PhaseChangeAction pc)
        {
            List<Function> actions = new List<Function> { };
            string heroName = TurnTaker.Name;


            actions.Add(new Function(HeroTurnTakerController,
                                    "Play constellation from hand",
                                     SelectionType.PutIntoPlay,
                                     () => SelectAndPlayCardFromHand(HeroTurnTakerController, false, null, new LinqCardCriteria((Card c) => IsConstellation(c)), true),
                                     GetPlayableCardsInHand(HeroTurnTakerController, true, pc.ToPhase).Where(IsConstellation).Count() > 0));
            actions.Add(new Function(HeroTurnTakerController,
                                    "Play constellation from trash",
                                    SelectionType.SearchTrash,
                                    () => SelectAndPutInPlayConstellationFromTrash(HeroTurnTakerController, pc.ToPhase),
                                    GetPuttableConstellationsFromTrash(HeroTurnTakerController, pc.ToPhase).Count() > 0));

            SelectFunctionDecision selectFunction = new SelectFunctionDecision(GameController, HeroTurnTakerController, actions, optional: false, pc, heroName + " had no constellations to put into play from their hand or trash.", cardSource: GetCardSource());
            var coroutine = GameController.SelectAndPerformFunction(selectFunction);
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

        private IEnumerator SelectAndPutInPlayConstellationFromTrash(HeroTurnTakerController hero, TurnPhase turnPhase)
        {
            var coroutine = GameController.SelectAndPlayCard(hero, GetPuttableConstellationsFromTrash(hero, turnPhase), optional: false, isPutIntoPlay: true, cardSource: GetCardSource());
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

        private IEnumerable<Card> GetPuttableConstellationsFromTrash(HeroTurnTakerController hero, TurnPhase turnPhase = null)
        {
            return hero.TurnTaker.Trash.Cards.Where((Card card) => IsConstellation(card) && GameController.CanPlayCard(FindCardController(card), isPutIntoPlay: true, turnPhase, evenIfAlreadyInPlay: false, canBeCancelled: true) == CanPlayCardResult.CanPlay);
        }
    }
}