﻿using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Cauldron.TheKnight
{
    public abstract class TheKnightCardController : CardController
    {
        public const string SingleHandKeyword = "single hand";
        private readonly string VigilarKey = "PastKnightVigilarKey";
        private readonly string RoninKey = "WastelandRoninKnightOwnershipKey";
        protected TheKnightCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowSpecialString(() => $"This card is currently being used by {VigilarOwnerName()}").Condition = () => VigilarOwnerName() != null;
            SpecialStringMaker.ShowSpecialString(() => $"This card is currently being used by {RoninOwnerName()}").Condition = () => RoninOwnerName() != null;
        }

        private string VigilarOwnerName()
        {
            var lentTo = GetCardPropertyJournalEntryCard(VigilarKey);
            if (lentTo != null)
            {
                return lentTo.Title;
            }
            return null;
        }
        private string RoninOwnerName()
        {
            var usedBy = GetCardPropertyJournalEntryCard(RoninKey);
            if (usedBy != null)
            {
                return usedBy.Title;
            }
            return null;
        }

        public bool IsSingleHandCard(Card card)
        {
            return card.DoKeywordsContain(SingleHandKeyword, evenIfUnderCard: true);
        }

        protected bool IsEquipmentEffectingCard(Card card)
        {
            return base.IsThisCardNextToCard(card) || (!base.Card.Location.IsNextToCard && card == base.CharacterCard);
        }

        protected bool IsOwnCharacterCard(Card card)
        {
            return card.IsHeroCharacterCard && card.ParentDeck == this.Card.ParentDeck;
        }

        public IEnumerator SelectOwnCharacterCard(List<SelectCardDecision> results, SelectionType selectionType)
        {
            if (base.HeroTurnTakerController.HasMultipleCharacterCards)
            {
                var criteria = new LinqCardCriteria(c => IsOwnCharacterCard(c), "hero character cards");
                var coroutine = base.GameController.SelectCardAndStoreResults(this.DecisionMaker, selectionType, criteria, results, false, cardSource: base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }
            else
            {
                var result = new SelectCardDecision(this.GameController, this.DecisionMaker, selectionType, new[] { base.CharacterCard }, false, true, cardSource: base.GetCardSource());
                result.ChooseIndex(0);
                result.AutoDecide();
                results.Add(result);
            }
            yield break;
        }

    }
}
