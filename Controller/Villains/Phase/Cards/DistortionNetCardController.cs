using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;

namespace Cauldron.Phase
{
    public class DistortionNetCardController : CardController
    {
        public DistortionNetCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        //When this card enters play, place it next to the hero with the highest HP.
        //Reduce damage dealt by that hero by 2.
        //At the start of that hero's turn, this card deals them {H} toxic damage.
    }
}