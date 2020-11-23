using System;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Starlight
{
    public class AncientConstellationFCardController : AncientConstellationGenericCardController
    {
        public AncientConstellationFCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        //everything is inherited from the Generic, this is just for art purposes
    }
}