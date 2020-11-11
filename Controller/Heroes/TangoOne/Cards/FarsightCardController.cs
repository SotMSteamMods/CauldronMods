
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TangoOne
{
    public class FarsightCardController : TangoOneBaseCardController
    {

        //==============================================================
        // Damage dealt by {TangoOne} Is irreducible.
        //==============================================================

        public static string Identifier = "Farsight";

        public FarsightCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {

            base.AddMakeDamageIrreducibleTrigger((DealDamageAction dd) => dd.DamageSource.IsSameCard(this.Card.Owner.CharacterCard));

            base.AddTriggers();
        }
    }
}