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

        protected IEnumerator SelectActiveCharacterCardToDealDamage(List<Card> storedResults, int damageAmount, DamageType damageType)
        {
            //future-proofing for Nightlore Council
            if (IsMultiCharPromo())
            {
                //TODO - get this working
            }
            else
            {
                storedResults.Add(TurnTaker.CharacterCard);
            }
            yield break;
        }

        protected bool IsMultiCharPromo()
        {
            return TurnTaker.HasMultipleCharacterCards;
        }

        protected List<Card> ListStarlights()
        {
            List<Card> starlights = new List<Card> { };
            if (IsMultiCharPromo())
            {
                //TODO - more futureproofing
            }
            else
            {
                starlights.Add(TurnTaker.CharacterCard);
            }
            return starlights;
        }
    }
}