using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Linq;
using Handelabra;
using System.Collections.Generic;
using System.Reflection;
using static System.Net.Mime.MediaTypeNames;

namespace Cauldron.Quicksilver
{
    public class QuicksilverTurnTakerController : HeroTurnTakerController
    {
        public QuicksilverTurnTakerController(TurnTaker turnTaker, GameController gameController) : base(turnTaker, gameController)
        {

        }

        public string[] availablePromos = new string[] { "UncannyQuicksilver", };

        public bool ArePromosSetup { get; set; } = false;

    }
}