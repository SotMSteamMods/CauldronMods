using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Starlight
{
    public class StarlightCardController : CardController
    {
        public StarlightCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        protected bool IsConstellation(Card card)
        {
            return (card != null) && GameController.DoesCardContainKeyword(card, "constellation");
        }

        protected bool IsNextToConstellation(Card card)
        {
            if (card != null && card.NextToLocation != null && card.NextToLocation.Cards != null)
            {
                int num = card.NextToLocation.Cards.Where((Card c) => IsConstellation(c) && c.IsInPlayAndHasGameText).Count();
                return num > 0;
            }
            return false;
        }

        protected IEnumerator SelectActiveCharacterCardToDealDamage(List<Card> storedResults, int? damageAmount = null, DamageType? damageType = null)
        {
            //future-proofing for Nightlore Council
            if (IsMultiCharPromo())
            {
                List<SelectCardDecision> storedDecision = new List<SelectCardDecision>();

                IEnumerator coroutine;
                if (damageAmount == null || damageType == null)
                {
                    coroutine = base.GameController.SelectCardAndStoreResults(this.HeroTurnTakerController, SelectionType.HeroCharacterCard, new LinqCardCriteria((Card c) => c.Owner == base.TurnTaker && c.IsCharacter && !c.IsIncapacitatedOrOutOfGame, "active Starlight"), storedDecision, optional: false, allowAutoDecide: false, includeRealCardsOnly: true, cardSource: GetCardSource());
                }
                else
                {
                    
                    DealDamageAction previewDamage = new DealDamageAction(GetCardSource(), null, null, (int)damageAmount, (DamageType)damageType);
                    coroutine = base.GameController.SelectCardAndStoreResults(this.HeroTurnTakerController, SelectionType.HeroToDealDamage, new LinqCardCriteria((Card c) => c.Owner == base.TurnTaker && c.IsCharacter && !c.IsIncapacitatedOrOutOfGame, "active Starlight"), storedDecision, optional: false, allowAutoDecide: false, previewDamage, includeRealCardsOnly: true, GetCardSource());
                }

                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                SelectCardDecision selectCardDecision = storedDecision.FirstOrDefault();
                if (selectCardDecision != null)
                {
                    storedResults.Add(selectCardDecision.SelectedCard);
                }
            }
            else
            {
                storedResults.Add(TurnTaker.CharacterCard);
            }
            yield break;
        }

        protected bool IsMultiCharPromo()
        {
            return HeroTurnTakerController.HasMultipleCharacterCards;
        }

        protected List<Card> ListStarlights()
        {
            return this.CharacterCards.ToList();
        }
    }
}