using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.Echelon
{
    public class RuthlessIntimidationCardController : TacticBaseCardController
    {
        //==============================================================
        // At the start of your turn, you may discard a card. If you do not, draw a card and destroy this card.
        // Increase damage dealt to the non-hero target with the lowest HP by 1.
        //==============================================================

        public static string Identifier = "RuthlessIntimidation";

        public RuthlessIntimidationCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        protected override void AddTacticEffectTrigger()
        {
            //Increase damage dealt to the non-hero target with the lowest HP by 1.
        }
    }
}