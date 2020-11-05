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
        #region Constructors

        public AllInCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController) { }

        #endregion Constructors

        #region Methods

        public override IEnumerator Play()
        {
            List<DiscardCardAction> storedResults = new List<DiscardCardAction>();

            //Discard a card from your hand...
            IEnumerator coroutine = base.GameController.SelectAndDiscardCard(this.DecisionMaker, true, null, storedResults, SelectionType.DiscardCard, null, base.TurnTaker, false, null, base.GetCardSource(null));
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            //...If you do...
            if (base.DidDiscardCards(storedResults, new int?(1), false))
            {
                Card cardToDiscard = storedResults.First<DiscardCardAction>().CardToDiscard;
                //...{Baccarat} deals each non-hero target...
                coroutine = base.DealMultipleInstancesOfDamage(new List<DealDamageAction>
                { 
                    //...1 infernal damage...
                    new DealDamageAction(base.GetCardSource(null), new DamageSource(base.GameController, base.CharacterCard), null, 1, DamageType.Infernal, false, null, null, null, false),

                    //...and 1 radiant damage.
                    new DealDamageAction(base.GetCardSource(null), new DamageSource(base.GameController, base.CharacterCard), null, 1, DamageType.Radiant, false, null, null, null, false)
                }, (Card c) => !c.IsHero, null, null, null, null, false);
            }
            else
            {
                coroutine = base.GameController.SendMessageAction(base.TurnTaker.Name + " did not discard a card, so no damage will be dealt.", Priority.Medium, base.GetCardSource(null), null, true);
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

        #endregion Methods
    }
}