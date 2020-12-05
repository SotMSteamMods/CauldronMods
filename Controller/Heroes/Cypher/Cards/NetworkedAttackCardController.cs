using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;

namespace Cauldron.Cypher
{
    public class NetworkedAttackCardController : CardController
    {
        //==============================================================
        // Each augmented hero may use a power now.
        //==============================================================

        public static string Identifier = "NetworkedAttack";

        public NetworkedAttackCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

    }
}