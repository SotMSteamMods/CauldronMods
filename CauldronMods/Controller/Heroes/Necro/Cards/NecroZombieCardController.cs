using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Linq;

namespace Cauldron.Necro
{
    public class NecroZombieCardController : UndeadCardController
    {
        public NecroZombieCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, 2)
        {
            SpecialStringMaker.ShowSpecialString(() => BuildUndeadSpecialString(highestOrLowest: true, cardCriteria: new LinqCardCriteria(c => !this.IsUndead(c) && IsHeroTargetConsidering1929(c), $"non-undead {HeroStringConsidering1929} target", useCardsSuffix: false)));
        }

        public override void AddTriggers()
        {
            //At the end of your turn, this card deals the non-Undead hero target with the highest HP 2 toxic damage.
            base.AddEndOfTurnTrigger(tt => tt == base.TurnTaker, p => base.DealDamageToHighestHP(base.Card, 1, c => !this.IsUndead(c) && IsHeroTargetConsidering1929(c), c => 2, DamageType.Toxic), TriggerType.DealDamage);
        }
    }
}
