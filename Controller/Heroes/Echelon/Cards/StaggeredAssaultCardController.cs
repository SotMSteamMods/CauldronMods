using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;

namespace Cauldron.Echelon
{
    public class StaggeredAssaultCardController : TacticBaseCardController
    {
        //==============================================================
        // "At the start of your turn, you may discard a card. If you do not, draw a card and destroy this card.",
        // "Once per turn when a hero target deals an instance of 2 or more damage, {Echelon} may deal 1 target 1 melee damage."
        //==============================================================

        public static string Identifier = "StaggeredAssault";

        public StaggeredAssaultCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        protected override void AddTacticEffectTrigger()
        {
            //"Once per turn when a hero target deals an instance of 2 or more damage, {Echelon} may deal 1 target 1 melee damage."
        }
    }
}