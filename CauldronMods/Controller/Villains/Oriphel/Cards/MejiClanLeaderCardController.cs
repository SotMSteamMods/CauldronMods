using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Oriphel
{
    public class MejiClanLeaderCardController : OriphelUtilityCardController
    {
        public MejiClanLeaderCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //"Reduce damage dealt to Goons by 1.",
            AddReduceDamageTrigger((Card c) => IsGoon(c), 1);

            //"At the end of the villain turn, this card deals each hero target 3 melee damage."
            AddDealDamageAtEndOfTurnTrigger(TurnTaker, this.Card, (Card c) => IsHeroTarget(c), TargetType.All, 3, DamageType.Melee);
        }
    }
}