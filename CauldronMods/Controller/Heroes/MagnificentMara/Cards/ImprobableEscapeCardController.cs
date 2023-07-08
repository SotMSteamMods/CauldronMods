using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.MagnificentMara
{
    public class ImprobableEscapeCardController : CardController
    {
        public ImprobableEscapeCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //"When a hero character is reduced to 0 or fewer HP, restore them to 2HP, then destroy this card.",
            var heroCharacters = FindCardsWhere(new LinqCardCriteria((Card c) =>  IsHeroCharacterCard(c)));
            foreach (Card hero in heroCharacters)
            {
                AddWhenHPDropsToZeroOrBelowRestoreHPTriggers(() => hero, () => 2, true, preventDamage: false);
            }

            //"When this card or any of your ongoing cards are destroyed you may draw a card."
            AddTrigger((DestroyCardAction dc) => dc.CardToDestroy != null &&
                                                IsOngoing(dc.CardToDestroy.Card) &&
                                                dc.CardToDestroy.Card.Owner == TurnTaker &&
                                                dc.CardToDestroy.Card != this.Card &&
                                                dc.WasCardDestroyed,
                            (DestroyCardAction dc) => DrawCard(),
                            TriggerType.DrawCard,
                            TriggerTiming.After);
            AddWhenDestroyedTrigger((DestroyCardAction dc) => DrawCard(HeroTurnTaker), TriggerType.DrawCard);
         }
    }
}