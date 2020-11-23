using System;
using System.Collections;
using System.Collections.Generic;
using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.StSimeonsCatacombs
{
    public class PanicCardController : CardController
    {
        #region Constructors

        public PanicCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {

        }

        #endregion Constructors

        #region Methods

        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            //Play this card next to the hero with the highest HP
            IEnumerator coroutine = base.SelectCardThisCardWillMoveNextTo(new LinqCardCriteria((Card c) => base.CanCardBeConsideredHighestHitPoints(c, (Card card) => card.IsHeroCharacterCard) && (additionalTurnTakerCriteria == null || additionalTurnTakerCriteria.Criteria(c.Owner)), "hero target with the lowest HP"), storedResults, isPutIntoPlay, decisionSources);
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

        public override void AddTriggers()
        {
            //At the start of that hero's next turn, that hero uses their innate power twice, then immediately end their turn, draw a card, and destroy this card.
            base.AddStartOfTurnTrigger((TurnTaker tt) => tt == base.GetCardThisCardIsNextTo().Owner, new Func<PhaseChangeAction, IEnumerator>(this.StartOfHeroResponse), new TriggerType[]
            {
                TriggerType.UsePower,
                TriggerType.SkipTurn,
                TriggerType.DrawCard,
                TriggerType.DestroySelf
            });
        }

        private IEnumerator StartOfHeroResponse(PhaseChangeAction pca)
        {
            HeroTurnTakerController httc = base.FindHeroTurnTakerController(pca.ToPhase.TurnTaker.ToHero());
            //that hero uses their innate power twice, 
            IEnumerator power1 = base.UsePowerOnOtherCard(base.GetCardThisCardIsNextTo());
            IEnumerator power2 = base.UsePowerOnOtherCard(base.GetCardThisCardIsNextTo());
            //then immediately end their turn, 
            IEnumerator endTurn = base.GameController.ImmediatelyEndTurn(httc, base.GetCardSource());
            //draw a card
            IEnumerator draw = base.DrawCard(httc.HeroTurnTaker);
            //destroy this card.
            IEnumerator destroy = base.DestroyThisCardResponse(pca);

            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(power1);
                yield return base.GameController.StartCoroutine(power2);
                yield return base.GameController.StartCoroutine(endTurn);
                yield return base.GameController.StartCoroutine(draw);
                yield return base.GameController.StartCoroutine(destroy);
            }
            else
            {
                base.GameController.ExhaustCoroutine(power1);
                base.GameController.ExhaustCoroutine(power2);
                base.GameController.ExhaustCoroutine(endTurn);
                base.GameController.ExhaustCoroutine(draw);
                base.GameController.ExhaustCoroutine(destroy);
            }
            yield break;
        }

        #endregion Methods
    }
}