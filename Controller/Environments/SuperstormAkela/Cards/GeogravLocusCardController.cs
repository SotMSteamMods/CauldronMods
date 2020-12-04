using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.SuperstormAkela
{
    public class GeogravLocusCardController : SuperstormAkelaCardController
    {

        public GeogravLocusCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowNumberOfCardsAtLocation(base.TurnTaker.PlayArea, new LinqCardCriteria((Card c) => IsLeftOfThisCard(c, base.Card), "card(s) left of this"));
        }

        public override void AddTriggers()
        {
            //Reduce damage dealt to this card by X, where X is the number of environment cards to the left of this one.
            Func<DealDamageAction, int> X = (DealDamageAction dd) => GetNumberOfCardsToTheLeftOfThisOne(base.Card).Value;
            AddReduceDamageTrigger((DealDamageAction dd) => dd.Target == base.Card, X);

            //At the end of the environment turn, play the top card of the environment deck.
            AddEndOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, PlayTheTopCardOfTheEnvironmentDeckWithMessageResponse, TriggerType.PlayCard);
        }


    }
}