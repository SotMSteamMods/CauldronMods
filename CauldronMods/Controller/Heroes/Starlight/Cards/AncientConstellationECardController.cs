using System;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Starlight
{
    public class AncientConstellationECardController : AncientConstellationGenericCardController
    {
        public AncientConstellationECardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        //everything is inherited from the Generic, this is just for art purposes
    }
}