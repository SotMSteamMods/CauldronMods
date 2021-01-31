using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Handelabra;

namespace Cauldron.MagnificentMara
{
    public class DowsingCrystalCardController : CardController
    {
        public override bool AllowFastCoroutinesDuringPretend { 
            get
            {
                if(this.Card.IsInPlay && Game.StatusEffects.Any((StatusEffect se) => se.CardSource == this.Card))
                {
                    return false;
                }
                return true;
            }
        }
        public DowsingCrystalCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
            AddThisCardControllerToList(CardControllerListType.CanCauseDamageOutOfPlay);
        }

        public override IEnumerator Play()
        {
            yield break;
        }
        public override IEnumerator UsePower(int index = 0)
        {
            //"Once before your next turn when a non-hero card enters play, one hero target may deal a non-hero target 2 damage of a type of their choosing. You may destroy this card to increase that damage by 2."

            //There is no way to set up a status effect that triggers when any card enters play.
            //The 'notice triggering effect and deal damage' part of this effect lives on MaraUtilityCharacter, 
            //this notifies it that it's time to start working and controls the destroy-crystal-to-boost-damage part.

            int numDamage = GetPowerNumeral(0, 2);
            int numBoost = GetPowerNumeral(1, 2);
            int[] numerals = new int[] { numDamage, numBoost };

            var dowsingEffect = new OnDealDamageStatusEffect(CardWithoutReplacements, nameof(DowsingCrystalDamageBoostResponse), $"Once before {DecisionMaker.Name}'s next turn when a non-hero card enters play, one hero target may deal a non-hero target 2 damage of a type of their choosing. You may destroy {Card.Title} to increase that damage by 2.", new TriggerType[] { TriggerType.IncreaseDamage, TriggerType.Hidden }, DecisionMaker.TurnTaker, Card, numerals);
            dowsingEffect.UntilStartOfNextTurn(TurnTaker);
            dowsingEffect.DoesDealDamage = true;
            dowsingEffect.SourceCriteria.IsHero = true;

            IEnumerator coroutine = GameController.AddStatusEffect(dowsingEffect, true, GetCardSource());
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

        public IEnumerator DowsingCrystalDamageBoostResponse(DealDamageAction dd, TurnTaker hero, StatusEffect effect, int[] powerNumerals = null)
        {
            if(dd.CardSource != null && dd.CardSource.StatusEffectSource == effect)
            {
                Card sourceCrystal = dd.CardSource.Card;
                if (sourceCrystal != null && sourceCrystal.IsInPlay)
                {
                    List<YesNoCardDecision> stored = new List<YesNoCardDecision> { };
                    IEnumerator decision = GameController.MakeYesNoCardDecision(DecisionMaker, SelectionType.DestroyCard, sourceCrystal, dd, stored, cardSource: dd.CardSource);
                    if (UseUnityCoroutines)
                    {
                        yield return GameController.StartCoroutine(decision);
                    }
                    else
                    {
                        GameController.ExhaustCoroutine(decision);
                    }

                    if (DidPlayerAnswerYes(stored))
                    {
                        IEnumerator coroutine;
                        if (IsRealAction())
                        {
                            coroutine = GameController.DestroyCard(DecisionMaker, sourceCrystal, cardSource: dd.CardSource);
                            if (UseUnityCoroutines)
                            {
                                yield return GameController.StartCoroutine(coroutine);
                            }
                            else
                            {
                                GameController.ExhaustCoroutine(coroutine);
                            }
                        }

                        //GameController.RemoveInhibitor(FindCardController(sourceCrystal));
                        int numBoost = powerNumerals?[1] ?? 2;
                        coroutine = GameController.IncreaseDamage(dd, numBoost, false, dd.CardSource);
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
                else if (!sourceCrystal.IsInPlay)
                {
                    GameController.RemoveInhibitor(FindCardController(sourceCrystal));
                }
                yield return null;
            }

            yield break;
        }
    }
}