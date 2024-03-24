using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Oriphel
{
    public class MejiNomadCardController : OriphelUtilityCardController
    {
        public MejiNomadCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowHeroTargetWithHighestHP();
            SpecialStringMaker.ShowNumberOfCardsInPlay(new LinqCardCriteria((Card c) => IsGuardian(c), "guardian"));
        }

        public override void AddTriggers()
        {
            //"At the end of the villain turn, this card deals the hero target with the highest HP X projectile damage, where X is the number of Guardians in play plus 2.",
            AddDealDamageAtEndOfTurnTrigger(TurnTaker, this.Card, (Card c) => IsHeroTarget(c), TargetType.HighestHP, 2, DamageType.Projectile, dynamicAmount: NumberOfGuardiansPlusTwo);
        }

        private int? NumberOfGuardiansPlusTwo(Card irrelevant)
        {
            int guardians = GameController.FindCardsWhere((Card c) => c.IsInPlayAndHasGameText && GameController.IsCardVisibleToCardSource(c, GetCardSource()) && IsGuardian(c)).Count();
            return guardians + 2;

        }
    }
}