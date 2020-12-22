using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.Echelon
{
    public class KnowYourEnemyCardController : TacticBaseCardController
    {
        //==============================================================
        // At the start of your turn, you may discard a card.
        // If you do not, draw a card and destroy this card.
        // The first time a hero destroys a non-hero target each turn, you may draw a card.
        //==============================================================

        public static string Identifier = "KnowYourEnemy";

        public KnowYourEnemyCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        protected override void AddTacticEffectTrigger()
        {
            //The first time a hero destroys a non-hero target each turn, you may draw a card.
        }

    }
}