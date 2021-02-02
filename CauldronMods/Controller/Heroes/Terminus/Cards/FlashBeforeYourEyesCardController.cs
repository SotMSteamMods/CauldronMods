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
            base.SpecialStringMaker.ShowNumberOfCardsAtLocations(() => base.GameController.AllTurnTakers.Where(tt => CanSelectTrash(tt) && ((tt.IsEnvironment && tt.IsInGame == true) || (tt.IsHero && !tt.IsIncapacitatedOrOutOfGame) || (tt.IsVillain && !tt.IsIncapacitatedOrOutOfGame))).Select(tt=>tt.Trash).ToList());
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
            Func<TurnTaker, bool> func = delegate (TurnTaker tt)
            {
                bool flag = CanSelectTrash(tt);
                if (flag)
                {
                    Location location = tt.Trash;

                    flag &= (tt.IsEnvironment && tt.IsInGame == true) || (tt.IsHero && !tt.IsIncapacitatedOrOutOfGame) || (tt.IsVillain && !tt.IsIncapacitatedOrOutOfGame);
                }
                return flag;
            };
            List<SelectTurnTakerDecision> selectTurnTakerDecisions = new List<SelectTurnTakerDecision>();

            coroutine = base.GameController.SelectTurnTakersAndDoAction(
                null,
                new LinqTurnTakerCriteria(func, "trash with cards to move"),
                SelectionType.MoveCardOnDeck,
                (tt) => base.GameController.SelectCardsFromLocationAndMoveThem(base.HeroTurnTakerController, tt.Trash, 1, 1, criteria, new List<MoveCardDestination>
                    {
                        new MoveCardDestination(tt.Deck)
                    }),
                1,
                false,
                1,
                selectTurnTakerDecisions,
                cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (selectTurnTakerDecisions != null && selectTurnTakerDecisions.Count() > 0)
            {
                AddCardProperty(selectTurnTakerDecisions.FirstOrDefault().SelectedTurnTaker);
            }

            yield break;
        }

        private bool CanSelectTrash(TurnTaker turnTaker)
        {
            IEnumerable<string> trashPropertyKeys = base.Game.Journal.GetCardPropertiesStringList(base.Card, TrashPropertyListKey);
            bool canSelect = true;
            string deckKey = $"{turnTaker.DeckDefinition.Name}:{turnTaker.DeckDefinition.Kind}:{turnTaker.DeckDefinition.ExpansionIdentifier}";

            if (trashPropertyKeys != null && trashPropertyKeys.Count() > 0 &&  trashPropertyKeys.Contains(deckKey))
            {
                canSelect = false;
            }

            return canSelect;
        }

        private void AddCardProperty(TurnTaker turnTaker)
        {
            List<string> trashPropertyKeys = new List<string>();
            string deckKey = BuildDeckKey(turnTaker);

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
            IEnumerable<string> trashPropertyKeys = base.Game.Journal.GetCardPropertiesStringList(base.Card, TrashPropertyListKey);

            foreach (string key in trashPropertyKeys)
            {
                base.Game.Journal.RemoveCardProperties(base.Card, key);
            }

            yield break;
        }

        private string BuildDeckKey(TurnTaker turnTaker)
        {
            return $"{turnTaker.DeckDefinition.Name}:{turnTaker.DeckDefinition.Kind}:{turnTaker.DeckDefinition.ExpansionIdentifier}";
        }
    }
}
