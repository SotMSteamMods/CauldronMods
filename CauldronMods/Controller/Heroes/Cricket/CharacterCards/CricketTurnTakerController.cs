using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Linq;
using Handelabra;
using System.Collections.Generic;
using System.Reflection;
using static System.Net.Mime.MediaTypeNames;

namespace Cauldron.Cricket
{
    public class CricketTurnTakerController : HeroTurnTakerController
    {
        public CricketTurnTakerController(TurnTaker turnTaker, GameController gameController) : base(turnTaker, gameController)
        {

        }

        public string[] availablePromos = new string[] { "FirstResponseCricket", "RenegadeCricket", "WastelandRoninCricket" };

        public bool ArePromosSetup { get; set; } = false;

    }
}