using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

using Handelabra;

namespace Cauldron.Gyrosaur
{
    public abstract class GyrosaurUtilityCardController : CardController
    {
        protected const string StabilizerKey = "GyroStabilizerEffectKey";
        protected GyrosaurUtilityCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        protected IEnumerator EvaluateCrashInHand(List<int> storedModifier, Func<bool> showDecisionIf = null, List<bool> didSkip = null)
        {
            if(showDecisionIf == null)
            {
                showDecisionIf = () => true;
            }
            Func<bool> fullShowDecisionIf = () => showDecisionIf();
            if(RARTInPlay)
            {
                fullShowDecisionIf = () => showDecisionIf() || RARTShowDecision();
            }
            //allow decision of increase/decrease if needed
            //sometimes it is not, as Gyro Stabilizer cannot increase/decrease the count past the requisite threshold

            if (CanActivateEffect(DecisionMaker, StabilizerKey) && fullShowDecisionIf())
            {
                IEnumerator coroutine;
                int currentCrash = TrueCrashInHand;

                string lowerWord = $"{currentCrash - 1} crash {(currentCrash - 1).ToString_SingularOrPlural("card", "cards")}";
                string sameWord = $"{currentCrash} crash {currentCrash.ToString_SingularOrPlural("card", "cards")}";
                string raiseWord = $"{currentCrash + 1} crash {(currentCrash + 1).ToString_SingularOrPlural("card", "cards")}";
                var words = new List<string>();
                if(currentCrash - 1 >= 0)
                {
                    words.Add(lowerWord);
                }
                words.Add(sameWord);
                words.Add(raiseWord);


                List<SelectWordDecision> storedWord = new List<SelectWordDecision>() ;
                coroutine = GameController.SelectWord(DecisionMaker, words, SelectionType.Custom, storedWord, optional: false, associatedCards: TurnTaker.GetCardsByIdentifier("GyroStabilizer"), cardSource: GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                if (DidSelectWord(storedWord))
                {
                    storedModifier.Add(CrashModifierFromDecision(storedWord.First()));
                }
                else
                {
                    if (didSkip != null)
                    {
                        didSkip.Add(true);
                    }
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
            return card != null && GameController.DoesCardContainKeyword(card, "crash");
        }

        protected bool RARTInPlay => TurnTaker.GetCardsWhere((Card c) => c.Identifier == "RecklessAlienRacingTortoise" && c.IsInPlayAndHasGameText).Any();
        protected Func<bool> RARTShowDecision => () => Game.ActiveTurnTaker == TurnTaker && TrueCrashInHand >= 3;
        //if there are at least 3 crash cards in hand and Reckless Alien Racing Tortoise is in play, we ALWAYS need to present
        //the Gyro Stabilizer choice, as it may set off RART, even if it doesn't otherwise matter to the effect in question
        protected int CrashModifierFromDecision(SelectWordDecision swd)
        {
            if(swd.SelectedWord != null)
            {
                int numChoices = swd.NumberOfChoices.Value;
                if(numChoices == 3 && swd.SelectedWord == swd.Choices.First())
                {
                    return -1;
                }
                
                if(swd.SelectedWord == swd.Choices.Last())
                {
                    return 1;
                }

            }
            return 0;
        }

        protected void ShowCrashInHandCount(bool otherInHand = false)
        {
            var standard = SpecialStringMaker.ShowNumberOfCardsAtLocation(HeroTurnTaker.Hand, new LinqCardCriteria((Card c) => IsCrash(c), "crash"));
            if(otherInHand)
            {
                standard.Condition = () => !Card.Location.IsHand;
                SpecialStringMaker.ShowNumberOfCardsAtLocation(HeroTurnTaker.Hand, new LinqCardCriteria((Card c) => c != this.Card && IsCrash(c), "other crash")).Condition = () => Card.Location.IsHand;
            }
        }

        public override CustomDecisionText GetCustomDecisionText(IDecision decision)
        {

            return new CustomDecisionText("how many crash cards should be counted by Gyro Stabilizer?", "How many crash cards should be counted by Gyro Stabilizer?", "Vote for how many crash cards should be counted by Gyro Stabilizer?", "crash cards should be counted by Gyro Stabilizer");

        }
    }
}
