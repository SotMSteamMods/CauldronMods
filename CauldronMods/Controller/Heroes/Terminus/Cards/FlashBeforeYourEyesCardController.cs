using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Terminus
{
    public class FlashBeforeYourEyesCardController : TerminusBaseCardController
    {
        /*
         * At the end of your turn, select a trash pile and put a card from it on top of its associated deck.
         * You cannot select that trash pile again for this effect until this card leaves play.
        */
        private string TrashPropertyListKey =>  $"TrashPropertyListKey{base.Card.InstanceIndex}";

        public FlashBeforeYourEyesCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowNumberOfCardsAtLocations(() => AllVisibleTrashes().Where(lc => CanSelectTrash(lc)).Select(lc => lc.Location).ToList());
        }

        public override void AddTriggers()
        {
            //promising
            //base.Game.Journal.GetCardPropertiesStringList
            //base.Game.Journal.RemoveCardProperties
            //base.Game.Journal.RecordCardProperties
            base.AddEndOfTurnTrigger((tt) => tt == base.TurnTaker, PhaseChangeActionResponse, TriggerType.PutOnDeck);
            AddAfterLeavesPlayAction(RemoveAllCardProperties, TriggerType.Hidden);
        }
        private IEnumerator PhaseChangeActionResponse(PhaseChangeAction phaseChangeAction)
        {
            IEnumerator coroutine;
            List<SelectCardDecision> storedResults = new List<SelectCardDecision>();
            LinqCardCriteria criteria = new LinqCardCriteria((card) => card.IsInTrash, "a card in trash", useCardsSuffix: false);
            Func<LocationChoice, bool> notChosen = delegate (LocationChoice lc)
            {
                return CanSelectTrash(lc);
            };

            LocationChoice? forcedTrash = null;
            var selectTrashDecision = new SelectLocationDecision(GameController, DecisionMaker, AllVisibleTrashes().Where(notChosen), SelectionType.MoveCardOnDeck, false, cardSource: GetCardSource());
            if(selectTrashDecision.NumberOfChoices == 0)
            {
                coroutine = GameController.SendMessageAction($"No trashes can be chosen for {Card.Title}.", Priority.Medium, GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                yield break;
            }

            if (selectTrashDecision.NumberOfChoices == 1)
            {
                forcedTrash = selectTrashDecision.Choices.FirstOrDefault();
                coroutine = GameController.SendMessageAction($"The only trash that {Card.Title} can select is {forcedTrash?.Location.GetFriendlyName()}.", Priority.Medium, GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                coroutine = MoveCardFromTrashToDeck(forcedTrash?.Location);
            }
            else
            {
                coroutine = GameController.SelectLocationAndDoAction(selectTrashDecision, MoveCardFromTrashToDeck);
            }
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if(forcedTrash != null)
            {
                AddCardProperty(forcedTrash.Value);
            }
            else if(selectTrashDecision.Completed && selectTrashDecision.Index != null)
            {
                AddCardProperty(selectTrashDecision.SelectedLocation);
            }
            yield break;
        }
        private IEnumerator MoveCardFromTrashToDeck(Location trash)
        {
            var destination = trash.OwnerTurnTaker.Deck;
            if(trash.IsSubTrash)
            {
                var foundDeck = trash.OwnerTurnTaker.SubDecks.Where(deck => deck.Identifier == trash.Identifier).FirstOrDefault();
                if(foundDeck != null)
                {
                    destination = foundDeck;
                }
            }

            return GameController.SelectCardFromLocationAndMoveIt(DecisionMaker, trash, new LinqCardCriteria(x => true), new List<MoveCardDestination> { new MoveCardDestination(destination) }, showOutput: true, responsibleTurnTaker: TurnTaker, cardSource: GetCardSource());
        }
        private List<LocationChoice> AllVisibleTrashes()
        {
            return FindLocationsWhere((Location L) => base.GameController.IsLocationVisibleToSource(L, GetCardSource()) && L.IsTrash && L.IsRealTrash).Select((Location L) => new LocationChoice(L)).ToList();
        }
        private bool CanSelectTrash(LocationChoice lc)
        {
            IEnumerable<string> trashPropertyKeys = base.Game.Journal.GetCardPropertiesStringList(base.Card, TrashPropertyListKey);
            bool canSelect = true;
            string trashKey = $"{lc.Location.GetFriendlyName()}:{lc.Location.OwnerTurnTaker.DeckDefinition.Kind}:{lc.Location.OwnerTurnTaker.DeckDefinition.ExpansionIdentifier}";

            if (trashPropertyKeys != null && trashPropertyKeys.Count() > 0 && trashPropertyKeys.Contains(trashKey))
            {
                canSelect = false;
            }

            return canSelect;
        }
        private void AddCardProperty(LocationChoice lc)
        {
            List<string> trashPropertyKeys = new List<string>();
            string deckKey = BuildDeckKey(lc);

            if (base.Game.Journal.GetCardPropertiesStringList(base.Card, TrashPropertyListKey) != null)
            {
                trashPropertyKeys = base.Game.Journal.GetCardPropertiesStringList(base.Card, TrashPropertyListKey).ToList();
            }

            if (!trashPropertyKeys.Contains(deckKey))
            {
                trashPropertyKeys.Add(deckKey);
                base.Game.Journal.RecordCardProperties(base.Card, TrashPropertyListKey, trashPropertyKeys);
            }
        }

        private IEnumerator RemoveAllCardProperties(GameAction gameAction)
        {
            if(IsRealAction())
            {
                Game.Journal.RecordCardProperties(base.Card, TrashPropertyListKey, new List<string> { });
            }
            yield return null;
            yield break;
        }
        private string BuildDeckKey(LocationChoice lc)
        {
            return $"{lc.Location.GetFriendlyName()}:{lc.Location.OwnerTurnTaker.DeckDefinition.Kind}:{lc.Location.OwnerTurnTaker.DeckDefinition.ExpansionIdentifier}";
        }
    }
}
