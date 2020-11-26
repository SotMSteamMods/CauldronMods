using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;

namespace Cauldron.SwarmEater
{
    public abstract class AugCardController : CardController
    {
        public AugCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public abstract ITrigger[] AddRegularTriggers();

        public abstract ITrigger[] AddAbsorbTriggers(Card cardThisIsUnder);

        public override void AddTriggers()
        {
            if (base.GetCardThisCardIsBelow() == null)
            {
                this.AddRegularTriggers();
            }
            else
            {
                Card nextTo = base.GetCardThisCardIsBelow();
                if (nextTo.Identifier == "AbsorbedNanites")
                {
                    nextTo = base.CharacterCard;
                }
                this.AddAbsorbTriggers(nextTo);
                return;
            }
        }
    }
}