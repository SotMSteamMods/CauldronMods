using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheMistressOfFate
{
    public class TheMistressOfFateCharacterCardController : VillainCharacterCardController
    {
        private TurnTaker HeroBeingRevived = null;
        public TheMistressOfFateCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddSideTriggers()
        {
            AddSideTrigger(AddTrigger<BulkMoveCardsAction>(IsCleaningUpIncappedHeroCards, PreserveHero, TriggerType.Hidden, TriggerTiming.Before));

            AddSideTrigger(AddTrigger((UnincapacitateHeroAction uha) => true, uha => SetRevivingHero(uha), TriggerType.Hidden, TriggerTiming.Before));
            AddSideTrigger(AddTrigger((ShuffleCardsAction sc) => sc.Location.OwnerTurnTaker == HeroBeingRevived, sc => CancelAction(sc), TriggerType.CancelAction, TriggerTiming.Before));
            AddSideTrigger(AddTrigger<BulkMoveCardsAction>(IsDefaultCardRestoration, RestoreHeroCards, TriggerType.Hidden, TriggerTiming.Before));
            AddSideTrigger(AddTrigger((UnincapacitateHeroAction uha) => true, uha => SetRevivingHero(uha, clear: true), TriggerType.Hidden, TriggerTiming.After, requireActionSuccess: false));

            AddSideTrigger(AddTrigger((GameOverAction go) => go.EndingResult == EndingResult.HeroesDestroyedDefeat, ContinueGameWithMessage, TriggerType.CancelAction, TriggerTiming.Before, priority: TriggerPriority.High));
            if (!Card.IsFlipped)
            {
                //"At the start of the villain turn, flip the left-most face down day card.",
                //"{TheMistressOfFate} is immune to villain damage. If there are no cards in the villain deck, the heroes lose.",
                //"Continue playing if all heroes are incapacitated. Incapacitated heroes keep the cards from their hand, deck, and trash separarte from each other when removing them from the game. Cards that were in play go to their trash."
                if(Game.IsAdvanced)
                {
                    //"advanced": "At the end of the villain turn, {TheMistressOfFate} deals the hero with the lowest HP {H} psychic damage.",
                }
            }
            else
            {
                //"Continue playing if all heroes are incapacitated.",
                //"When {TheMistressOfFate} to this side:",
                //"{Bulletpoint} Resore all other targets to their maximum HP.",
                //"{Bulletpoint} Destroy all environment cards.",
                //"{Bulletpoint} Flip all day cards face down.",
                //"{Bulletpoint} Remove all cards in the villain trash and the top {H - 1} cards of the villain deck from the game without looking at them.",
                //"{Bulletpoint} Flip all incapacitated heroes and restore them to their maximum HP. Cards that were in a hero's deck, hand, or trash when they were incapacitated are returned to those respective locations.",
                //"Then, flip {TheMistressOfFate}'s villain character cards."
                if(Game.IsAdvanced)
                {
                    //"flippedAdvanced": "When {TheMistressOfFate} flips to this side, she regains 10 HP.",
                }
            }
        }

        private IEnumerator ContinueGameWithMessage(GameOverAction go)
        {
            IEnumerator coroutine = GameController.SendMessageAction("The Timeline continues to turn...", Priority.High, GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            coroutine = CancelAction(go);
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
        private IEnumerator SetRevivingHero(UnincapacitateHeroAction uha, bool clear = false)
        {
            if (clear)
                HeroBeingRevived = null;
            else
                HeroBeingRevived = uha.HeroCharacterCard?.TurnTaker;
            yield return null;
            yield break;
        }
        private bool IsCleaningUpIncappedHeroCards(BulkMoveCardsAction bmc)
        {
            if(bmc.Destination.IsOutOfGame && bmc.Destination.OwnerTurnTaker.IsHero)
            {
                return true;
            }
            return false;
        }
        private IEnumerator PreserveHero(BulkMoveCardsAction bmc)
        {
            var hero = bmc.Destination.OwnerTurnTaker;
            var cardsInHand = hero.ToHero().Hand.Cards;
            var cardsInDeck = hero.Deck.Cards;
            var cardsInTrash = hero.Trash.Cards;
            IEnumerator coroutine = GameController.BulkMoveCards(TurnTakerController, cardsInHand, StorageLocation(hero, "Hand"));
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            coroutine = GameController.BulkMoveCards(TurnTakerController, cardsInDeck, StorageLocation(hero, "Deck"));
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            coroutine = GameController.BulkMoveCards(TurnTakerController, cardsInTrash, StorageLocation(hero, "Trash"));
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            var cardsRemaining = GameController.GetAllCards().Where((Card c) => !c.IsCharacter && c.Owner == hero && c.Location.HighestRecursiveLocation.IsInGame);
            coroutine = GameController.BulkMoveCards(TurnTakerController, cardsRemaining, StorageLocation(hero, "Trash"));
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = CancelAction(bmc);
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

        private IEnumerator RestoreHeroCards(BulkMoveCardsAction bmc)
        {
            var hero = bmc.Destination.OwnerTurnTaker;
            IEnumerator coroutine = GameController.BulkMoveCards(TurnTakerController, StorageLocation(hero, "Deck").Cards, hero.Deck, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = GameController.BulkMoveCards(TurnTakerController, StorageLocation(hero, "Hand").Cards, hero.ToHero().Hand, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = GameController.BulkMoveCards(TurnTakerController, StorageLocation(hero, "Trash").Cards, hero.Trash, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }

            coroutine = CancelAction(bmc);
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

        private bool IsDefaultCardRestoration(BulkMoveCardsAction bmc)
        {
            if (bmc.Destination.IsDeck && bmc.Destination.OwnerTurnTaker == HeroBeingRevived && bmc.CardSource == null)
            {
                return true;
            }
            return false;
        }

        private Location StorageLocation(TurnTaker hero, string variety)
        {
            var card = hero.OffToTheSide.Cards.Where((Card c) => c.Identifier == variety + "Storage").FirstOrDefault();
            if(card != null)
            {
                return card.UnderLocation;
            }
            return null;
        }
    }
}
