using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Terminus
{
    public class TerminusBaseCharacterCardController : HeroCharacterCardController
    {
        public TerminusBaseCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowTokenPool(base.Card.FindTokenPool("TerminusWrathPool"));
            //base.AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
        }

        //public override bool AskIfCardIsIndestructible(Card card)
        //{
        //    bool isIndestructible = false;
        //    Card stainedBadge = base.FindCard("StainedBadge");

        //    if (stainedBadge.IsInPlayAndHasGameText && 
        //        card == base.CharacterCard && 
        //        base.GameController.AllTurnTakers.Count((tt) => 
        //            tt != base.TurnTaker && 
        //            tt.IsHero && 
        //            !tt.IsIncapacitatedOrOutOfGame && 
        //            AskIfTurnTakerIsVisibleToCardSource(tt, base.GetCardSource()) == true) > 0)
        //    {
        //        isIndestructible = true;
        //    }

        //    return isIndestructible;
        //}

        //public override bool ShouldBeDestroyedNow()
        //{
        //    return base.ShouldBeDestroyedNow();
        //}
    }
}
