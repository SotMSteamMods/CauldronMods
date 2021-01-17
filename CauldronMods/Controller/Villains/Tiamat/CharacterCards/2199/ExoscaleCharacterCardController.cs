using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;

namespace Cauldron.Tiamat
{
    public class ExoscaleCharacterCardController : DragonscaleCharacterCardController
    {
        public ExoscaleCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
    }
}