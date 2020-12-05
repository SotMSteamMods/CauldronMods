using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Oriphel
{
    public class ShardbearerNathanielCardController : OriphelUtilityCardController
    {
        public ShardbearerNathanielCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            //"Increase damage dealt by Guardians and {Oriphel} by 1."
            AddIncreaseDamageTrigger((DealDamageAction dd) => dd.DamageSource.IsCard && (IsGuardian(dd.DamageSource.Card) || dd.DamageSource.Card == oriphelIfInPlay), 1);
        }
    }
}