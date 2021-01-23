using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheInfernalChoir
{
    public abstract class TheInfernalChoirUtilityCardController : CardController
    {
        protected TheInfernalChoirUtilityCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        protected Card FindVagrantHeartHiddenSoul()
        {
            var p1Heart = TurnTaker.FindCard("VagrantHeartPhase1", false);
            return (p1Heart?.IsInPlay ?? false) ? p1Heart : null;
        }

        protected bool DoesPlayAreaContainHiddenHeart(TurnTaker tt)
        {
            var card = FindVagrantHeartHiddenSoul();
            if (card is null)
                return false;
            return tt == card.Location.OwnerTurnTaker;
        }

        protected bool IsHiddenHeartInPlay()
        {
            return FindVagrantHeartHiddenSoul() != null;
        }

        protected bool IsSoulRevealedInPlay()
        {
            return FindVagrantHeartSoulRevealed() != null;
        }

        protected Card FindVagrantHeartSoulRevealed()
        {
            var p2Heart = TurnTaker.FindCard("VagrantHeartPhase2", false);
            return (p2Heart?.IsInPlay ?? false) ? p2Heart : null;
        }

        protected bool IsGhost(Card c, bool evenIfUnderCard = false, bool evenIfFaceDown = false)
        {
            return c != null && (c.DoKeywordsContain("ghost", evenIfUnderCard, evenIfFaceDown) || GameController.DoesCardContainKeyword(c, "ghost", evenIfUnderCard, evenIfFaceDown));
        }
    }
}
