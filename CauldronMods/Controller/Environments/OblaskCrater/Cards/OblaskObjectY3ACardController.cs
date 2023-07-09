using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.OblaskCrater
{
    public class OblaskObjectY3ACardController : OblaskCraterUtilityCardController
    {
        /* 
         * Play this card next to a hero, then play the top card of the environment deck. 
         * When the hero next to this card would be dealt damage by an environment target, 
         * they may discard a card. If they do, redirect that damage.
         */
        public OblaskObjectY3ACardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.AllowFastCoroutinesDuringPretend = true;
        }

        public override IEnumerator DeterminePlayLocation(List<MoveCardDestination> storedResults, bool isPutIntoPlay, List<IDecision> decisionSources, Location overridePlayArea = null, LinqTurnTakerCriteria additionalTurnTakerCriteria = null)
        {
            //Play this card next to a hero.
            IEnumerator coroutine = base.SelectCardThisCardWillMoveNextTo(new LinqCardCriteria((Card c) =>  IsHeroCharacterCard(c) && !c.IsIncapacitatedOrOutOfGame && GameController.IsCardVisibleToCardSource(c, GetCardSource())), storedResults, true, decisionSources);
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
            base.AddTrigger<DealDamageAction>((dda) => dda.DamageSource.IsEnvironmentTarget && dda.Target == base.GetCardThisCardIsNextTo(), DealDamageActionResponse, new TriggerType[] { TriggerType.DiscardCard, TriggerType.RedirectDamage }, TriggerTiming.Before, new ActionDescription[] { ActionDescription.Unspecified }, false, requireActionSuccess: true, true);
            base.AddIfTheTargetThatThisCardIsNextToLeavesPlayDestroyThisCardTrigger();
        }

        private IEnumerator DealDamageActionResponse(DealDamageAction dealDamageAction)
        {
            IEnumerator coroutine;
            HeroTurnTakerController heroTurnTakerController = base.GameController.FindHeroTurnTakerController(base.GetCardThisCardIsNextTo().Owner.ToHero());
            List<DiscardCardAction> discardCardActions = new List<DiscardCardAction>();

            // When the hero next to this card would be dealt damage by an environment target, 
            // they may discard a card. If they do, redirect that damage.
            coroutine = base.GameController.SelectAndDiscardCards(heroTurnTakerController, 1, false, 0, storedResults: discardCardActions, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
            
            if (base.DidDiscardCards(discardCardActions))
            {
                coroutine = base.GameController.SelectTargetAndRedirectDamage(heroTurnTakerController, (card) => true, dealDamageAction, cardSource: base.GetCardSource());
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
        
        public override IEnumerator Play()
        {
            IEnumerator coroutine;

            // Play the top card of the environment deck.
            coroutine = PlayTheTopCardOfTheEnvironmentDeckWithMessageResponse(null);
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
