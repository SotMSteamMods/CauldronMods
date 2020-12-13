using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;

namespace Cauldron.Echelon
{
    public class KnowYourEnemyCardController : CardController
    {
        //==============================================================
        //==============================================================

        public static string Identifier = "KnowYourEnemy";

        public KnowYourEnemyCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

    }
}