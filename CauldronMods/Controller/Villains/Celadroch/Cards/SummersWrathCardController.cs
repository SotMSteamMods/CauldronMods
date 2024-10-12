using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Celadroch
{
    public class SummersWrathCardController : CardController
    {
        /*
         * 	"At the end of the villain turn, this card deals each hero target 2 fire damage.",
			"This card is immune to damage unless another villain target has been dealt damage this turn."
         */

        public SummersWrathCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowSpecialString(() => $"{Card.Title} is immune to damage.")
                .Condition = () => Card.IsInPlayAndHasGameText && ImmuneToDamageCriteria();
        }

        private bool ImmuneToDamageCriteria()
        {
            return !Journal.DealDamageEntriesThisTurn().Any(j => j.TargetCard != Card && IsVillainTarget(j.TargetCard) && j.Amount > 0);
        }

        public override void AddTriggers()
        {
            AddDealDamageAtEndOfTurnTrigger(TurnTaker, Card, c => IsHeroTarget(c), TargetType.All, 2, DamageType.Fire);

            AddImmuneToDamageTrigger(dda => dda.Target == Card && ImmuneToDamageCriteria());
        }
    }
}
