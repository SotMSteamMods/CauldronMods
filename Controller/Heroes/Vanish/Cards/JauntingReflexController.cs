using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Vanish
{
    public class JauntingReflexCardController : CardController
    {
        public JauntingReflexCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }
        /*
        public override void AddTriggers()
        {
            AddTrigger<DealDamageAction>(dda => !dda.DamageSource.IsHero && dda.Target.IsHero && dda.DidDealDamage, null, TriggerType.UsePower, TriggerTiming.After, isActionOptional: true);
        }

        private bool WasNotUsedThisTurn()
        {
            return !base.Journal.CardPropertiesEntriesThisTurn(Card).Any(j => j.Key == "UsedJauntingReflex");
        }

        private IEnumerator HeroDamagedResponse(DealDamageAction action)
        {
            List<DiscardCardAction> results = new List<DiscardCardAction>();
            var coroutine = GameController.SelectAndDiscardCard(DecisionMaker, optional: true, storedResults: results, cardSource: GetCardSource());
            
                        

            //

            if (DidDiscardCards(results, 1))
            {
                base.SetCardPropertyToTrueIfRealAction("UsedJauntingReflex");

                coroutine = GameController.SelectAndUsePower(DecisionMaker, optional: true, cardSource: GetCardSource());
            }
        }
        */
    }
}