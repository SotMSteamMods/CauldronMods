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
            SpecialStringMaker.ShowNumberOfCardsAtLocations(() => new Location[] { TurnTaker.Trash, HeroTurnTaker.Hand },
                                new LinqCardCriteria((Card c) => IsConstellation(c), "constellation"));
        }

        public override void AddTriggers()
        {
            //GameController.CanPerformPhaseAction(GameController.FindTurnPhase(TurnTaker, Phase.PlayCard))
            //GameController.FindNextTurnPhase

            //"At the start of your play phase, put a constellation card from your hand or trash into play."
            //because of how the game handles play phase with no cards in hand, this is actually "end of start phase"
            AddPhaseChangeTrigger((TurnTaker tt) => tt == TurnTaker,
                                (Phase p) => true,
                                (PhaseChangeAction pc) => PhaseChangeCriteria(pc),
                                PlayConstellationFromHandOrTrash,
                                new TriggerType[] { TriggerType.PutIntoPlay },
                                TriggerTiming.After);
        }

        private bool PhaseChangeCriteria(PhaseChangeAction action)
        {
            //get the phase that would come after this.
            var nextTurnPhase = GameController.FindNextTurnPhase(action.FromPhase);

            //The engine skips the play phase if the player has no cards, that's why we can't check the ToPhase directly.
            //There's also the case of Wager Master: Breaking the Rules that can reorder the phase order.
            //cases:
            //from: My Start, to: My Play
            //from: My Start, to: My Power, no cards in hand to play
            //from: My Power, to: My Play, Breaking the Rules

            //if the next phase would be my play card phase
            if (nextTurnPhase.Phase == Phase.PlayCard && nextTurnPhase.TurnTaker == TurnTaker)
            {
                //check that theres not a status effect that's about to skip my play phase
                //this is partially implementation dependant, but all the base set cards that skip phases use this status effect
                //except RealmOfDiscord:Time Crawls and Northspar:FrozenSolid which works correctly as the skip depends on the user's actions.
                if (GameController.StatusEffectControllers.Any(c => c.StatusEffect is PreventPhaseActionStatusEffect s && s.ToTurnPhaseCriteria.Phase == Phase.PlayCard && s.ToTurnPhaseCriteria.TurnTaker == TurnTaker))
                    return false;

                return true;
            }
            return false;
        }

        private IEnumerator PlayConstellationFromHandOrTrash(PhaseChangeAction pc)
        {
            string heroName = TurnTaker.Name;
            List<Function> actions = new List<Function>()
            {
                new Function(HeroTurnTakerController,
                                    "Play constellation from hand",
                                    SelectionType.PutIntoPlay,
                                    () => SelectAndPlayCardFromHand(HeroTurnTakerController, false, null, new LinqCardCriteria((Card c) => IsConstellation(c), "constellation"), true),
                                    GetPlayableCardsInHand(HeroTurnTakerController, true, pc.ToPhase).Any(IsConstellation)),
                new Function(HeroTurnTakerController,
                                    "Play constellation from trash",
                                    SelectionType.SearchTrash,
                                    () => SelectAndPutInPlayConstellationFromTrash(HeroTurnTakerController, pc.ToPhase),
                                    GetPuttableConstellationsFromTrash(HeroTurnTakerController, pc.ToPhase).Any()),
            };

            SelectFunctionDecision selectFunction = new SelectFunctionDecision(GameController, HeroTurnTakerController, actions, optional: false, pc, $"{heroName} had no constellations to put into play from their hand or trash.", cardSource: GetCardSource());
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
            return hero.TurnTaker.Trash.Cards.Where((Card card) => IsConstellation(card) && GameController.CanPlayCard(FindCardController(card), isPutIntoPlay: true, turnPhase) == CanPlayCardResult.CanPlay);
        }
    }
}