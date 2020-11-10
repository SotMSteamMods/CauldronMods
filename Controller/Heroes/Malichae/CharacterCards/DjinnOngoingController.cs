using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Linq;
using System;

namespace Cauldron.Malichae
{
    public abstract class DjinnOngoingController : MalichaeCardController
    {
        protected DjinnOngoingController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
    }
}
