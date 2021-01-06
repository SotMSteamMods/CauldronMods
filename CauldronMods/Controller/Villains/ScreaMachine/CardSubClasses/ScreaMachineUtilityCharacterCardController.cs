using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.ScreaMachine
{
    public abstract class ScreaMachineUtilityCharacterCardController : VillainCharacterCardController
    {
        protected ScreaMachineUtilityCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        private HashSet<string> _bandKeywords = new HashSet<string>(StringComparer.Ordinal)
        {
            "vocalist",
            "guitarist",
            "bassist",
            "drummer"
        };

        protected bool IsBandCard(Card c, bool evenIfUnderCard = false, bool evenIfFacedown = false)
        {
            return GameController.GetAllKeywords(c, evenIfUnderCard, evenIfFacedown).Any(str => _bandKeywords.Contains(str));
        }

        protected bool IsVocalist(Card c, bool evenIfUnderCard = false, bool evenIfFacedown = false)
        {
            return GameController.DoesCardContainKeyword(c, "vocalist", evenIfUnderCard, evenIfFacedown);
        }

        protected bool IsGuitarist(Card c, bool evenIfUnderCard = false, bool evenIfFacedown = false)
        {
            return GameController.DoesCardContainKeyword(c, "guitarist", evenIfUnderCard, evenIfFacedown);
        }

        protected bool IsBassist(Card c, bool evenIfUnderCard = false, bool evenIfFacedown = false)
        {
            return GameController.DoesCardContainKeyword(c, "bassist", evenIfUnderCard, evenIfFacedown);
        }

        protected bool IsDrummer(Card c, bool evenIfUnderCard = false, bool evenIfFacedown = false)
        {
            return GameController.DoesCardContainKeyword(c, "drummer", evenIfUnderCard, evenIfFacedown);
        }
    }
}
