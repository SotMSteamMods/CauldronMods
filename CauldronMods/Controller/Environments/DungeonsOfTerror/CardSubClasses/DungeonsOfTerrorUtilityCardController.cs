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
            //eventually, maybe add in reveal logic of some kind?
            int numFates = 0;
            foreach(Card card in cardsToCheck)
            {
                if(IsFate(card))
                {
                    numFates++;
                }
            }

            storedResults.Add(numFates);
            yield break;
        }
    }
}
