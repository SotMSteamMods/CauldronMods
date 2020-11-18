using System;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;


namespace Cauldron.Dendron
{
    public class ChokingInscriptionCardController : CardController
    {
        public static string Identifier = "ChokingInscription";

        public ChokingInscriptionCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

    }
}