using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Handelabra.Sentinels.Engine.Controller;
using Handelabra.Sentinels.Engine.Model;

namespace Cauldron.OblaskCrater
{
    public class GentleTransportCardController : OblaskCraterUtilityCardController
    {
        /* 
         * At the end of the environment turn, move this card next to a hero. Then the predator card with the highest HP deals
         * each target in this card's play area {H - 1} melee damage. 
         * If the hero next to this card plays a card with a power on it, they may immediately use a power on that card.
         */
        public GentleTransportCardController(Card card, TurnTakerController turnTakerController) : base(card, turnTakerController)
        {
        }

        public override void AddTriggers()
        {
            base.AddEndOfTurnTrigger((ttc)=> ttc == base.FindEnvironment().TurnTaker, PhaseChangeActionResponse, new TriggerType[] { TriggerType.MoveCard, TriggerType.DealDamage });
            base.AddTrigger<PlayCardAction>((pca) => pca.CardToPlay.HasPowers && base.GetCardThisCardIsNextTo() == pca.CardToPlay.Owner.CharacterCard, PlayCardActionResponse, TriggerType.UsePower, TriggerTiming.After);
        }

        private IEnumerator PhaseChangeActionResponse(PhaseChangeAction phaseChangeAction)
        {
            List<SelectCardDecision> storedSelectHeroResults = new List<SelectCardDecision>();
            List<SelectCardDecision> storedSelectPredatorResults = new List<SelectCardDecision>();
            IEnumerable<Card> highestPredators;
            IEnumerable<Card> targetsInPlayArea;
            IEnumerator coroutine;
            Card selectedHero;

            coroutine = base.GameController.SelectHeroCharacterCard(base.DecisionMaker, SelectionType.MoveCardNextToCard, storedSelectHeroResults, cardSource: base.GetCardSource());
            if (base.UseUnityCoroutines)
            {
                yield return base.GameController.StartCoroutine(coroutine);
            }
            else
            {
                base.GameController.ExhaustCoroutine(coroutine);
            }

            if (storedSelectHeroResults != null && storedSelectHeroResults.Count() > 0)
            {
                selectedHero = storedSelectHeroResults.FirstOrDefault().SelectedCard;

                // move this card next to a hero
                coroutine = base.GameController.MoveCard(base.TurnTakerController, base.Card, selectedHero.NextToLocation,
                            showMessage: false,
                            flipFaceDown: false,
                            cardSource: base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }

                // Then the predator card with the highest HP deals each target in this card's play area {H - 1} melee damage. 
                highestPredators = base.GameController.FindAllTargetsWithHighestHitPoints(1, (card) => card.DoKeywordsContain("predator"), base.GetCardSource());
                coroutine = base.GameController.SelectCardAndStoreResults(base.DecisionMaker, SelectionType.HighestHP, new LinqCardCriteria((lcc) => highestPredators.Contains(lcc)), storedSelectPredatorResults, false, cardSource: base.GetCardSource());
                if (base.UseUnityCoroutines)
                {
                    yield return base.GameController.StartCoroutine(coroutine);
                }
                else
                {
                    base.GameController.ExhaustCoroutine(coroutine);
                }
                
                if (storedSelectPredatorResults != null && storedSelectPredatorResults.Count() > 0)
                {
                    targetsInPlayArea = selectedHero.Owner.GetPlayAreaCards().Where(card => card.IsTarget);
                    coroutine = base.DealDamage(storedSelectPredatorResults.FirstOrDefault().SelectedCard, card => targetsInPlayArea.Contains(card), base.H - 1, DamageType.Melee);
                    if (base.UseUnityCoroutines)
                    {
                        yield return base.GameController.StartCoroutine(coroutine);
                    }
                    else
                    {
                        base.GameController.ExhaustCoroutine(coroutine);
                    }
                }
            }
            yield break;
        }

        private IEnumerator PlayCardActionResponse(PlayCardAction playCardAction)
        {
            IEnumerator coroutine;

            coroutine = base.GameController.SelectAndUsePower(playCardAction.TurnTakerController.ToHero(), true, (power) => power.CardController.Card ==  playCardAction.CardToPlay, cardSource: base.GetCardSource());
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