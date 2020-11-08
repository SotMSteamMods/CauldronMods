using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Linq;
using System;

namespace Cauldron.Necro
{
    public abstract class UndeadCardController : NecroCardController
    {
        protected UndeadCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        protected void SetMaximumHPWithRituals(int baseHP)
        {
            this.Card.SetMaximumHP(GetNumberOfRitualsInPlay() + baseHP, true);
        }
    }
}
