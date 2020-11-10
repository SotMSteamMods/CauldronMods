using System;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TangoOne
{
    public class DamnGoodGroundCardController : TangoOneBaseCardController
    {
        public static string Identifier = "DamnGoodGround";

        public DamnGoodGroundCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

    }
}