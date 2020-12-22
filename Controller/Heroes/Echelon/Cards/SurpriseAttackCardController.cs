using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;

namespace Cauldron.Echelon
{
    public class SurpriseAttackCardController : TacticBaseCardController
    {
        //==============================================================
        //"At the start of your turn, you may discard a card. If you do not, draw a card and destroy this card.",
        //"Whenever a hero uses a power that deals damage, increase that damage by 1. You may change the type of that damage to psychic."
        //==============================================================

        public static string Identifier = "SurpriseAttack";

        public SurpriseAttackCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        protected override void AddTacticEffectTrigger()
        {
            //"Whenever a hero uses a power that deals damage, increase that damage by 1. You may change the type of that damage to psychic."
        }
    }
}