using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.TheMistressOfFate
{
    public class StolenFutureCardController : TheMistressOfFateUtilityCardController
    {
        public StolenFutureCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            _isStoredCard = false;
            SpecialStringMaker.ShowHeroCharacterCardWithHighestHP().Condition = () => !Card.IsInPlayAndHasGameText;
        }

        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            //"Play this card next to the hero with the highest HP.
            var storedHero = new List<Card>();
            IEnumerator coroutine = GameController.FindTargetWithHighestHitPoints(1, (Card c) =>  IsHeroCharacterCard(c), storedHero, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            var hero = storedHero.FirstOrDefault();
            if (hero != null)
            {
                storedResults?.Add(new MoveCardDestination(hero.NextToLocation));
            }
            yield break;
        }

        public override IEnumerator Play()
        {
            IEnumerator coroutine;
            //[The hero this is next to] may deal 1 target 5 radiant damage. 
            var heroTTC = FindHeroTurnTakerController(GetCardThisCardIsNextTo()?.Owner.ToHero());
            if (heroTTC != null)
            {
                coroutine = GameController.SelectTargetsAndDealDamage(heroTTC, new DamageSource(GameController, GetCardThisCardIsNextTo()), 5, DamageType.Radiant, 1, false, 0, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }
            //Then, destroy this card.
            coroutine = GameController.DestroyCard(DecisionMaker, this.Card, cardSource: GetCardSource());
            if (UseUnityCoroutines)
            {
                yield return GameController.StartCoroutine(coroutine);
            }
            else
            {
                GameController.ExhaustCoroutine(coroutine);
            }
            yield break;
        }

        public override void AddTriggers()
        {
            //"When this card is destroyed, destroy all hero characters in its play area."
            AddWhenDestroyedTrigger(DestroyAllHeroesHereResponse, TriggerType.DestroyCard);
        }

        private IEnumerator DestroyAllHeroesHereResponse(DestroyCardAction dc)
        {
            var playArea = this.Card.Location.HighestRecursiveLocation;
            if(playArea.IsInPlayAndNotUnderCard)
            {
                IEnumerator coroutine = GameController.SelectAndDestroyCards(DecisionMaker, new LinqCardCriteria((Card c) =>  IsHeroCharacterCard(c) && c.IsTarget && c.Location.HighestRecursiveLocation == playArea), null, cardSource: GetCardSource());
                if (UseUnityCoroutines)
                {
                    yield return GameController.StartCoroutine(coroutine);
                }
                else
                {
                    GameController.ExhaustCoroutine(coroutine);
                }
            }
            yield break;
        }
    }
}
