using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Mythos
{
    public class TornPageCardController : MythosUtilityCardController
    {
        public TornPageCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        private const string FirstTimeActivated = "FirstTimeActivated";

        public override void AddTriggers()
        {
            //The first time each turn that:
            //{MythosDanger} a hero card is drawn...
            base.AddTrigger<DrawCardAction>((DrawCardAction action) => !base.HasBeenSetToTrueThisTurn(FirstTimeActivated) && base.IsTopCardMatching(MythosDangerDeckIdentifier) && action.DidDrawCard, this.DealDamageResponse, new TriggerType[] { TriggerType.DealDamage }, TriggerTiming.After);
            //{MythosMadness} a hero card enters play...
            base.AddTrigger<CardEntersPlayAction>((CardEntersPlayAction action) => !base.HasBeenSetToTrueThisTurn(FirstTimeActivated) && action.CardEnteringPlay.IsHero && action.IsSuccessful, this.DealDamageResponse, new TriggerType[] { TriggerType.DealDamage }, TriggerTiming.After);
            //{MythosClue} a power is used...
            base.AddTrigger<UsePowerAction>((UsePowerAction action) => !base.HasBeenSetToTrueThisTurn(FirstTimeActivated) && action.IsSuccessful, this.DealDamageResponse, new TriggerType[] { TriggerType.DealDamage }, TriggerTiming.After);
            //...this card deals that hero 2 infernal damage and 2 psychic damage.
        }

        private IEnumerator DealDamageResponse(GameAction action)
        {
            //...this card deals that hero 2 infernal damage and 2 psychic damage.
            yield break;
        }
    }
}
