using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Handelabra;

namespace Cauldron.TheKnight
{
    public abstract class TheKnightCardController : CardController
    {
        public const string SingleHandKeyword = "single hand";
        private readonly string VigilarKey = "PastKnightVigilarKey";
        protected readonly string RoninKey = "WastelandRoninKnightOwnershipKey";
        protected TheKnightCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowSpecialString(() => $"This card is currently being used by {VigilarOwnerName()}").Condition = () => this.Card.Location.OwnerTurnTaker != this.TurnTaker && VigilarOwnerName() != null;
            SpecialStringMaker.ShowSpecialString(() => $"This card is currently being used by {RoninOwnerName()}").Condition = () => IsMultiCharPromo() && RoninOwnerName() != null;
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
            return card != null && card.DoKeywordsContain(SingleHandKeyword, evenIfUnderCard: true);
        }

        protected bool IsEquipmentEffectingCard(Card card)
        {
            if (IsMultiCharPromo())
            {
                if(this.Card.Location.IsNextToCard)
                {
                    return card == this.Card.Location.OwnerCard;
                }
                else
                {
                    var owner = GetCardPropertyJournalEntryCard(RoninKey);
                    return card == owner;
                }
            }
            else
            {
                return card == base.CharacterCard;
            }
        }

        protected bool IsOwnCharacterCard(Card card)
        {
            return IsHeroCharacterCard(card) && card.ParentDeck == this.Card.ParentDeck;
        }

        public IEnumerator SelectOwnCharacterCard(List<SelectCardDecision> results, SelectionType selectionType)
        {
            if (IsMultiCharPromo())
            {
                var criteria = new LinqCardCriteria(c => IsOwnCharacterCard(c) && c.IsInPlayAndHasGameText && !c.IsIncapacitatedOrOutOfGame, "hero character cards");
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

        protected bool IsMultiCharPromo()
        {
            return this.TurnTakerControllerWithoutReplacements.HasMultipleCharacterCards && TurnTakerControllerWithoutReplacements is TheKnightTurnTakerController kttc && kttc.InstructionCardController != null;
        }

        protected Card GetKnightCardUser(Card c)
        {
            if (c == null)
            {
                return null;
            }

            if (IsMultiCharPromo())
            {
                if (c.Location.IsNextToCard)
                {
                    return c.Location.OwnerCard;
                }

                if(c.Owner == this.TurnTaker)
                {
                    var propCard = GameController.GetCardPropertyJournalEntryCard(c, RoninKey);
                    return propCard ?? this.CharacterCard;
                }
            }

            return this.CharacterCard;
        }

        // Lightly modified copy of CardController.AdjustTargetnessResponse, which is unfortunately private.
        protected IEnumerator AdjustTargetnessResponseNotPrivate(GameAction a, Card card, int maxHP)
        {
            if (a is RemoveTargetAction rta)
            {
                IEnumerator coroutine = CancelAction(rta);
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
                rta.CardToRemoveTarget.SetMaximumHP(maxHP, alsoSetHP: false);
            }
            else if (a is BulkRemoveTargetsAction brta)
            {
                brta.RemoveCardsFromBulkProcess(c => c == card).ForEach(delegate (Card c)
                {
                    c.SetMaximumHP(maxHP, alsoSetHP: false);
                });
            }
        }
    }
}
