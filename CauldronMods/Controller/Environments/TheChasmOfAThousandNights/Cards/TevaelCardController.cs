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
            SpecialStringMaker.ShowNonEnvironmentTargetWithHighestHP(ranking: 3);
            AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
        }

        public override bool CanBeDestroyed => Card.HitPoints <= 0;

        public override IEnumerator DestroyAttempted(DestroyCardAction destroyCard)
        {
            return GameController.SendMessageAction($"{Card.Title} is indestructible while it has more than 0 HP.", Priority.Medium, GetCardSource());
        }

        public override bool AskIfCardIsIndestructible(Card card)
        {
            if (!CanBeDestroyed)
                return true;

            return base.AskIfCardIsIndestructible(card);
        }


        public override void AddTriggers()
        {
            //At the end of the environment turn, this card deals the non-environment target with the third highest HP 4 projectile damage.
            AddDealDamageAtEndOfTurnTrigger(TurnTaker, Card, (Card c) => c.IsNonEnvironmentTarget && GameController.IsCardVisibleToCardSource(c, GetCardSource()), TargetType.HighestHP, 4, DamageType.Projectile, highestLowestRanking: 3);
        }
    }
}
