using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.WindmillCity
{
    public class RainOfDebrisCardController : WindmillCityUtilityCardController
    {
        public RainOfDebrisCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            SpecialStringMaker.ShowLowestHP(cardCriteria: new LinqCardCriteria(c => IsResponder(c), "responder"));
        }

        public override IEnumerator Play()
        {
            //When this card enters play, it deals the Responder wth the lowest HP 2 melee damage. 
            IEnumerator coroutine = DealDamageToLowestHP(Card, 1, (Card c) => IsResponder(c), (Card c) => 2, DamageType.Melee);
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
            //At the end of the environment turn, each hero may discard a card. This card deals any hero that did not discard a card this way 2 melee damage.
            AddEndOfTurnTrigger((TurnTaker tt) => tt == TurnTaker, EndOfTurnResponse, new TriggerType[]
            {
                TriggerType.DiscardCard,
                TriggerType.DealDamage
            });
        }

        private IEnumerator EndOfTurnResponse(PhaseChangeAction pca)
        {
            //At the end of the environment turn, each hero may discard a card.
            List<DiscardCardAction> storedResultsDiscard = new List<DiscardCardAction>();
            IEnumerator coroutine = GameController.EachPlayerDiscardsCards(0, 1, storedResultsDiscard, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            //This card deals any hero that did not discard a card this way 2 melee damage.
            List<HeroTurnTakerController> discardHeroes = new List<HeroTurnTakerController>();
            if(DidDiscardCards(storedResultsDiscard))
            {
                foreach(DiscardCardAction dca in storedResultsDiscard)
                {
                    discardHeroes.Add(dca.HeroTurnTakerController);
                }
            }
            coroutine = DealDamage(Card, (Card card) => card.IsHeroCharacterCard && !discardHeroes.Select((HeroTurnTakerController t) => t.TurnTaker).Contains(card.Owner), 2, DamageType.Melee);
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
        }
    }
}
