
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace Cauldron.Vector
{
    public class BloodSampleCardController : CardController
    {
        //==============================================================
        // When this card enters play, {Vector} deals each hero target 1 toxic damage.
        // At the start of the villain turn, if Supervirus is in play
        // and {Vector} was dealt {H x 2} or more damage last round,
        // you may put this card beneath Supervirus.
        //==============================================================

        public static readonly string Identifier = "BloodSample";

        private const int DamageToDeal = 1;

        public BloodSampleCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

    }
}