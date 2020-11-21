using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Vanish
{
    public class TacticalRelocationCardController : CardController
    {
        public TacticalRelocationCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowNumberOfCardsAtLocations(
                    () => GameController.HeroTurnTakerControllers.Select(httc => httc.HeroTurnTaker.Trash),
                    new LinqCardCriteria(c => c.IsOngoing && IsEquipment(c) && c.IsInTrash, "equipment or ongoings in trash")
                );
        }
    }
}