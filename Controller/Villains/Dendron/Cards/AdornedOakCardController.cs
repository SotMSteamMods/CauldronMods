
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace Cauldron.Dendron
{
    public class AdornedOakCardController : DendronBaseCardController
    {
        //==============================================================
        // Reduce damage dealt to Tattoos by 1.
        //==============================================================

        public static readonly string Identifier = "AdornedOak";

        private const int DamageAmountToReduce = 1;

        public AdornedOakCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            base.AddReduceDamageTrigger(IsTattoo, DamageAmountToReduce);

            base.AddTriggers();
        }
    }
}