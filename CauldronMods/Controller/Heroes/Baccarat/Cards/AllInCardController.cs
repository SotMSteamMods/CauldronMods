using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Baccarat
{
    public class AllInCardController : CardController
    {
        public AllInCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        public override IEnumerator Play()
        {
            List<DiscardCardAction> storedResults = new List<DiscardCardAction>();

            //Discard a card from your hand...
            IEnumerator coroutine = base.GameController.SelectAndDiscardCard(this.DecisionMaker, true, null, storedResults, responsibleTurnTaker: base.TurnTaker, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //...If you do...
            if (base.DidDiscardCards(storedResults, new int?(1)))
            {
                Card cardToDiscard = storedResults.First<DiscardCardAction>().CardToDiscard;
                //...{Baccarat} deals each non-hero target...
                coroutine = base.DealMultipleInstancesOfDamage(new List<DealDamageAction>
                { 
                    //...1 infernal damage...
                    new DealDamageAction(base.GetCardSource(), new DamageSource(base.GameController, base.CharacterCard), null, 1, DamageType.Infernal),

                    //...and 1 radiant damage.
                    new DealDamageAction(base.GetCardSource(), new DamageSource(base.GameController, base.CharacterCard), null, 1, DamageType.Radiant)
                }, (Card c) => !IsHeroTarget(c));
            }
            else
            {
                coroutine = base.GameController.SendMessageAction(base.TurnTaker.Name + " did not discard a card, so no damage will be dealt.", Priority.Medium, base.GetCardSource(), null, true);
            }
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }
    }
}