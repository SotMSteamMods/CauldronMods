using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheChasmOfAThousandNights
{
    public class TevaelCardController : TheChasmOfAThousandNightsUtilityCardController
    {
        public TevaelCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
            SpecialStringMaker.ShowNonEnvironmentTargetWithHighestHP(ranking: 3);
        }

        public override bool AskIfCardIsIndestructible(Card card)
        {
            //This card is indestructible while it has more than 0 HP.
            return card == Card && card.HitPoints > 0;
        }

        public override void AddTriggers()
        {
            //At the end of the environment turn, this card deals the non-environment target with the third highest HP 4 projectile damage.
            AddDealDamageAtEndOfTurnTrigger(TurnTaker, Card, (Card c) => c.IsNonEnvironmentTarget && GameController.IsCardVisibleToCardSource(c, GetCardSource()), TargetType.HighestHP, 4, DamageType.Projectile, highestLowestRanking: 3);
        }
    }
}
