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

        protected string VagrantHeartHiddenHeartIdentifier => "VagrantHeartPhase1";
        protected string VagrantHeartSoulRevealedIdentifier => "VagrantHeartPhase2";

        protected Card FindVagrantHeartHiddenHeart()
        {
            var p1Heart = TurnTaker.FindCard(VagrantHeartHiddenHeartIdentifier, false);
            return (p1Heart?.IsInPlay ?? false) ? p1Heart : null;
        }

        protected bool DoesPlayAreaContainHiddenHeart(TurnTaker tt)
        {
            var card = FindVagrantHeartHiddenHeart();
            if (card is null)
                return false;
            return tt == card.Location.OwnerTurnTaker;
        }

        protected bool IsVagrantHeartHiddenHeartInPlay()
        {
            return FindVagrantHeartHiddenHeart() != null;
        }

        protected bool IsVagrantHeartSoulRevealedInPlay()
        {
            return FindVagrantHeartSoulRevealed() != null;
        }

        protected Card FindVagrantHeartSoulRevealed()
        {
            var p2Heart = TurnTaker.FindCard(VagrantHeartSoulRevealedIdentifier, false);
            return (p2Heart?.IsInPlay ?? false) ? p2Heart : null;
        }

        protected void DebugHeartStatus()
        {
            var p1Heart = TurnTaker.FindCard(VagrantHeartHiddenHeartIdentifier, false);
            var p2Heart = TurnTaker.FindCard(VagrantHeartSoulRevealedIdentifier, false);

            Console.WriteLine($"STATUS - Heart1 {p1Heart.Location.GetFriendlyName()}");
            Console.WriteLine($"STATUS - Heart2 {p2Heart.Location.GetFriendlyName()}");

        }

        protected bool IsGhost(Card c, bool evenIfUnderCard = false, bool evenIfFaceDown = false)
        {
            return c != null && (c.DoKeywordsContain("ghost", evenIfUnderCard, evenIfFaceDown) || GameController.DoesCardContainKeyword(c, "ghost", evenIfUnderCard, evenIfFaceDown));
        }
    }
}
