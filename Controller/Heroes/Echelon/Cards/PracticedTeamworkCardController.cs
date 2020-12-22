using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;

namespace Cauldron.Echelon
{
    public class PracticedTeamworkCardController : TacticBaseCardController
    {
        //==============================================================
        //"When this card enters play, each player may draw a card.",
        //"At the start of your turn, you may discard a card. If you do not, destroy this card.",
        //"Reduce damage dealt to hero targets by hero targets by 1."
        //==============================================================

        public static string Identifier = "PracticedTeamwork";

        public PracticedTeamworkCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            this.DrawWhenDropping = false;
        }

        protected override void AddTacticEffectTrigger()
        {
            //"Reduce damage dealt to hero targets by hero targets by 1."
        }
    }
}