using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.Gyrosaur
{
    public abstract class GyrosaurUtilityCharacterCardController : HeroCharacterCardController
    {
        protected const string StabilizerKey = "GyroStabilizerEffectKey";
        public GyrosaurUtilityCharacterCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }
        protected IEnumerator EvaluateCrashInHand(List<int> storedModifier, Func<bool> showDecisionIf = null)
        {
            if (showDecisionIf == null)
            {
                showDecisionIf = () => true;
            }
            Func<bool> fullShowDecisionIf = () => showDecisionIf();
            if (RARTInPlay)
            {
                fullShowDecisionIf = () => showDecisionIf() || RARTShowDecision();
            }
            //allow decision of increase/decrease if needed
            //sometimes it is not, as Gyro Stabilizer cannot increase/decrease the count past the requisite threshold

            if (CanActivateEffect(Card, StabilizerKey) && fullShowDecisionIf())
            {
                IEnumerator coroutine;
                int currentCrash = TrueCrashInHand;
                var functions = new List<Function>
                {
                    new Function(DecisionMaker, $"{currentCrash - 1} crash card(s)", SelectionType.RemoveTokens, () => DoNothing(), onlyDisplayIfTrue: currentCrash > 0),
                    new Function(DecisionMaker, $"{currentCrash} crash card(s)", SelectionType.None, () => DoNothing()),
                    new Function(DecisionMaker, $"{currentCrash + 1} crash card(s)", SelectionType.AddTokens, () => DoNothing()),
                };
                var selectFunction = new SelectFunctionDecision(GameController, DecisionMaker, functions, true, cardSource: GetCardSource(), associatedCards: TurnTaker.GetCardsByIdentifier("GyroStabilizer"));

                var storedFunction = new List<SelectFunctionDecision>();
                coroutine = GameController.SelectAndPerformFunction(selectFunction, storedFunction);
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                if (DidSelectFunction(storedFunction, DecisionMaker))
                {
                    storedModifier.Add(CrashModifierFromDecision(storedFunction.FirstOrDefault()));
                }
                else
                {
                    storedModifier.Add(0);
                }
            }
            else
            {
                storedModifier.Add(0);
                yield return null;
            }
            yield break;
        }

        protected int TrueCrashInHand => TurnTaker.GetCardsWhere((Card c) => c.Location == HeroTurnTaker.Hand && IsCrash(c)).Count();

        protected bool IsCrash(Card card)
        {
            return GameController.DoesCardContainKeyword(card, "crash");
        }

        protected bool RARTInPlay => TurnTaker.GetCardsWhere((Card c) => c.Identifier == "RecklessAlienRacingTortise" && c.IsInPlayAndHasGameText).Any();
        protected Func<bool> RARTShowDecision => () => Game.ActiveTurnTaker == TurnTaker && TrueCrashInHand >= 3;
        //if there are at least 3 crash cards in hand and Reckless Alien Racing Tortoise is in play, we ALWAYS need to present
        //the Gyro Stabilizer choice, as it may set off RART, even if it doesn't otherwise matter to the effect in question
        protected int CrashModifierFromDecision(SelectFunctionDecision sfd)
        {
            if (sfd.SelectedFunction != null)
            {
                var selectedType = sfd.SelectedFunction.SelectionType;
                if (selectedType == SelectionType.RemoveTokens)
                {
                    return -1;
                }
                if (selectedType == SelectionType.AddTokens)
                {
                    return 1;
                }
            }
            return 0;
        }
    }
}
