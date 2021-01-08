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
