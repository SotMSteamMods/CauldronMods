using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;

namespace Cauldron
{
    public class HeavyRadiationCardController : CardController
    {
        public HeavyRadiationCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }
        //Reduce damage dealt to {Gray} by 1 for each Radiation card in play.",
        //At the end of the villain turn, if there are no Radiation cards in play, play the top card of the villain deck.
    }
}