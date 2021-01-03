using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.TheCybersphere
{
    public class N1nj4CardController : TheCybersphereCardController
    {

        public N1nj4CardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowLowestHP(cardCriteria: new LinqCardCriteria((Card c) => c != this.Card && GameController.IsCardVisibleToCardSource(c, GetCardSource()), "target", false));
        }

        public override void AddTriggers()
        {
            //At the end of the environment turn, this card deals the target other than itself with the lowest HP 3 energy damage.
            AddDealDamageAtEndOfTurnTrigger(base.TurnTaker, base.Card, (Card c) => c != base.Card && GameController.IsCardVisibleToCardSource(c, GetCardSource()), TargetType.LowestHP, 3, DamageType.Energy);

            //Whenever damage dealt by this card destroys a target, play the top card of the environment deck.
            AddTrigger<DealDamageAction>((DealDamageAction dd) => dd.DamageSource != null && dd.DamageSource.Card == base.Card && dd.DidDestroyTarget, PlayTheTopCardOfTheEnvironmentDeckWithMessageResponse, TriggerType.PlayCard, TriggerTiming.After);
        }
    }
}