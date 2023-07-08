using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.OblaskCrater
{
    public class ShadowOfOblaskCardController : OblaskCraterUtilityCardController
    {
        /*
         * At the end of the environment turn, this card deals the hero target with the second lowest HP {H} energy damage.
         * If no other predator cards are in play, increase damage dealt by this card by 1.
         */
        public ShadowOfOblaskCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroTargetWithLowestHP(ranking: 2);
            SpecialStringMaker.ShowIfElseSpecialString(() => base.GameController.GetAllCards().Any(c => c != base.Card && c.IsInPlay && IsPredator(c)), () => "There is another predator card in play.", () => "There are no other predator cards in play.").Condition = () => Card.IsInPlayAndHasGameText;
        }

        public override void AddTriggers()
        {
            base.AddDealDamageAtEndOfTurnTrigger(TurnTaker, Card, (Card c) => IsHeroTarget(c) && GameController.IsCardVisibleToCardSource(c, GetCardSource()), TargetType.LowestHP, base.H, DamageType.Energy, highestLowestRanking: 2);
            base.AddIncreaseDamageTrigger((DealDamageAction dd) => dd.DamageSource.IsSameCard(Card) && !base.GameController.GetAllCards().Any(c => c != base.Card && c.IsInPlay && IsPredator(c)), 1);
        }
    }
}
