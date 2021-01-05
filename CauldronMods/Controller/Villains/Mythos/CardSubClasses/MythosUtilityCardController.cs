using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Mythos
{
    public abstract class MythosUtilityCardController : CardController
    {
        protected MythosUtilityCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        protected string MythosClueDeckIdentifier = "MythosClueDeck";
        protected string MythosDangerDeckIdentifier = "MythosDangerDeck";
        protected string MythosMadnessDeckIdentifier = "MythosMadnessDeck";

        public bool IsTopCardMatching(string type)
        {
            return base.TurnTaker.Deck.TopCard.ParentDeck.Identifier == type;
        }

        public override MoveCardDestination GetTrashDestination()
        {
            return new MoveCardDestination(base.TurnTaker.Trash);
        }
    }
}
