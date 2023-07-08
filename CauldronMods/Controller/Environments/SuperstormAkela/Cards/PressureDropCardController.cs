using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cauldron.SuperstormAkela
{
    public class PressureDropCardController : SuperstormAkelaCardController
    {

        public PressureDropCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowHeroCharacterCardWithLowestHP();
            base.SpecialStringMaker.ShowSpecialString(() => BuildCardsLeftOfThisSpecialString()).Condition = () => Card.IsInPlayAndHasGameText;
            base.SpecialStringMaker.ShowSpecialString(() => BuildCardsRightOfThisSpecialString()).Condition = () => Card.IsInPlayAndHasGameText;

        }

        public override IEnumerator Play()
        {
            //When this card enters play, the hero with the lowest HP must discard X cards, where X is the number of environment cards to the left of this one.

            List<Card> storedResults = new List<Card>();
            IEnumerator coroutine = base.GameController.FindTargetWithLowestHitPoints(1, (Card c) =>  IsHeroCharacterCard(c), storedResults, cardSource: GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            if (DidFindCard(storedResults))
            {
                Card heroCard = storedResults.First();
                HeroTurnTakerController hero = FindHeroTurnTakerController(heroCard.Owner.ToHero());
                int X = (GetNumberOfCardsToTheLeftOfThisOne(base.Card) ?? 0);
                coroutine = base.GameController.SelectAndDiscardCards(hero, X, false, X, cardSource: GetCardSource());
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

        public override void AddTriggers()
        {
            //At the start of the environment turn, if there are 3 environment cards to the right of this one, destroy this card.
            AddStartOfTurnTrigger((TurnTaker tt) => tt == base.TurnTaker, DestroyIf3ToTheRight, TriggerType.DestroySelf);
        }

        private IEnumerator DestroyIf3ToTheRight(PhaseChangeAction pca)
        {
            int numCardsToTheRight = GetNumberOfCardsToTheRightOfThisOne(base.Card).Value;
            if(numCardsToTheRight == 3)
            {
                IEnumerator coroutine = DestroyThisCardResponse(pca);
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