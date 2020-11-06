using Handelabra.Sentinels.Engine.Controller;
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

        protected TheKnightCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        protected bool IsSingleHandCard(Card card)
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

        protected IEnumerator SelectOwnCharacterCard(List<SelectCardDecision> result, SelectionType selectionType)
        {
            if (base.HeroTurnTakerController.HasMultipleCharacterCards)
            {
                var criteria = new LinqCardCriteria(c => IsOwnCharacterCard(c), "hero character cards");
                var coroutine = base.GameController.SelectCardAndStoreResults(this.DecisionMaker, selectionType, criteria, result, false, cardSource: base.GetCardSource());
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
                result.Add(new SelectCardDecision(this.GameController, this.DecisionMaker, selectionType, new[] { base.CharacterCard }, false, true, cardSource: base.GetCardSource()));
            }
            yield break;
        }

    }
}
