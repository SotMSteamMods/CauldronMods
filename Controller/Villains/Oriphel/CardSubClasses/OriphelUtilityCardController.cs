using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.Oriphel
{
    public class OriphelUtilityCardController : CardController
    {
        public OriphelUtilityCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        protected bool IsGuardian(Card c)
        {
            return GameController.DoesCardContainKeyword(c, "guardian");
        }

        protected bool IsGoon(Card c)
        {
            return GameController.DoesCardContainKeyword(c, "goon");
        }

        protected bool IsTransformation(Card c)
        {
            return GameController.DoesCardContainKeyword(c, "transformation");
        }

        protected Card oriphelIfInPlay
        {
            get
            {
                return CharacterCard.Title == "Oriphel" ? CharacterCard : null;
            }
        }
        protected Card jadeIfInPlay
        {
            get
            {
                return CharacterCard.Title == "Jade" ? CharacterCard : null;
            }
        }

        protected GameAction FakeAction
        {
            get
            {
                return new PhaseChangeAction(GetCardSource(), Game.ActiveTurnPhase, Game.ActiveTurnPhase, true);
            }
        }
    }
}