using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.TheKnight
{
    public class TheKnightUtilityCharacterCardController : HeroCharacterCardController
    {
        protected bool IsCoreCharacterCard = true;
        protected readonly string RoninKey = "WastelandRoninKnightOwnershipKey";
        public TheKnightUtilityCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddStartOfGameTriggers()
        {
            if (IsCoreCharacterCard)
            {
                (TurnTakerController as TheKnightTurnTakerController).ManageCharactersOffToTheSide(true);
            }
        }

        protected Card GetKnightCardUser(Card c)
        {
            if (c == null)
            {
                return null;
            }

            if (this.TurnTakerControllerWithoutReplacements.HasMultipleCharacterCards)
            {
                if (c.Location.IsNextToCard)
                {
                    return c.Location.OwnerCard;
                }

                if (c.Owner == this.TurnTaker)
                {
                    var propCard = GameController.GetCardPropertyJournalEntryCard(c, RoninKey);
                    return propCard ?? this.CharacterCard;
                }
            }

            return this.CharacterCard;
        }
    }
}