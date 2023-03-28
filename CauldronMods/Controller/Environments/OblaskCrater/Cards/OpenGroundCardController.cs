using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.OblaskCrater
{
    public class OpenGroundCardController : OblaskCraterUtilityCardController
    {
        /* 
         * Once per turn when a hero target would be dealt damage by a non-hero target, they may increase that damage
         * by 1 and play the top card of the environment deck.
         * If that card is a target, redirect that damage to it.
         */
        private const string FirstTimeWouldBeDealtDamage = "OncePerTurn";

        public OpenGroundCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            base.SpecialStringMaker.ShowHasBeenUsedThisTurn(FirstTimeWouldBeDealtDamage);
            this.AllowFastCoroutinesDuringPretend = false;
        }

        public override void AddTriggers()
        {
            base.AddTrigger<DealDamageAction>((dda)=> !base.IsPropertyTrue(FirstTimeWouldBeDealtDamage) && IsHeroTarget(dda.Target) && !dda.DamageSource.IsHero && dda.DamageSource.IsTarget, DealDamageActionResponse, new TriggerType[]
                {
                    TriggerType.IncreaseDamage,
                    TriggerType.PlayCard,
                    TriggerType.RedirectDamage
                }, TriggerTiming.Before, requireActionSuccess: true, isActionOptional: true);

            AddAfterLeavesPlayAction((GameAction ga) => ResetFlagAfterLeavesPlay(FirstTimeWouldBeDealtDamage), TriggerType.Hidden);
        }

        private IEnumerator DealDamageActionResponse(DealDamageAction dealDamageAction)
        {
            IEnumerator coroutine;
            HeroTurnTakerController heroTurnTakerController = base.GameController.FindHeroTurnTakerController(dealDamageAction.Target.Owner.ToHero());
            List<YesNoCardDecision> yesNoCardDecisions = new List<YesNoCardDecision>();
            List<PlayCardAction> playCardActions = new List<PlayCardAction>();

            coroutine = base.GameController.MakeYesNoCardDecision(heroTurnTakerController, SelectionType.PlayTopCardOfEnvironmentDeck, base.Card, action: dealDamageAction, storedResults: yesNoCardDecisions, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }
                        
            if (base.DidPlayerAnswerYes(yesNoCardDecisions))
            {
                // they may increase that damage by 1
                coroutine = base.GameController.IncreaseDamage(dealDamageAction, 1, cardSource: base.GetCardSource()); 
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                // play the top card of the environment deck
                coroutine = base.GameController.PlayTopCard(base.DecisionMaker, base.TurnTakerController, storedResults: playCardActions, cardSource: base.GetCardSource());
                if (this.UseUnityCoroutines)
                {
                    yield return this.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    this.GameController.ExhaustCoroutine(coroutine);
                }

                // If that card is a target,
                if (base.DidPlayCards(playCardActions) && playCardActions.Count((pca) => pca.CardToPlay.IsTarget) > 0)
                {
                    // redirect that damage to it.
                    coroutine = base.GameController.RedirectDamage(dealDamageAction, playCardActions.FirstOrDefault().CardToPlay, cardSource: base.GetCardSource());
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                }

                base.SetCardPropertyToTrueIfRealAction(FirstTimeWouldBeDealtDamage);

            }

            yield break;
        }
    }
}
