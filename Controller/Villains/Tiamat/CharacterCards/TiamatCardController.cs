using System;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron
{
    class TiamatCardController : CardController
    {
        public TiamatCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public Card InfernoTiamatCharacterCard
        {
            get
            {
                return base.TurnTaker.GetCardByIdentifier("InfernoTiamatCharacterCard");
            }
        }

        public Card StormTiamatCharacterCard
        {
            get
            {
                return base.TurnTaker.GetCardByIdentifier("StormTiamatCharacterCard");
            }
        }

        public Card WinterTiamatCharacterCard
        {
            get
            {
                return base.TurnTaker.GetCardByIdentifier("WinterTiamatCharacterCard");
            }
        }
    }
}
