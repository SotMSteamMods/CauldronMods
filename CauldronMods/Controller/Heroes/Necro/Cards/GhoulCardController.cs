using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Linq;

namespace Cauldron.Necro
{
    public class GhoulCardController : UndeadCardController
    {
        public GhoulCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, 2)
        {
            SpecialStringMaker.ShowSpecialString(() => BuildUndeadSpecialString(highestOrLowest: false, ranking: 2, cardCriteria: new LinqCardCriteria(c => !this.IsUndead(c) && IsHeroTargetConsidering1929(c), $"non-undead {HeroStringConsidering1929} target", useCardsSuffix: false)));
        }

        public override void AddTriggers()
        {
            //At the end of your turn, this card deals the non-undead hero target with the second lowest HP 2 toxic damage.
            base.AddEndOfTurnTrigger(tt => tt == TurnTaker, p => base.DealDamageToLowestHP(Card, 2, c => !this.IsUndead(c) && IsHeroTargetConsidering1929(c), c => 2, DamageType.Toxic), TriggerType.DealDamage);
        }
    }
}
