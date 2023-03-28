using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Cauldron.Northspar
{
    public class AlienFungusCardController : NorthsparCardController
    {

        public AlienFungusCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override void AddTriggers()
        {
            //Whenever a Frozen card enters play, this card and Tak Ahab each regain 3HP
            Func<CardEntersPlayAction, bool> criteria = (CardEntersPlayAction cp) => cp.CardEnteringPlay != null && base.IsFrozen(cp.CardEnteringPlay) && cp.IsSuccessful;
            base.AddTrigger<CardEntersPlayAction>(criteria, this.FrozenEntersPlayResponse, TriggerType.GainHP, TriggerTiming.After);

            //At the end of the environment turn this card deals each hero target 2 toxic damage
            base.AddDealDamageAtEndOfTurnTrigger(base.TurnTaker, base.Card, (Card c) => IsHeroTarget(c), TargetType.All, 2, DamageType.Toxic);
        }

        private IEnumerator FrozenEntersPlayResponse(CardEntersPlayAction cp)
        {
            //this card and Tak Ahab each regain 3HP
            List<Card> cardsToGainHP = new List<Card> { base.Card };
            if(base.IsTakAhabInPlay())
            {
                Card takAhab = base.FindTakAhabInPlay();
                cardsToGainHP.Add(takAhab);
            }

            foreach(Card hpGainer in cardsToGainHP)
            {
                IEnumerator coroutine = base.GameController.GainHP(hpGainer, new int?(3), cardSource: base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
            }

            yield break;
           
        }
    }
}