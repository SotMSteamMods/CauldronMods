using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.DungeonsOfTerror
{
    public abstract class DungeonsOfTerrorUtilityCardController : CardController
    {
        protected DungeonsOfTerrorUtilityCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public static readonly string FateKeyword = "fate";

        protected bool IsFate(Card card)
        {
            return card.DoKeywordsContain(FateKeyword);
        }

        protected IEnumerator CheckForNumberOfFates(IEnumerable<Card> cardsToCheck, List<int> storedResults)
        { 
            int numFates = 0;
            if(!cardsToCheck.Any())
            {
                yield break;
            }

            
            foreach (Card card in cardsToCheck)
            {
                if(card != null && IsFate(card))
                {
                    numFates++;
                }
            }
            storedResults.Add(numFates);
            yield break;
        }

        protected bool? IsTopCardOfLocationFate(Location location)
        {
            Card card = location.TopCard;
            if (card == null)
            {
                return null;
            }

            return IsFate(card);
        }

        protected string BuildTopCardOfLocationSpecialString(Location location)
        {
            bool? fate = IsTopCardOfLocationFate(location);
            string special = "";
            if(fate == null)
            {
                special = $"There are no cards in {location.GetFriendlyName()}.";
            } else

            {
                special = $"The top card of {location.GetFriendlyName()} is ";
                if(fate.Value == false)
                {
                    special += "not ";
                }
                special += "a fate card.";
            }

            return special;
        }
    }
}
