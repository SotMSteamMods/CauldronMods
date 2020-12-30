using System;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Tiamat
{
    public abstract class SpellCardController : CardController
    {
        protected SpellCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        protected int PlusNumberOfThisCardInTrash(int value)
        {
            return value + (from card in base.TurnTaker.Trash.Cards
                            where card.Identifier == this.Card.Identifier
                            select card).Count<Card>();
        }
    }
}