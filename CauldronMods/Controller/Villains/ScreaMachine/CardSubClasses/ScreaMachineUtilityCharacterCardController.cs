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

        protected bool IsBandCard(Card c, bool evenIfUnderCard = false, bool evenIfFacedown = false)
        {
            return GameController.GetAllKeywords(c, evenIfUnderCard, evenIfFacedown).Any(str => ScreaMachineBandmate.Keywords.Contains(str));
        }

        protected bool IsVocalist(Card c, bool evenIfUnderCard = false, bool evenIfFacedown = false)
        {
            return GameController.DoesCardContainKeyword(c, ScreaMachineBandmate.Value.Valentine.GetKeyword(), evenIfUnderCard, evenIfFacedown);
        }

        protected bool IsGuitarist(Card c, bool evenIfUnderCard = false, bool evenIfFacedown = false)
        {
            return GameController.DoesCardContainKeyword(c, ScreaMachineBandmate.Value.Slice.GetKeyword(), evenIfUnderCard, evenIfFacedown);
        }

        protected bool IsBassist(Card c, bool evenIfUnderCard = false, bool evenIfFacedown = false)
        {
            return GameController.DoesCardContainKeyword(c, ScreaMachineBandmate.Value.Bloodlace.GetKeyword(), evenIfUnderCard, evenIfFacedown);
        }

        protected bool IsDrummer(Card c, bool evenIfUnderCard = false, bool evenIfFacedown = false)
        {
            return GameController.DoesCardContainKeyword(c, ScreaMachineBandmate.Value.RickyG.GetKeyword(), evenIfUnderCard, evenIfFacedown);
        }
    }
}
