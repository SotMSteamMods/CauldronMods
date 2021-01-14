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
            string[] futureElements = new string[] { "ElementOfIce", "ElementOfFire", "ElementOfLightning" };
            if (base.CharacterCardController is FutureTiamatCharacterCardController && futureElements.Contains(base.Card.Identifier))
            {
                //Each spell card in the villain trash counts as Element of Ice, Element of Fire, and Element of Lightining.
                return value + (from card in base.TurnTaker.Trash.Cards
                                where this.IsSpell(card)
                                select card).Count();
            };
            return value + (from card in base.TurnTaker.Trash.Cards
                            where card.Identifier == this.Card.Identifier
                            select card).Count();
        }

        public bool IsSpell(Card c)
        {
            return c.DoKeywordsContain("spell");
        }
    }
}