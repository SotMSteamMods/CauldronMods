using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheStranger
{
    public abstract class RuneCardController : TheStrangerBaseCardController
    {
        protected RuneCardController(Card card, TurnTakerController turnTakerController, LinqCardCriteria nextToCardCriteria) : base(card, turnTakerController)
        {
            this.NextToCardCriteria = nextToCardCriteria;
        }

        public override void AddTriggers()
        {
            //At the start of your turn you may destroy this card. If you do not, The Stranger deals himself 1 irreducible toxic damage.
            base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, this.DestroyCardResponse, TriggerType.DestroySelf, null, false);
           
            //if the card this is next to leaves, have this card fall off
            Card cardThisCardIsNextTo = base.GetCardThisCardIsNextTo(true);
            base.AddIfTheCardThatThisCardIsNextToLeavesPlayMoveItToTheirPlayAreaTrigger(false, cardThisCardIsNextTo != null && !cardThisCardIsNextTo.IsHeroCharacterCard);
        }

        private IEnumerator DestroyCardResponse(PhaseChangeAction action)
        {
            //You may destroy this card
            List<DestroyCardAction> storedResult = new List<DestroyCardAction>();
            IEnumerator coroutine = this.GameController.DestroyCard(this.DecisionMaker, this.Card, true, storedResult, null, null,null, null, null, null, null, this.GetCardSource(null));
            if (this.UseUnityCoroutines)
            {
                yield return this.GameController.StartCoroutine(coroutine);
            }
            else
            {
                this.GameController.ExhaustCoroutine(coroutine);
            }
            if(storedResult == null || storedResult.Count == 0 || !storedResult.First().WasCardDestroyed )
            {
                //if you did not...
               
                    //The Stranger deals himself 1 irreducible toxic damage
                    IEnumerator coroutine2 = base.DealDamage(base.CharacterCard, base.CharacterCard, 1, DamageType.Toxic, true, false, false, null, null, null, false, null);
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine2);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine2);
                    }
            }
            yield break;
        }

        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            IEnumerator coroutine = base.SelectCardThisCardWillMoveNextTo(new LinqCardCriteria((Card c) => this.NextToCardCriteria.Criteria(c) && (additionalTurnTakerCriteria == null || additionalTurnTakerCriteria.Criteria(c.Owner)), this.NextToCardCriteria.Description, true, false, null, null, false), storedResults, isPutIntoPlay, decisionSources);
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

        public LinqCardCriteria NextToCardCriteria { get; }
    }
}