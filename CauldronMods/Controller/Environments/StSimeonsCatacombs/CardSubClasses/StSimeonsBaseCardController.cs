using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Linq;

namespace Cauldron.StSimeonsCatacombs
{
    public abstract class StSimeonsBaseCardController : CardController
    {
        protected StSimeonsBaseCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        protected Card catacombs => TurnTaker.FindCard(StSimeonsCatacombsCardController.Identifier);

        protected bool IsGhost(Card card)
        {
            return card != null && card.DoKeywordsContain("ghost");
        }

        protected bool IsDefinitionRoom(Card card)
        {
            return card != null && card.Definition.Keywords.Contains("room");
        }
    }
}