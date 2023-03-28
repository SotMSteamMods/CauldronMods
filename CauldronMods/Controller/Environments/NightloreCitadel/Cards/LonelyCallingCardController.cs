using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.NightloreCitadel
{
    public class LonelyCallingCardController : NightloreCitadelUtilityCardController
    {
        public LonelyCallingCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowSpecialString(() => BuildKeywordsOnLastCardSpecialString());
            SpecialStringMaker.ShowSpecialString(() => BuildNumberOfCardsThisRoundSpecialString()).Condition = () => Card.IsInPlayAndHasGameText;
        }

        public override void AddTriggers()
        {
            //Players may not play cards that share a keyword with the last hero card that entered play.
            CannotPlayCards(cardCriteria: (Card c) => c.Location.IsHero && DoesCardShareKeywordWithLastHeroCard(c));
            //At the start of the environment turn, if no hero cards entered play this round, destroy this card
            AddStartOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, DestroyThisCardResponse, TriggerType.DestroySelf, additionalCriteria: (PhaseChangeAction pca) => GetNumberOfHeroCardsEnteredPlayThisRound() == 0);
        }

        private bool DoesCardShareKeywordWithLastHeroCard(Card c)
        {
            Card lastCard = GetLastHeroCardToEnterPlay();
            if(lastCard == null)
            {
                return false;
            }
            var keywords = new HashSet<string>(GameController.GetAllKeywords(lastCard), StringComparer.OrdinalIgnoreCase);
            var sharesAKeyword = GameController.GetAllKeywords(c).Any(k => keywords.Contains(k));
            return sharesAKeyword;
        }

        private Card GetLastHeroCardToEnterPlay()
        {
            CardEntersPlayJournalEntry playJournalEntry = Game.Journal.CardEntersPlayEntries().Where(cpe => IsHero(cpe.Card) && cpe.Card.BattleZone == Card.BattleZone).LastOrDefault();
            if(playJournalEntry != null)
            {
                return playJournalEntry.Card;
            }

            return null;
            
        }

        private int GetNumberOfHeroCardsEnteredPlayThisRound()
        {
            int cardsEnteredPlayThisRound = Game.Journal.CardEntersPlayEntries().Where(cpe => IsHero(cpe.Card) && cpe.Round == Game.Round).Count();
            return cardsEnteredPlayThisRound;
        }

        private string BuildKeywordsOnLastCardSpecialString()
        {
            Card lastCard = GetLastHeroCardToEnterPlay();
            if(lastCard == null)
            {
                return "No hero cards have entered play.";
            }
            string keywordsSpecials = "Keywords on the last hero card to enter play: ";
            var keywords = GameController.GetAllKeywords(lastCard);
            keywordsSpecials += String.Join(", ", keywords.ToArray());

            return keywordsSpecials;

        }

        private string BuildNumberOfCardsThisRoundSpecialString()
        {
            int num = GetNumberOfHeroCardsEnteredPlayThisRound();
            string numberSpecial = "";
            if(num == 1)
            {
                numberSpecial += "1 hero card has ";
            } else
            {
                numberSpecial += $"{num} hero cards have ";
            }

            numberSpecial += "entered play this round.";

            return numberSpecial;
        }
    }
}
