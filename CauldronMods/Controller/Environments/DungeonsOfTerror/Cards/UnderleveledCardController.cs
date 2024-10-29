using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.DungeonsOfTerror
{
    public class UnderleveledCardController : DungeonsOfTerrorUtilityCardController
    {
        public UnderleveledCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowSpecialString(() => BuildTopCardOfLocationSpecialString(TurnTaker.Trash));
        }

        public override void AddTriggers()
        {
            //While the top card of the environment trash is a fate card, increase damage dealt by villain targets by 1. 
            AddIncreaseDamageTrigger((DealDamageAction dd) => IsTopCardOfLocationFate(TurnTaker.Trash) == true && dd.DamageSource.IsVillainTarget, (DealDamageAction dd) => 1);

            //While it is not a fate card, increase damage dealt by hero targets by 1.
            AddIncreaseDamageTrigger((DealDamageAction dd) => IsTopCardOfLocationFate(TurnTaker.Trash) == false && dd.DamageSource.IsHeroTarget, (DealDamageAction dd) => 1);

            //At the start of the environment turn, destroy this card.
            AddStartOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, DestroyThisCardResponse, TriggerType.DestroySelf);
        }
    }
}
