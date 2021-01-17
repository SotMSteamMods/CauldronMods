using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;

namespace Cauldron.Tiamat
{
    public class NeoscaleCharacterCardController : DragonscaleCharacterCardController
    {
        public NeoscaleCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
    }
}