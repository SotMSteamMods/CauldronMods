using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Mythos
{
    public class RustedArtifactCardController : MythosUtilityCardController
    {
        public RustedArtifactCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.AddThisCardControllerToList(CardControllerListType.MakesIndestructible);
        }

        public override void AddTriggers()
        {
            //Increase damage dealt to hero targets by 1.
            base.AddIncreaseDamageTrigger((DealDamageAction action) => IsHeroTarget(action.Target), 1);
            //{MythosMadness}{MythosDanger} This card is indestructible and immune to damage.
            base.AddImmuneToDamageTrigger((DealDamageAction action) => (base.IsTopCardMatching(MythosMadnessDeckIdentifier) || base.IsTopCardMatching(MythosDangerDeckIdentifier)) && action.Target == this.Card);
        }

        public override bool AskIfCardIsIndestructible(Card card)
        {
            //{MythosMadness}{MythosDanger} This card is indestructible...
            return (base.IsTopCardMatching(MythosMadnessDeckIdentifier) || base.IsTopCardMatching(MythosDangerDeckIdentifier)) && card == this.Card;
        }
    }
}
