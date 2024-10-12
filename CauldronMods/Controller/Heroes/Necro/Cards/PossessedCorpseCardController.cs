using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Linq;

namespace Cauldron.Necro
{
    public class PossessedCorpseCardController : UndeadCardController
    {
        public PossessedCorpseCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController, 2)
        {
            SpecialStringMaker.ShowSpecialString(() => BuildUndeadSpecialString(highestOrLowest: false, cardCriteria: new LinqCardCriteria(c => !this.IsUndead(c) && IsHeroTargetConsidering1929(c), $"non-undead {HeroStringConsidering1929} target", useCardsSuffix: false)));
        }

        public override void AddTriggers()
        {
            //At the end of your turn, this card deals the non-undead target with the lowest HP 2 toxic damage.
            base.AddEndOfTurnTrigger(tt => tt == TurnTaker, p => base.DealDamageToLowestHP(Card, 1, c => !this.IsUndead(c) && IsHeroTargetConsidering1929(c), c => 2, DamageType.Infernal), TriggerType.DealDamage);
        }
    }
}
